using UnityEngine;
using System.Collections;
using GameProtocol;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using GameProtocol.model.fight;

public class UI_Fight : MonoBehaviour {
    /// <summary>
    /// 设置按钮
    /// </summary>
    Button OptionsBtn;
    /// <summary>
    /// 时间标签
    /// </summary>
    Text TimeText;
    /// <summary>
    /// 电量
    /// </summary>
    Image ElectricSlider;
    /// <summary>
    /// 时间刷新计时器ID
    /// </summary>
    protected int UpdateTimeId = -1;
    /// <summary>
    /// 玩家ID和对战信息的队伍信息集
    /// </summary>
    Dictionary<int, FightUserModel> TeamInfo = new Dictionary<int, FightUserModel>();
    /// <summary>
    /// 游戏层
    /// </summary>
    protected GameObject GameInfoPanel;
    private void Awake()
    {
        GameApp.Instance.UI_FightScript = this;
        StartRoom();
    }
    private void OnDestroy()
    {
        if (UpdateTimeId != -1)
            GameApp.Instance.TimeManagerScript.Remove(UpdateTimeId);
    }

    private object fightui { get { return GameApp.Instance.UI_FightScript; } }
    public T GetUIFight<T>() {
        return (T)fightui;
    }
    /// <summary>
    /// 房间初始化
    /// </summary>
    protected void StartRoom()
    {
        //通过组件获取获取父组件和父组件的子组件来获取设置按钮
        OptionsBtn = transform.parent.Find("roominfo/options").GetComponent<Button>();
        TimeText = transform.parent.Find("roominfo/time").GetComponent<Text>();
        ElectricSlider = transform.parent.Find("roominfo/ElectricSlider/child").GetComponent<Image>();
        //为设置按钮添加事件
        OptionsBtn.onClick.AddListener(delegate () {
            GameApp.Instance.GameLevelManagerScript.LoadSystemUI(GameResource.SystemUIType.UIOPTIONSPANEL);
        });
        //执行完毕,告诉服务器准备完成
        this.Write(TypeProtocol.FIGHT, FightProtocol.ENTERFIGHT_CREQ, null);
        UpdateTime();
    }

    void UpdateTime() {
        TimeText.text = DateTime.Now.ToString("hh:mm");
        UpdateTimeId = GameApp.Instance.TimeManagerScript.AddSchedule(delegate ()
        {
            UpdateTime();
        }, 60 * 1000);
    }
    /// <summary>
    /// ID缓存
    /// </summary>
    List<int> cacheId = new List<int>();
    /// <summary>
    /// 刷新队伍成员信息
    /// </summary>
    /// <param name="model"></param>
    public void UpdateTeam(FightUserModel model) {
        if (TeamInfo.ContainsKey(model.id))
        {
            if (TeamInfo.ContainsKey(GameSession.Instance.UserInfo.id))
            {
                //TODO:刷新玩家信息
                GameApp.Instance.UI_HeadScript.UpdateItme(model, TeamInfo[GameSession.Instance.UserInfo.id].Direction);
            }
            return;
        }
        switch (GameSession.Instance.RoomType)
        {
            //如果当前游戏类型是赢三张的话，则直接刷新赢三张手牌脚本
            case SConst.GameType.WINTHREEPOKER:
                GameApp.Instance.CardOtherScript.GetCardOther<TPCardOther>().UpdateData(model);
                break;
        }
        //添加队伍成员
        TeamInfo.Add(model.id, model);
        //将队伍成员的ID进行缓存，直到收到玩家自己的信息
        if (!TeamInfo.ContainsKey(GameSession.Instance.UserInfo.id))
        {
            cacheId.Add(model.id);
            return;
        }
        int userdir = TeamInfo[GameSession.Instance.UserInfo.id].Direction;
        for (int i = 0;i<cacheId.Count;i++)
        {
            GameApp.Instance.UI_HeadScript.UpdateItme(TeamInfo[cacheId[i]], userdir);
        }
        cacheId.Clear();
        GameApp.Instance.UI_HeadScript.UpdateItme(model, userdir);
        if (TeamInfo.Count >= GameApp.Instance.GetPlayCount())
        {
            //TODO:游戏即将开始
        }
    }

    /// <summary>
    /// 添加游戏UI层
    /// </summary>
    /// <param name="go"></param>
    public void SetGameInfoPanel(GameObject go)
    {
        GameInfoPanel = go;
    }
}
