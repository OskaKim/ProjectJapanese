using UnityEngine;
using System.Collections;

/*一時停止*/
public class Pause : MonoBehaviour {
    [SerializeField]
    GameObject PausePanel;

    bool pausing;

    void Update()
    {
        //アンドロイドのbackボタン
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausing ^= true;
            UpdatePause();
        }
    }

    void UpdatePause()
    {
        if (pausing)
        {
            //時間を止める
            Time.timeScale = 0;
            PausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            PausePanel.SetActive(false);
        }
    }
}
