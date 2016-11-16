using UnityEngine;
using System.Collections;

public class CheckAnswerButton : MonoBehaviour {

    //問題の正誤有無をチェック
    public void GoCheckToAnswer()
    {
        Question.QuestionManager.bCheckAnswer = true;
    }
}
