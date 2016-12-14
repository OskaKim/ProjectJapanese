using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class QuestionManagerObject : MonoBehaviour
{
    /*エディターで格納*/

    //Objects
    [SerializeField]
    Text QuestionText;
    [SerializeField]
    Button AnswerPrefab;
    [SerializeField]
    Text AnswerText;
    [SerializeField]
    Text CorrectAnswerText;
    [SerializeField]
    Animator Panel;
    [SerializeField]
    Text CurrentScoreText;
    [SerializeField]
    Text TypeText;
    [SerializeField]
    Text TimeoutText;
    [SerializeField]
    Scrollbar TimerBar;
    [SerializeField]
    SEManager SEManagerPrefab;
    [SerializeField]
    Animator CorrectImageAnimation;
    [SerializeField]
    Animator IncorrectImageAnimation;
    [SerializeField]
    Transform Boss;
    [SerializeField]
    Animator WarningAnimator;
    [SerializeField]
    GameObject[] ObjectsForWarningAnimation = new GameObject[0];

    Question.QuestionManager QuestionFuncs;

    //次の問題表示までのタイマー
    float timerForNextQues = 0;

    //問題の制限時間
    const float TIMEOUTTICK = 10.0f;
    float timeoutTimer = TIMEOUTTICK;

    //現在のステータス
    public Question.SolveStatus status;

    void Start()
    {
        //SEのインスタンス生成
        SEManagerPrefab = Instantiate(SEManagerPrefab) as SEManager;
        SEManagerPrefab.name = "SEs";
        //初期化
        Question.QuestionManager.Create(ref QuestionFuncs, SEManagerPrefab, QuestionText, AnswerPrefab, AnswerText, CorrectAnswerText, this, "Lesson" + CurrentlyUserInfo.selectedLevel.ToString());
        status = Question.SolveStatus.Waiting;
    }
    public void StartAnimationEnd()
    {
        QuestionFuncs.UpdateToNextQuestion();
    }

    void Update()
    {
        QuestionFuncs.UpdateQuestion(ref timerForNextQues, ref status);
                
        /*timerForNextQuesが更新されたら、timerForNextQues秒後に次の問題を出力*/
        if (timerForNextQues > 0.0f)
        {
            timerForNextQues -= Time.deltaTime;
            if (timerForNextQues <= 0)
            {
                timerForNextQues = 0;
                QuestionFuncs.UpdateToNextQuestion();
                timeoutTimer = TIMEOUTTICK;
            }
        }
        else
        {
            if (status == Question.SolveStatus.Solving)
            {
                //制限時間を数える
                timeoutTimer -= Time.deltaTime;
            }

            //時間切れ
            if (timeoutTimer <= 0)
            {
                timerForNextQues = Question.QuestionManager.TICK_X;
                /*間違ったこととみなす*/
                if (status == Question.SolveStatus.Solving)
                    status = Question.SolveStatus.Incorrect;
                QuestionFuncs.ClearQuestion();
                QuestionText.text = "時間切れ";
            }
            
            //テキストオブジェクトに出力
            TimeoutText.text = "残り：" + (int)timeoutTimer + "秒";

            //タイマーをスクロールバーで表示
            TimerBar.size = timeoutTimer / TIMEOUTTICK;
        }

        ////問題がまだ設定されていない状況なら処理終了
        //if (status == Question.SolveStatus.Solving)
        //    return;


        /*不正解だったらダメージアニメ開始*/
        switch(status)
        {
            case Question.SolveStatus.Correct:
                CurrentScoreText.text = "点数：" + CurrentlyUserInfo.score.ToString();
                break;
            case Question.SolveStatus.Incorrect:
                if (!Panel.GetBool("Damaged")){
                    Panel.SetBool("Damaged", true);
                }
                break;
            default:
                break;
        }

        //WarningAnimation終了
        if(WarningAnimation && WarningAnimator.GetBool("IsEnd"))
        {
            WarningAnimation = false;
            QuestionFuncs.UpdateToNextQuestion();
        }
    }

    public void ClearAnswerButton()
    {
        QuestionFuncs.ClearAnswer();
    }

    public void TypeTextUpdate(string text)
    {
        TypeText.text = text;
    }

    /*Animation*/
    public void PlayAnimation_CorrectImage()
    {
        CorrectImageAnimation.SetBool("bFlag", true);
    }
    public void PlayAnimation_IncorrectImage()
    {
        IncorrectImageAnimation.SetBool("bFlag", true);
    }

    /*ボスオブジェクト*/
    public Transform GetBossTransform()
    {
        return Boss;
    }

    //warning animaion
    bool mWarningAnimation = false;
    public bool WarningAnimation
    {
        get
        {
            return mWarningAnimation;
        }
        set
        {
            mWarningAnimation = value;
            WarningAnimator.gameObject.SetActive(value);
            //かかわるオブジェクトを設定
            foreach (var o in ObjectsForWarningAnimation){
                o.SetActive(value);
            }
        }
    }
}
