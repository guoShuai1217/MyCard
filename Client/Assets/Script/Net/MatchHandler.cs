using ClientNetFrame;
using UnityEngine;
using GameProtocol;
using GameProtocol.model.match;

public class MatchHandler : MonoBehaviour, IHandler
{
    public void MessageReceive(SocketModel model)
    {
        switch (model.command)
        {
            case MatchProtocol.LEAVEMATCH_SRES:
                {
                    switch (model.GetMessage<int>()) {
                        case 0:
                        case -2:
                            GameApp.Instance.GameLevelManagerScript.CloseSystemUI(GameResource.SystemUIType.MATCHPANEL);
                            break;
                        case -1:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("游戏即将开始");
                            break;
                    }
                }
                break;
            case MatchProtocol.MATCHCLOSE_BRQ:
                {
                    GameApp.Instance.TimeManagerScript.AddSchedule(delegate ()
                    {
                        GameApp.Instance.CommonHintDlgScript.OpenHintBox("游戏将于五秒后自动返回大厅");
                        this.Write(TypeProtocol.USER, UserProtocol.GETINFO_CREQ, null);
                    },5000);
                }
                break;
            case MatchProtocol.MATCHFINISH_BRQ:
                {
                    //TODO:LOAD BATTLE
                    GameApp.Instance.GameLevelManagerScript.LoadScene(GameResource.SceneName.BATTLE);
                    GameApp.Instance.GameLevelManagerScript.CloseSystemUI(GameResource.SystemUIType.MATCHPANEL);
                }
                break;
            case MatchProtocol.MATCHINFO_BRQ:
                {
                    //刷新接收到的匹配信息
                    MatchInfoModel m = model.GetMessage<MatchInfoModel>();
                    if (GameApp.Instance.UI_MatchScript != null)
                        GameApp.Instance.UI_MatchScript.UpdateRoomRoleInfo(m);
                }
                break;
            case MatchProtocol.STARTMATCH_SRES:
                {
                    //接收开始匹配结果
                    ResponseStartMatchInfo m = model.GetMessage<ResponseStartMatchInfo>();
                    switch (m.Status)
                    {
                        //匹配成功
                        case 0:
                            //加载匹配页面
                            GameApp.Instance.GameLevelManagerScript.LoadSystemUI(GameResource.SystemUIType.MATCHPANEL,delegate() {
                                GameObject go;
                                if (GameApp.Instance.GameLevelManagerScript.SystemUICache.TryGetValue(GameResource.SystemUIType.MATCHPANEL,out go))
                                {
                                    if(!go.GetComponent<UI_Match>())
                                        go.AddComponent<UI_Match>();
                                    //刷新匹配界面信息
                                    go.GetComponent<UI_Match>().StartMatch(m);
                                }
                            });
                            break;
                        case -1:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("当前余额不足");
                            break;
                        case -2:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("已在其他房间中，不可进行匹配");
                            break;
                    }
                }break;

        }
    }
}

