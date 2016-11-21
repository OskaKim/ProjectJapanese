using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleSceneButtons : MonoBehaviour {
    [SerializeField]
    Animator TitleScrollAnimator;
    float timer = 0.0f;

    public void pushStartButton()
    {
        TitleScrollAnimator.enabled = true;
        timer = 1.0f;
    }

    void Update()
    {
        if(timer > 0.0f)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                SceneManager.LoadSceneAsync("Game");
            }
        }
    }
}