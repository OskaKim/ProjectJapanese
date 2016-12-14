using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*ボタンをクリックするとselectSEを再生する。*/
[RequireComponent(typeof(Button))]
public class ButtonSE : MonoBehaviour
{
    [SerializeField]
    SEManager.SEs selectSE = SEManager.SEs.None;
    SEManager SEManagerInst;

    void Start()
    {
        //SEが設定されてないなら、このscriptをdetach
        if(selectSE == SEManager.SEs.None)
        {
            Destroy(this);
            return;
        }
        
        //SEsを探し、格納する
        var temp = GameObject.Find("SEs");
        if (!temp)
        {
            temp = Instantiate(Resources.Load("Prefabs/Ses") as GameObject) as GameObject;
            temp.name = "SEs";
        }
        SEManagerInst = temp.GetComponent<SEManager>();

        //ボタンクリックエフェクトに追加
        GetComponent<Button>().onClick.AddListener(ButtonClicked);
    }

    //ボタンクリック
    public void ButtonClicked()
    {
        SEManagerInst.Play(selectSE);
    }
}