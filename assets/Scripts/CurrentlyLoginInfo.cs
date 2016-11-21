using UnityEngine;
using System.Collections;

/*現在のログイン情報取得用*/
public class CurrentlyLoginInfo : MonoBehaviour {

    //ID
    static string m_id = "default";
    public static string ID
    {
        get { return m_id; }
    }
    
    public static int SCORE = 0;
    public static int HIGHSCORE = 0;
    public static int LEVEL = 1;
    public static int HIGHLEVEL = 1;

    //ユーザ設定
    public static void Login(string id, int score)
    {
        m_id = id;
        SCORE = score;
    }

    //ログアウト
    public static void Logout()
    {
        m_id = null;
        SCORE = 0;
    }

    //ログインの有無
    public static bool IsLogin()
    {
        if (m_id == null)
            return false;
        return true;
    }
}