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

    void Start()
    {
        //点数初期化
        CurrentlyLoginInfo.SCORE = 0;
        //テスト用
        FileSystem.SaveLoadManager save = new FileSystem.SaveLoadManager("save.txt");

        //旧 初期化
        //QuestionFuncs = new Question.QuestionManager(QuestionText, AnswerPrefab, AnswerText, "Lesson1");
        //新 初期化
        Question.QuestionManager.Create(ref QuestionFuncs, QuestionText, AnswerPrefab, AnswerText, "Lesson1");
        QuestionFuncs.UpdateQuestion(0);
    }

    void Update()
    {
        QuestionFuncs.Update();

        /*for test!*/
        if (Input.GetKeyDown(KeyCode.Keypad1))
            QuestionFuncs.UpdateQuestion(0);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            QuestionFuncs.UpdateQuestion(1);
        if (Input.GetKeyDown(KeyCode.Keypad3))
            QuestionFuncs.UpdateQuestion(2);
        if (Input.GetKeyDown(KeyCode.Keypad4))
            QuestionFuncs.UpdateQuestion(3);
        if (Input.GetKeyDown(KeyCode.Keypad5))
            QuestionFuncs.UpdateQuestion(4);
    }

    public void ClearAnswerButton()
    {
        QuestionFuncs.ClearAnswer();
    }
}
