using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {
    [SerializeField]
    Text myScore;
    [SerializeField]
    Text myHighScore;
    void Start()
    {
        //HighScore更新
        if (CurrentlyLoginInfo.HIGHSCORE < CurrentlyLoginInfo.SCORE)
            CurrentlyLoginInfo.HIGHSCORE = CurrentlyLoginInfo.SCORE;

        myScore.text = CurrentlyLoginInfo.SCORE.ToString();
        myHighScore.text = CurrentlyLoginInfo.HIGHSCORE.ToString();

        //点数をファイルにセーブ
        FileSystem.SaveLoadManager save = new FileSystem.SaveLoadManager("save.txt");
        save.SetScoreBYID(CurrentlyLoginInfo.ID, CurrentlyLoginInfo.HIGHSCORE);
    }
}
