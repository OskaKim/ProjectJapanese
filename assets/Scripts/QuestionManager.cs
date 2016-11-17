using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/*問題に関する機能がまとめされている*/
/*このスクリプトはゲームオブジェクトに貼らない*/
namespace Question
{
    //問題の構造体
    public struct QuesStructor
    {
        public int type;
        public string ques;
        public List<string> ans;
        public int corAns;
    }

    //インスタンスを生成し、初期化してから使う
    public class QuestionManager : ScriptableObject
    {
        /*問題のリスト*/
        List<QuesStructor> questions;
        QuesStructor curQues;

        /*オブジェクトインスタンスの格納が必要*/
        Text QuestionText;
        Button AnswerPrefab;
        Text AnswerText;

        /*シーン上に存在する答えオブジェクトのリスト（オブジェクト管理のため）*/
        List<Button> CurAnswers;

        /*正誤チェック用*/
        public static int curType = 1;
        public static string selectedAns = null;
        public static bool bCheckAnswer = false;

        /*新 初期化*/ //warning対策のため
        public static void Create(ref QuestionManager Inst, Text quesTextObj, Button ansPrefab, Text answerTextObj, string fileName = "Lesson1")
        {
            Inst = ScriptableObject.CreateInstance<Question.QuestionManager>();

            //初期化
            Inst.questions = new List<QuesStructor>();

            FileSystem.QuestionFileManager FileMng = new FileSystem.QuestionFileManager(fileName);

            //ファイルシステムから問題のリストを参照格納
            FileMng.SetQuestions(ref Inst.questions);

            //初期化
            Inst.QuestionText = quesTextObj;
            Inst.AnswerPrefab = ansPrefab;
            Inst.AnswerText = answerTextObj;
            Inst.CurAnswers = new List<Button>();
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
        
        public void Update()
        {
            CheckAnswerPhase();
            AnswerText.text = GetAnswerDisplay();
        }

        /*答えから、文章の組み合わせを返す*/
        string GetAnswerDisplay()
        {
            if (selectedAns == null)
                return null;

            string displayAns = null;
            foreach(var ans in selectedAns)
            {
                displayAns += curQues.ans[(int)char.GetNumericValue(ans) - 1];
            }
            Debug.Log(displayAns);
            
            return displayAns;
        }

        /*答えチェック段階*/
        void CheckAnswerPhase()
        {
            if (!bCheckAnswer)
                return;

            var currentCorrect = curQues.corAns.ToString();

            bool isCorrect = false;
            if(selectedAns == currentCorrect){
                isCorrect = true;
            }

            string display =
                isCorrect ?
                "正解" : "×";

            /*点数記録*/
            if (isCorrect) CurrentlyLoginInfo.SCORE++;

            /*現在の問題は終了*/
            bCheckAnswer = false;
            currentCorrect = null;
            selectedAns = null;
            ClearQuestion();

            //正誤表示
            QuestionText.text = display;
        }

        /*num番目問題にアップデートする*/
        public void UpdateQuestion(int num)
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
            }
            setQues(curQues);
        }

        /*シーン上に残っている問題をClearする*/
        public void ClearQuestion()
        {
            QuestionText.text = null;
            ClearAnswer();

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
            float LengthofString = buttonText.text.Length;
            buttonsize = new Vector2(buttonText.fontSize * LengthofString, buttonsize.y);
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

            Vector2 offset = new Vector2(200, Screen.height/2);

            for(int i = 0; i < ques.ans.Count; ++i)
            {
                //ボタン生成
                Button button = Instantiate(AnswerPrefab);
                button.transform.SetParent(myCanvas.transform);

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
                offset = new Vector2(offset.x + buttonsize.x, offset.y);

                //管理のため、リストに入れる
                CurAnswers.Add(button);
            }
        }
        public void SetQuestion_type2(Question.QuesStructor ques)
        {
            var myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

            QuestionDefaultlySetting(ques);

            Vector2 offset = new Vector2(200, Screen.height / 2 + 100);

            for (int i = 0; i < ques.ans.Count; ++i)
            {
                //ボタン生成
                Button button = Instantiate(AnswerPrefab);
                button.transform.SetParent(myCanvas.transform);

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
                offset = new Vector2(offset.x, offset.y - buttonsize.y);

                //管理のため、リストに入れる
                CurAnswers.Add(button);
            }
        }
    }
}