using UnityEngine;
using System.Collections;

/*現在のユーザーの情報を取得*/
public struct CurrentlyUserInfo
{
    /*ユーザー情報を全部消す*/
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    /*現在選択済みのレートを取得*/
    public static int selectedLevel
    {
        get
        {
            return PlayerPrefs.GetInt("LEVEL");
        }
        set
        {
            PlayerPrefs.SetInt("LEVEL", value);
        }
    }
    /*現在のスコアを取得*/
    public static int score
    {
        get
        {
            return PlayerPrefs.GetInt("SCORE");
        }
        set
        {
            PlayerPrefs.SetInt("SCORE", value);
        }
    }
}