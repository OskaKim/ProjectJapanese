using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    [SerializeField]
    Text myScore;
    [SerializeField]
    Text myHighScore;
    [SerializeField]
    Text newScoreDisplayText;

    void Start()
    {
        FileSystem.SaveLoadManager save = new FileSystem.SaveLoadManager();
        int highscore, score;
        highscore = save.GetScore(CurrentlyUserInfo.selectedLevel);
        score = CurrentlyUserInfo.score;

        //HighScore更新
        if (highscore < score)
        {
            highscore = score;
            newScoreDisplayText.enabled = true;
        }

        //点数表示
        myScore.text = score.ToString();
        myHighScore.text = highscore.ToString();

        //ファイル更新
        save.SetScore(highscore, CurrentlyUserInfo.selectedLevel);
        
        //現在のユーザー情報を初期化
        CurrentlyUserInfo.DeleteAll();
    }
}
