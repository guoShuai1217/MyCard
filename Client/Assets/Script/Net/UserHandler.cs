using UnityEngine;
using System.Collections;
using ClientNetFrame;
using System;
using GameProtocol;
using GameProtocol.model.login;

public class UserHandler : MonoBehaviour, IHandler
{
    public void MessageReceive(SocketModel model)
    {
        switch (model.command)
        {
            case UserProtocol.GETINFO_SRES:
                { 
                    UserModel um = model.GetMessage<UserModel>();
                    if (um != null)
                    {
                        GameApp.Instance.CommonHintDlgScript.OpenHintBox("获取用户信息成功" + um.nickname);
                        GameSession.Instance.UserInfo = um;
                        GameApp.Instance.GameLevelManagerScript.LoadScene(GameResource.SceneName.MAIN);
                    }
                    else
                    {
                        GameApp.Instance.CommonHintDlgScript.OpenHintBox("获取用户信息失败");
                        ExtendHandler.Close();
                        ExtendHandler.Connect();
                    }
                }break;
        }
    }
}
