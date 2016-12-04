using UnityEngine;
using System.Collections;

public class CharacterStatusManager : MonoBehaviour {

    [SerializeField]
    QuestionManagerObject Question_Mng;
    [SerializeField]
    UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl CharacterController;

    Question.SolveStatus status;

    void Update()
    {
        //ステータス取得
        GetStatus();
        SetController();
    }

    //ステータスを取得
    void GetStatus()
    {
        status = Question_Mng.status;
    }

    void SetController()
    {
        switch(status)
        {
            case Question.SolveStatus.Waiting:
                CharacterController.Waiting();
                break;
            case Question.SolveStatus.Solving:
                CharacterController.Running();
                break;
            case Question.SolveStatus.Correct:
                CharacterController.Correct();
                status = Question.SolveStatus.Waiting;
                break;
            case Question.SolveStatus.Incorrect:
                CharacterController.InCorrect();
                status = Question.SolveStatus.Waiting;
                break;
        }
    }
}
