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
    public void LoadScene_Lesson1()
    {
        SceneManager.LoadScene("Game");
    }
    public void LoadScene_ScoreScene()
    {
        SceneManager.LoadScene("ScoreScene");
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}
