using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginScene : MonoBehaviour {

    [SerializeField]
    InputField AddUser_InputField;
    [SerializeField]
    Text AddUser_WarningMessage;
    [SerializeField]
    InputField Login_InputField;
    [SerializeField]
    Text Login_WarningMessage;
    [SerializeField]
    Button StartButton;

    FileSystem.SaveLoadManager mySaveLoadManager;

    void Start()
    {
        mySaveLoadManager = new FileSystem.SaveLoadManager("save.txt");
        bool bStartButton = CurrentlyLoginInfo.IsLogin() ? true : false;
        StartButton.gameObject.SetActive(bStartButton);
    }

    //ユーザー登録用のテキスト入力フィールドが変更された際
    public void Changed_AddUserInputField(string text)
    {
        const string WarningMessageForDisplay = "既に登録されたユーザーです。";

        AddUser_WarningMessage.text =
            mySaveLoadManager.isExistUser(text) ? WarningMessageForDisplay : null;
    }

    //ユーザー登録ボタン
    public void AddUserButton()
    {
        //入力された値
        string newID = AddUser_InputField.text;

        /*エラー出力*/
        if (mySaveLoadManager.isExistUser(newID))
            AddUser_WarningMessage.text = "既に登録されたユーザーです。";
        else
        {
            //ユーザー登録手続き
            int newScore = 0;
            mySaveLoadManager.AddUser(newID, newScore);
            AddUser_WarningMessage.text = "登録完了";
        }
    }

    //ログイン用の入力フィールドが変更されたら、警告メッセージを消す
    public void Changed_LoginInputField(string text)
    {
        Login_WarningMessage.text = null;
    }

    //ログインボタン
    public void LoginButton()
    {
        //入力された値
        string inputID = Login_InputField.text;

        if (!mySaveLoadManager.isExistUser(inputID))
        {
            Login_WarningMessage.text = "登録されていないユーザーです。";
            return;
        }
        CurrentlyLoginInfo.Login(inputID, 0);
        bool bStartButton = CurrentlyLoginInfo.IsLogin() ? true : false;
        StartButton.gameObject.SetActive(bStartButton);
    }

    //Startボタン押下
    public void PushStartButton()
    {
        SceneManager.LoadScene("Title");
    }
}