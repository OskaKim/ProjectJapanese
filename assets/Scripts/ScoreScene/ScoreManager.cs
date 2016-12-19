using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    [SerializeField]
    Text myScore;
    [SerializeField]
    Text myBossScore;
    [SerializeField]
    Text myHighScore;
    [SerializeField]
    Text myBossHighScore;
    [SerializeField]
    Text newScoreDisplayText;

    //表示する値
    int myScore_DisplayValue;
    int myBossScore_DisplayValue;

    int myHighScore_DisplayValue;
    int myBossHighScore_DisplayValue;

    int myScore_RealValue;
    int myBossScore_RealValue;

    int myHighScore_RealValue;
    int myBossHighScore_RealValue;
    int questionLength;
    int bossquestionLength;

    //カウントするエフェクトを表すためのタイマー
    float countTimer = 0;
    const float countTick = 0.05f;

    bool bBoss = false;

    void Start()
    {
        FileSystem.SaveLoadManager save = new FileSystem.SaveLoadManager();
        save.GetScore(CurrentlyUserInfo.selectedLevel, ref myHighScore_RealValue, ref myBossHighScore_RealValue);
        myScore_RealValue = CurrentlyUserInfo.score;
        if (CurrentlyUserInfo.bBoss)
        {
            myBossScore_RealValue = CurrentlyUserInfo.bossScore;
            bBoss = true;
        }
        //問題の個数
        FileSystem.QuestionFileManager questionMng = new FileSystem.QuestionFileManager("Lesson" + CurrentlyUserInfo.selectedLevel);
        questionLength = questionMng.GetNumOfQuestions();
        bossquestionLength = questionMng.GetNumOfQuestionsofBoss();

        //HighScore更新
        if (myHighScore_RealValue < myScore_RealValue)
        {
            myHighScore_RealValue = myScore_RealValue;
            newScoreDisplayText.enabled = true;
        }
        //boss highscore
        if(myBossHighScore_RealValue < myBossScore_RealValue)
        {
            myBossHighScore_RealValue = myBossScore_RealValue;
            newScoreDisplayText.enabled = true;
        }

        //ファイル更新
        save.SetScore(CurrentlyUserInfo.selectedLevel, (int)myHighScore_RealValue, (int)myBossHighScore_RealValue);
        
        //現在のユーザー情報を初期化
        CurrentlyUserInfo.DeleteAll();

        //表示するためのvalue
        myScore_DisplayValue = 0;
        myBossScore_DisplayValue = 0;
        myHighScore_DisplayValue = 0;
        myBossHighScore_DisplayValue = 0;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        //点数表示
        myScore.text = questionLength + "問から" + myScore_DisplayValue.ToString() + "正解達成\n";
        myHighScore.text = "最大記録：" + myHighScore_DisplayValue.ToString();
        if (bBoss)
        {
            myBossScore.text = bossquestionLength + "問から" + myBossScore_DisplayValue.ToString() + "正解達成\n";
            myBossHighScore.text = "ボス戦最大記録：" + myBossHighScore_DisplayValue.ToString();
        }
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
            if(bBoss && myBossScore_DisplayValue < myBossScore_RealValue)
            {
                myBossScore_DisplayValue++;
            }
            if(myHighScore_DisplayValue < myHighScore_RealValue)
            {
                myHighScore_DisplayValue++;
            }
            if(bBoss && myBossHighScore_DisplayValue < myBossHighScore_RealValue)
            {
                myBossHighScore_DisplayValue++;
            }
        }
        UpdateScoreText();
    }
}
