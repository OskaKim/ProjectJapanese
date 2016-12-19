using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System;

/*問題に関する機能がまとめされている*/
/*このスクリプトはゲームオブジェクトに貼らない*/
namespace Question
{
    //問題の構造体
    public struct QuesStructor
    {
        public int type;
        public string typetext;
        public string ques;
        public List<string> ans;
        public int corAns;
    }

    //問題解きの状態
    public enum SolveStatus {Waiting, Solving, Correct, Incorrect, Stoping};

    //インスタンスを生成し、初期化してから使う
    public class QuestionManager : ScriptableObject
    {
        //問題のファイルの名前
        string fileName;
        //現在の問題番号
        int curQuesNum = 0;
        //次の問題表示までのタイマー
        public const float TICK_O = 1.0f, TICK_X = 1.5f;

        /*タイプの題を見せてから問題が出るまでのタイマー*/
        const float TICK_NEXTQUES = 2.0f;
        float timerForNextQues = 0;

        /*問題のリスト*/
        List<QuesStructor> questions;
        QuesStructor curQues;

        /*オブジェクトインスタンスの格納が必要*/
        Text QuestionText;
        Button AnswerPrefab;
        Text AnswerText;
        Text CorrectAnswerText;
        Text TypeText;

        /*QuestionManagerオブジェクトの関数を呼ぶ場合使う*/
        QuestionManagerObject ManagerObj;

        /*シーン上に存在する答えオブジェクトのリスト（オブジェクト管理のため）*/
        List<Button> CurAnswers;

        /*正誤チェック用*/
        public static int curType = 1;
        public static string selectedAns = null;
        public static bool bCheckAnswer = false;

        /*サウンドエフェクトマネジャー*/
        SEManager SEManagerInst;

        //ボスステージに進むに必要な点数
        int passScoreToBoss;

        /*
        ボスステージだったら、trueの状態。
        このparamを利用し、ボスだけの例外などを作成する。
        */
        bool bBossStage = false;

        /*新 初期化*/ //warning対策のため
        public static void Create(ref QuestionManager Inst, SEManager seManager, Text quesTextObj, Button ansPrefab, Text answerTextObj, Text correctAnswerTextObj, QuestionManagerObject obj, string fileName = "Lesson1")
        {
            Inst = CreateInstance<QuestionManager>();

            //初期化
            Inst.questions = new List<QuesStructor>();
            Inst.fileName = fileName;
            FileSystem.QuestionFileManager FileMng = new FileSystem.QuestionFileManager(fileName);
            //ファイルシステムから問題のリストを参照格納
            FileMng.SetQuestions(ref Inst.questions);

            //初期化
            Inst.QuestionText = quesTextObj;
            Inst.AnswerPrefab = ansPrefab;
            Inst.AnswerText = answerTextObj;
            Inst.CorrectAnswerText = correctAnswerTextObj;
            Inst.CurAnswers = new List<Button>();
            Inst.ManagerObj = obj;
            Inst.SEManagerInst = seManager;
            Inst.passScoreToBoss = FileMng.GetPassScore();

            //問題リストをシャッフル
            Inst.ShuffleQuestions();

            //ボス戦突入前と初期化
            CurrentlyUserInfo.bBoss = false;
        }

        /*旧 初期化*/
        //public QuestionManager(Text quesTextObj, Button ansPrefab, Text answerTextObj, string fileName = "Lesson1")
        //{
        //    FileSystem.FileManager FileMng = new FileSystem.FileManager(fileName);

        //    //ファイルシステムから問題のリストを参照格納
        //    FileMng.SetQuestions(ref questions);

        //    //初期化
        //    QuestionText = quesTextObj;
        //    AnswerPrefab = ansPrefab;
        //    AnswerText = answerTextObj;
        //    CurAnswers = new List<Button>();
        //}

        //シャッフル
        void ShuffleQuestions()
        {
            int n = questions.Count;
            QuesStructor t;
            int i;

            System.Random rnd = new System.Random();
            
            while(n > 0)
            {
                i = rnd.Next(0, n--);
                t = questions[n];
                questions[n] = questions[i];
                questions[i] = t;
            }
        }

        //update
        public void UpdateQuestion(ref float timer, ref SolveStatus status)
        {

            /*Stopping状態だったらアップデートしない
            *状況１：ボス戦進入に失敗
            */
            if (ManagerObj.status == SolveStatus.Stoping)
            {
                //@param1秒ごにスコアシーンへ移動
                ManagerObj.MoveToScoreSceneIn(5.0f);
                return;
            }

            if(GetAnswerDisplay() != null)
               AnswerText.text = GetAnswerDisplay();

            CheckAnswerPhase(ref timer, ref status);

            /*問題を出題するまで間を取る*/
            if (timerForNextQues < 0)
            {
                UpdateQuestion(curQuesNum++);
                timerForNextQues = 0;
            }
            else if (timerForNextQues > 0)
                timerForNextQues -= Time.deltaTime;
        }

