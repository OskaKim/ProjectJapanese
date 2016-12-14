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

    //表示する値
    float myScore_DisplayValue;
    float myHighScore_DisplayValue;
    float myScore_RealValue;
    float myHighScore_RealValue;

    //カウントするエフェクトを表すためのタイマー
    float countTimer = 0;
    const float countTick = 0.05f;

    void Start()
    {
        FileSystem.SaveLoadManager save = new FileSystem.SaveLoadManager();
        myHighScore_RealValue = save.GetScore(CurrentlyUserInfo.selectedLevel);
        myScore_RealValue = CurrentlyUserInfo.score * 10;

        //HighScore更新
        if (myHighScore_RealValue < myScore_RealValue)
        {
            myHighScore_RealValue = myScore_RealValue;
            newScoreDisplayText.enabled = true;
        }

        //ファイル更新
        save.SetScore((int)myHighScore_RealValue, CurrentlyUserInfo.selectedLevel);
        
        //現在のユーザー情報を初期化
        CurrentlyUserInfo.DeleteAll();

        //表示するためのvalue
        myScore_DisplayValue = 0;
        myHighScore_DisplayValue = 0;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        //点数表示
        myScore.text = myScore_DisplayValue.ToString();
        myHighScore.text = myHighScore_DisplayValue.ToString();
    }

    void Update()
    {
        CountEffect();
    }

    void CountEffect()
    {
        countTimer += Time.deltaTime;
        if (countTimer >= countTick)
        {
            countTimer = 0;
            if(myScore_DisplayValue < myScore_RealValue)
            {
                myScore_DisplayValue++;
            }
            if(myHighScore_DisplayValue < myHighScore_RealValue)
            {
                myHighScore_DisplayValue++;
            }
        }
        UpdateScoreText();
    }
}
