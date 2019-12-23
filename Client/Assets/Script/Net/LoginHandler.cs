using UnityEngine;
using System.Collections;
using ClientNetFrame;
using GameProtocol;
using GameProtocol.model.login;
using System;

/// <summary>
/// 处理服务器二级协议消息分发处理
/// </summary>
public class LoginHandler : MonoBehaviour, IHandler
{
    public void MessageReceive(SocketModel model)
    {
        switch (model.command)
        {
            //处理服务器返回的登录结果
            case AccountProtocol.ENTER_SRES:
                {
                    int Status = model.GetMessage<int>();
                    switch (Status)
                    {
                        case 0:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("登录成功");
                            Debug.Log("登录成功");
                            LoginReceive();
                            break;
                        case -1:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("请求错误");
                            Debug.Log("请求错误");
                            break;
                        case -2:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("账号密码不合法");
                            Debug.Log("账号密码不合法");
                            break;
                        case -3:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("没有此账号");
                            Debug.Log("没有此账号");
                            break;
                        case -4:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("密码错误");
                            Debug.Log("密码错误");
                            break;
                        case -5:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("账号已登录");
                            Debug.Log("账号已登录");
                            break;
                    }
                }break;
            //处理服务器返回的注册结果
            case AccountProtocol.QUICKREG_SRES:
                {
                    //接收注册结果
                    ResponseRegisterModel rrm = model.GetMessage<ResponseRegisterModel>();
                    if (rrm == null || rrm.Status != 0)
                    {
                        GameApp.Instance.CommonHintDlgScript.OpenHintBox("快速注册登录失败");
                        Debug.Log("注册失败");
                        return;
                    }
                    GameApp.Instance.CommonHintDlgScript.OpenHintBox("成功快速注册登录");
                    GameApp.Instance.CommonHintDlgScript.OpenHintBox("成功快速注册登录");
                    GameApp.Instance.CommonHintDlgScript.OpenHintBox("注册密码为" + rrm.Password);
                    Debug.Log("注册成功,注册密码为:" + rrm.Password);
                    LoginReceive();
                }
                break;
        }
    }

    private void LoginReceive()
    {
        this.Write(TypeProtocol.USER, UserProtocol.GETINFO_CREQ, null);
    }
}
