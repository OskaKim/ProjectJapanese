using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{

    [SerializeField]
    Transform TapText;
    [SerializeField]
    Transform ButtonsPanel;
    [SerializeField]
    Transform CreditPanel;
    
    //現在のシーケンス
    Sequence curSequence;

    void Start()
    {
        Time.timeScale = 1;
        curSequence = Sequence_Initialize;
        curSequence();
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
