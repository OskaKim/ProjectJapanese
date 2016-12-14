using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{

    [SerializeField]
    Transform TapText;
    [SerializeField]
    Transform ButtonsPanel;
    [SerializeField]
    Transform CreditPanel;
    [SerializeField]
    Text TimeInfoText;    
    
    //現在のシーケンス
    Sequence curSequence;

    void Start()
    {
        Time.timeScale = 1;
        curSequence = Sequence_Initialize;
        curSequence();

        //ファイルから読み込み
        FileSystem.SaveLoadManager loadMng = new FileSystem.SaveLoadManager();
        
        var prev = loadMng.GetPrevTime();
        /*時間表示*/
        string timeinfo_display = "前回：" + prev;
        timeinfo_display += "\n現在：" + System.DateTime.Now;
        TimeInfoText.text = timeinfo_display;
    }

    void Update()
    {
        curSequence();
    }

    delegate void Sequence();
    void Sequence_Initialize()
    {
        TapText.gameObject.SetActive(true);
        ButtonsPanel.gameObject.SetActive(false);
        curSequence = Sequence_Tap;
    }
    void Sequence_Tap()
    {
        if (Input.anyKeyDown)
        {
            curSequence = Sequence_SelectButton;

            TapText.gameObject.SetActive(false);
            ButtonsPanel.gameObject.SetActive(true);
        }
    }
    void Sequence_SelectButton()
    {

    }

    public void OpenCredit()
    {
        CreditPanel.gameObject.SetActive(true);
    }
    public void CloseCredit()
    {
        CreditPanel.gameObject.SetActive(false);
    }
}
