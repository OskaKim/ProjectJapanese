using UnityEngine;
using System.Collections;

public class AnswerButton : MonoBehaviour {
    
    //答えを現在のボタンに設定する
    public void SetAnswer()
    {
        //現在選択した答え
        char curAns = gameObject.name[0];

        /*重複に選択されているかチェック*/
        if (Question.QuestionManager.selectedAns != null)
        foreach (var Ans in Question.QuestionManager.selectedAns)
        {
            //答えが選択済み
            if(curAns == Ans)
            {
                Debug.Log("選択済みの答えです");
                return;
            }
        }

        /*タイプ*/
        switch (Question.QuestionManager.curType)
        {
            case 1:
                //答えに追加
                Question.QuestionManager.selectedAns += curAns;
                break;
            case 2:
            case 3:
                //答えに初期化
                Question.QuestionManager.selectedAns = curAns.ToString();
                break;
            default:
                Debug.Log("問題のタイプ設定が正しくありません。");
                break;
        }
    }
}
