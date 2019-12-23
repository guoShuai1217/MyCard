using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameProtocol;
using GameProtocol.model.login;

public class UI_Login : MonoBehaviour {
    /// <summary>
    /// 快速登录
    /// </summary>
    Button QuickBtn;
    /// <summary>
    /// 账号登录
    /// </summary>
    Button WechatBtn;
    /// <summary>
    /// 账号输入框
    /// </summary>
    InputField UsernameIF;
	void Awake () {
        GameApp.Instance.UI_LoginScript = this;
        QuickBtn = transform.Find("Panel/QuickButton").GetComponent<Button>();
        WechatBtn = transform.Find("Panel/WechatButton").GetComponent<Button>();
        UsernameIF = transform.Find("Panel/username").GetComponent<InputField>();
        OnClick();
    }

    void OnClick() {
        //为快速登录添加回调事件
        QuickBtn.onClick.AddListener(delegate () {
            //向服务器发送请求快速注册
            ExtendHandler.SendMessage(TypeProtocol.ACCOUNT, AccountProtocol.QUICKREG_CREQ, null);
            //this.Write(TypeProtocol.LOGIN, LoginProtocol.QUICKREG_CREQ, null);
            //GameApp.Instance.NetMessageUtilScript.NetIO.write(TypeProtocol.LOGIN, LoginProtocol.QUICKREG_CREQ, null);
            Debug.Log("请求快速注册登录");
            GameApp.Instance.CommonHintDlgScript.OpenHintBox("请求快速注册登录");
            GameApp.Instance.CommonHintDlgScript.OpenHintBox("请求快速注册登录");
            GameApp.Instance.CommonHintDlgScript.OpenHintBox("请求快速注册登录");
            GameApp.Instance.CommonHintDlgScript.OpenHintBox("请求快速注册登录");
            GameApp.Instance.CommonHintDlgScript.OpenHintBox("请求快速注册登录");
            GameApp.Instance.CommonHintDlgScript.OpenHintBox("请求快速注册登录");
        });
        //为账号登录添加回调事件
        WechatBtn.onClick.AddListener(delegate () {
            string user = UsernameIF.text;
            if (user.Length < 6) return;
            //创建一个账号登录对象
            RequestLoginModel rlm = new RequestLoginModel();
            rlm.Ditch = 0;
            rlm.Username = user;
            rlm.Password = "password";
            this.Write(TypeProtocol.ACCOUNT, AccountProtocol.ENTER_CREQ, rlm);
            Debug.Log("请求账号登录");
        });
    }




	
}