        /*答えから、文章の組み合わせを返す*/
        string GetAnswerDisplay()
        {
            if (selectedAns == null)
                return null;

            string displayAns = null;
            foreach (var ans in selectedAns)
            {
                displayAns += curQues.ans[(int)char.GetNumericValue(ans) - 1];
            }

            return displayAns;
        }

        /*答えチェック段階*/
        void CheckAnswerPhase(ref float timer, ref SolveStatus status)
        {
            //問題が全部入力された
            if (!bCheckAnswer || (selectedAns != null && curQues.corAns.ToString().Length != selectedAns.Length))
                return;

            var currentCorrect = curQues.corAns.ToString();

            bool isCorrect = false;
            if (selectedAns == currentCorrect)
            {
                isCorrect = true;
            }
            
            //正誤によるパラメータ設定
            if(isCorrect)
            {
                ManagerObj.PlayAnimation_CorrectImage();
                timer = TICK_O;
                status = SolveStatus.Correct;
                SEManagerInst.Play(SEManager.SEs.Correct);

                //ボスステージの例外
                if (bBossStage)
                {
                    //ダメージエフェクト
                    ManagerObj.GetBoss().Damaged();
                }
            }
            else
            {
                ManagerObj.PlayAnimation_IncorrectImage();
                timer = TICK_X;
                status = SolveStatus.Incorrect;
                SEManagerInst.Play(SEManager.SEs.InCorrect);

                //ユーザーが選んだ答えを赤色で表示
                AnswerText.color = Color.red;
                //正解を表示
                DisplayCorrectAns(CorrectAnswerText);
            }

            /*点数記録*/
            if (isCorrect)
            {
                if (!bBossStage)
                    ++CurrentlyUserInfo.score;
                else
                    ++CurrentlyUserInfo.bossScore;
            }
            /*現在の問題は終了*/
            bCheckAnswer = false;
            currentCorrect = null;
            selectedAns = null;
            ClearQuestion();
        }

        /*正解を表示する*/
        void DisplayCorrectAns(Text DisplayText)
        {
            string displayCorAns = "正解：";
            foreach (var ans in curQues.corAns.ToString())
            {
                displayCorAns += curQues.ans[(int)char.GetNumericValue(ans) - 1];
            }
            DisplayText.text = displayCorAns;
        }

        /*num番目問題にアップデートする*/
        void UpdateQuestion(int num)
        {
            //クリア
            ClearQuestion();

            //現在の問題
            curQues = questions[num];

            //タイプによって問題を設定
            SetQuestion setQues = null;
            switch (curQues.type)
            {
                case 1:
                    setQues = new SetQuestion(SetQuestion_type1);
                    break;
                case 2:
                case 3:
                    setQues = new SetQuestion(SetQuestion_type2);
                    break;
                //別のタイプを追加する場合は新しい関数か、適する関数を呼出す必要がある。
                default:
                    setQues = new SetQuestion(SetQuestion_type2);
                    break;
            }
            setQues(curQues);
            ManagerObj.status = SolveStatus.Solving;
            Debug.Log(ManagerObj.status);
        }

        /*次の問題を表示*/
        public void UpdateToNextQuestion()
        {
            /*ボスステージ*/
            if (questions.Count <= curQuesNum)
            {
                /** ボスステージに入る前の条件
                * ボスステージ進入前 */
                if (!bBossStage)
                {
                    //クリア
                    ClearQuestion();
                    AnswerText.text = null;
                    CorrectAnswerText.text = null;

                    /*ステージのパース点数を超えた*/
                    if (passScoreToBoss <= CurrentlyUserInfo.score)
                    {
                        bBossStage = true;
                        CurrentlyUserInfo.bBoss = true;
                        ManagerObj.GetBossTransform().gameObject.SetActive(true);
                        curQuesNum = 0;
                        /*ボスステージに問題再設定*/
                        {
                            FileSystem.QuestionFileManager FileMng = new FileSystem.QuestionFileManager(fileName + "Boss");
                            //ファイルシステムから問題のリストを参照格納
                            FileMng.SetQuestions(ref questions);
                            //問題リストをシャッフル
                            ShuffleQuestions();
                        }
                        //warning animation　始動
                        ManagerObj.WarningAnimation = true;
                    }
                    /*ボスステージ進入失敗*/
                    else
                    {
                        ManagerObj.BossStartFailed();
                    }
                }
                //ボスステージクリア
                else if (bBossStage)
                {
                    SceneManager.LoadScene("ScoreScene");
                    //return;
                }
                return;
            }
            //クリア
            ClearQuestion();
            //タイプ文字を設定
            ManagerObj.TypeTextUpdate(questions[curQuesNum].typetext);
            timerForNextQues = TICK_NEXTQUES;
            //答え表示オブジェクト再設定
            AnswerText.color = Color.black;
            AnswerText.text = null;
            CorrectAnswerText.text = null;
            //UpdateQuestion(curQuesNum++);
        }

