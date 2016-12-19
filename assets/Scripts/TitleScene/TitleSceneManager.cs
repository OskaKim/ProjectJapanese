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
    [SerializeField]
    Popup PointPopup;
    
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

        //少なくとも最後の接続から１日は超える場合
        if (loadMng.GetTimeGap_Days() > 1)
        {
            //popupStart
            PointPopup.PopupStart();
            //ポイント変更の情報表示
            PointPopup.transform.FindChild("Text").GetComponent<Text>().text = loadMng.UpdatePoint_BasedTime();
        }

        //現在の時間を設定
        loadMng.SetCurrentTime();
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
