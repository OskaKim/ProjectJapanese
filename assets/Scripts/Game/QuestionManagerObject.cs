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

    Question.QuestionManager QuestionFuncs;

    //次の問題表示までのタイマー
    float timerForNextQues = 0;

    void Start()
    {
        //点数初期化
        CurrentlyLoginInfo.SCORE = 0;
        //旧 初期化
        //QuestionFuncs = new Question.QuestionManager(QuestionText, AnswerPrefab, AnswerText, "Lesson1");
        //新 初期化
        Question.QuestionManager.Create(ref QuestionFuncs, QuestionText, AnswerPrefab, AnswerText, "Lesson" + CurrentlyLoginInfo.LEVEL);
        QuestionFuncs.UpdateToNextQuestion();
    }

    void Update()
    {
        QuestionFuncs.UpdateQuestion(ref timerForNextQues);
        
        /*timerForNextQuesが更新されたら、timerForNextQues秒後に次の問題を出力*/
        if(timerForNextQues > 0.0f)
        {
            timerForNextQues -= Time.deltaTime;
            if(timerForNextQues <= 0)
            {
                timerForNextQues = 0;
                QuestionFuncs.UpdateToNextQuestion();
            }
        }
    }

    public void ClearAnswerButton()
    {
        QuestionFuncs.ClearAnswer();
    }
}