        /*シーン上に残っている問題をClearする*/
        public void ClearQuestion()
        {
            ManagerObj.TypeTextUpdate(null);
            QuestionText.text = null;
            ClearAnswer();
            //AnswerText.text = null;
            //AnswerText.color = Color.black;

            if (CurAnswers != null)
            {
                foreach (var ans in CurAnswers)
                {
                    Destroy(ans.gameObject);
                }
                CurAnswers.Clear();
            }
        }
        
        /*答えリストを初期化*/
        public void ClearAnswer()
        {
            selectedAns = null;
        }

        /*問題タイプによって、異なる設定をする*/
        delegate void SetQuestion(QuesStructor ques);

        /*ボタンのサイズ設定(全てのタイプで同じ処理)*/
        void SetButtonSize(Text buttonText, RectTransform buttonRectTransfrom, ref Vector2 buttonsize)
        {
            const float addedWidth = 50f, addedHeight = 5f;
            float LengthofString = buttonText.text.Length;
            buttonsize = new Vector2(buttonText.fontSize * LengthofString + addedWidth, buttonsize.y + addedHeight);
            buttonRectTransfrom.sizeDelta = buttonsize;
        }

        void QuestionDefaultlySetting(Question.QuesStructor ques)
        {
            //タイプ
            curType = ques.type;
            //質問
            QuestionText.text = "Q. " + ques.ques;
        }
        public void SetQuestion_type1(Question.QuesStructor ques)
        {
            var myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

            QuestionDefaultlySetting(ques);

            Vector2 offset = new Vector2(10, Screen.height / 2);

            for (int i = 0; i < ques.ans.Count; ++i)
            {
                //ボタン生成
                Button button = Instantiate(AnswerPrefab);
                button.transform.SetParent(myCanvas.transform.FindChild("Panel"));
                button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                //ボタンの名前は答え番号で
                button.name = (i + 1).ToString();

                //答えの内容設定
                Text ButtonText = button.transform.GetChild(0).GetComponent<Text>();
                ButtonText.text = ques.ans[i];

                /*ボタンのサイズ設定*/
                Vector2 buttonsize = button.GetComponent<RectTransform>().sizeDelta;
                SetButtonSize(ButtonText, button.GetComponent<RectTransform>(), ref buttonsize);

                //位置設定
                //float offset = buttonsize.x;
                button.transform.position = offset;
                offset = new Vector2(offset.x + buttonsize.x * myCanvas.GetComponent<RectTransform>().localScale.x, offset.y);

                //管理のため、リストに入れる
                CurAnswers.Add(button);
            }
        }
        public void SetQuestion_type2(Question.QuesStructor ques)
        {
            var myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

            QuestionDefaultlySetting(ques);

            Vector2 offset = new Vector2(50, Screen.height / 2);

            for (int i = 0; i < ques.ans.Count; ++i)
            {
                //ボタン生成
                Button button = Instantiate(AnswerPrefab);
                button.transform.SetParent(myCanvas.transform.FindChild("Panel"));

                button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                //ボタンの名前は答え番号で
                button.name = (i + 1).ToString();

                //答えの内容設定
                Text ButtonText = button.transform.GetChild(0).GetComponent<Text>();
                ButtonText.text = ques.ans[i];

                /*ボタンのサイズ設定*/
                Vector2 buttonsize = button.GetComponent<RectTransform>().sizeDelta;
                SetButtonSize(ButtonText, button.GetComponent<RectTransform>(), ref buttonsize);

                //位置設定
                //float offset = buttonsize.x;
                const float spaceBetweenButtons = 0.1f;
                button.transform.position = offset;
                offset = new Vector2(offset.x, offset.y - buttonsize.y *(spaceBetweenButtons + myCanvas.GetComponent<RectTransform>().localScale.y));

                //管理のため、リストに入れる
                CurAnswers.Add(button);
            }
        }
    }
}