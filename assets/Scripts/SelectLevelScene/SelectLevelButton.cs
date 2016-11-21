using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectLevelButton : MonoBehaviour {
    public int level { get; set; }
    public void Click()
    {
        CurrentlyLoginInfo.LEVEL = level;
        SceneManager.LoadScene("Game");
    }
}
