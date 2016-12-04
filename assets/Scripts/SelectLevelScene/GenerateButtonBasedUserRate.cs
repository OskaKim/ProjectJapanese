using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenerateButtonBasedUserRate : MonoBehaviour
{
    [SerializeField]
    Button buttonPrefab;
    [SerializeField]
    Animator Panel;

    void Start()
    {
        //現在のユーザー情報を初期化
        CurrentlyUserInfo.DeleteAll();
        
        FileSystem.SaveLoadManager SaveLoadMng = new FileSystem.SaveLoadManager();
        //現在挑戦できる最大レベル
        var curMaxLevel = SaveLoadMng.GetRate();
        var myCanvas = FindObjectOfType<Canvas>();

        Vector2 offset = new Vector2(0, 100);

        for (int i = 0; i < curMaxLevel; ++i)
        {
            //ボタンの位置などの設定
            var button = Instantiate(buttonPrefab) as Button;
            button.transform.SetParent(myCanvas.transform.GetChild(0));
            button.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            button.GetComponent<RectTransform>().anchoredPosition = offset + new Vector2(0, - i * button.GetComponent<RectTransform>().sizeDelta.y);

            //各ボタンのレベルを指定
            button.GetComponent<SelectLevelButton>().level = i + 1;

            //ボタンに表示するテキストの設定
            var buttontext = button.transform.FindChild("Text").GetComponent<Text>();
            buttontext.text = "Level" + (i + 1) +
                ": " + SaveLoadMng.GetScore(i + 1) + "点";            
        }
    }
    public void ButtonClick(int level)
    {
        if (Panel.GetBool("bSelectEnd")) return;

        CurrentlyUserInfo.selectedLevel = level;
        Panel.SetBool("bSelectEnd", true);
        //SceneManager.LoadScene("Game");
    }
}