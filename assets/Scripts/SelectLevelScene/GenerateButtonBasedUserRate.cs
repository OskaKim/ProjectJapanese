using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenerateButtonBasedUserRate : MonoBehaviour
{
    [SerializeField]
    Button buttonPrefab;

    void Start()
    {
        FileSystem.SaveLoadManager SaveLoadMng = new FileSystem.SaveLoadManager();
        //現在挑戦できる最大レベル
        var curMaxLevel = SaveLoadMng.GetRateByID(CurrentlyLoginInfo.ID);
        var myCanvas = FindObjectOfType<Canvas>();

        for (int i = 0; i < curMaxLevel; ++i)
        {
            //ボタンの位置などの設定
            var button = Instantiate(buttonPrefab) as Button;
            button.transform.SetParent(myCanvas.transform);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, - i * 50);

            //各ボタンのレベルを指定
            button.GetComponent<SelectLevelButton>().level = i + 1;

            //ボタンに表示するテキストの設定
            var buttontext = button.transform.FindChild("Text").GetComponent<Text>();
            buttontext.text = "Level" + (i+1);
        }
    }
}
