using UnityEngine;
using UnityEngine.SceneManagement;

//簡単なボタンの関数集合
public class Buttons : MonoBehaviour {

    public void LoadScene_Title()
    {
        SceneManager.LoadScene("Title");
    }
    public void LoadScene_SelectLevel()
    {
        SceneManager.LoadScene("SelectLevel");
    }
    public void LoadScene_Game()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }
    public void LoadScene_ScoreScene()
    {
        SceneManager.LoadScene("ScoreScene");
    }
    public void ExitButton()
    {
        Application.Quit();
    }
    public void SetDataToDefault_ExceptDate()
    {
        //ファイルから読み込み
        FileSystem.SaveLoadManager loadMng = new FileSystem.SaveLoadManager();
        loadMng.AddUser();
        System.DateTime date = new System.DateTime(2016, 12, 10, 1, 1, 1);
        loadMng.SetTime(date);
    }
}