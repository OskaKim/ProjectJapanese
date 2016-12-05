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
        SEManagerPrefab = Instantiate(SEManagerPrefab) as SEManager;
        //初期化
        Question.QuestionManager.Create(ref QuestionFuncs, SEManagerPrefab, QuestionText, AnswerPrefab, AnswerText, this, "Lesson" + CurrentlyUserInfo.selectedLevel.ToString());
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
    }

    public void ClearAnswerButton()
    {
        QuestionFuncs.ClearAnswer();
    }

    public void TypeTextUpdate(string text)
    {
        TypeText.text = text;
    }
}
