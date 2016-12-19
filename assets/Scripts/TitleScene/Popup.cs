using UnityEngine;

public class Popup : MonoBehaviour {

    [SerializeField]
    float speed = 3.0f;
    [SerializeField]
    bool startAuto = true;

    bool popupStart_Enabled = false;
    bool popupEnd_Enabled = false;

    void Start()
    {
        //初期化
        var startScale = transform.GetComponent<RectTransform>().localScale;
        startScale.x = 0.0f;
        startScale.y = 0.0f;
        transform.GetComponent<RectTransform>().localScale = startScale;
        
        //自動設定
        if (startAuto)
            PopupStart();
    }
    void Update()
    {
        //popup startのアニメーション
        if(popupStart_Enabled)
        {
            var scale = transform.GetComponent<RectTransform>().localScale;
            float rSpeed = speed * Time.deltaTime;
            scale.x += rSpeed;
            scale.y += rSpeed;

            //終了
            if(scale.x >= 1.0f || scale.y >= 1.0f)
            {
                scale.x = 1.0f;
                scale.y = 1.0f;
                popupStart_Enabled = false;
            }
            transform.GetComponent<RectTransform>().localScale = scale;
        }
        //popup endのアニメーション
        else if(popupEnd_Enabled)
        {
            var scale = transform.GetComponent<RectTransform>().localScale;
            float rSpeed = speed * Time.deltaTime;
            scale.x -= rSpeed;
            scale.y -= rSpeed;

            //終了
            if (scale.x <= 0.0f || scale.y <= 0.0f)
            {
                scale.x = 0.0f;
                scale.y = 0.0f;
                popupEnd_Enabled = false;
            }
            transform.GetComponent<RectTransform>().localScale = scale;
        }
    }

    //初期状態に戻す
    public void PopupStart()
    {
        var startScale = transform.GetComponent<RectTransform>().localScale;
        startScale.x = 0.0f;
        startScale.y = 0.0f;
        transform.GetComponent<RectTransform>().localScale = startScale;
        popupStart_Enabled = true;
    }
    public void PopupEnd()
    {
        var startScale = transform.GetComponent<RectTransform>().localScale;
        startScale.x = 1.0f;
        startScale.y = 1.0f;
        transform.GetComponent<RectTransform>().localScale = startScale;
        popupEnd_Enabled = true;
    }
}