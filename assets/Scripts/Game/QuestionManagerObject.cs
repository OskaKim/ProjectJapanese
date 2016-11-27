using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class QuestionManagerObject : MonoBehaviour
{
    /*エディターで格納*/
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

    Question.QuestionManager QuestionFuncs;

    //次の問題表示までのタイマー
    float timerForNextQues = 0;

    //問題の制限時間
    const float TIMEOUTTICK = 10.0f;
    float timeoutTimer = TIMEOUTTICK;

    void Start()
    {
        //点数初期化
        CurrentlyLoginInfo.SCORE = 0;
        //旧 初期化
        //QuestionFuncs = new Question.QuestionManager(QuestionText, AnswerPrefab, AnswerText, "Lesson1");
        //新 初期化
        Question.QuestionManager.Create(ref QuestionFuncs, QuestionText, AnswerPrefab, AnswerText, this, "Lesson" + CurrentlyLoginInfo.LEVEL);
    }
    public void StartAnimationEnd()
    {
        QuestionFuncs.UpdateToNextQuestion();
    }

    void Update()
    {
        Question.SolveStatus status = Question.SolveStatus.NotSelected;
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
            //制限時間を数える
            timeoutTimer -= Time.deltaTime;

            //時間切れ
            if (timeoutTimer <= 0)
            {
                timerForNextQues = Question.QuestionManager.TICK_X;
                /*間違ったこととみなす*/
                status = Question.SolveStatus.Incorrect;
                QuestionFuncs.ClearQuestion();
            }
            
            //テキストオブジェクトに出力
            TimeoutText.text = "残り：" + (int)timeoutTimer + "秒";
        }

        //問題がまだ設定されていない状況なら処理終了
        if (status == Question.SolveStatus.NotSelected)
            return;


        /*不正解だったらダメージアニメ開始*/
        switch(status)
        {
            case Question.SolveStatus.Correct:
                CurrentScoreText.text = "点数：" + CurrentlyLoginInfo.SCORE.ToString();
                break;
            case Question.SolveStatus.Incorrect:
                Panel.SetBool("Damaged", true);
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
