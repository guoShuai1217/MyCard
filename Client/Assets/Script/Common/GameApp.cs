using System;
using System.Collections.Generic;
/// <summary>
/// 存储客户端单一实例的单例,达到访问其他非静态脚本的目的
/// </summary>
public class GameApp{
    //单例模式
    private static GameApp instance;
    public static GameApp Instance {
        get {
            if (instance == null)
                instance = new GameApp();
            return instance;
        }
    }
    /// <summary>
    /// 日志管理器
    /// </summary>
    public DebugManager DebugManagerScript;
    /// <summary>
    /// 计时器管理类
    /// </summary>
    public TimeManager TimeManagerScript;
    /// <summary>
    /// 资源管理器
    /// </summary>
    public ResourcesManager ResourcesManagerScript;
    /// <summary>
    /// 音乐管理器
    /// </summary>
    public MusicManger MusicMangerScript;
    /// <summary>
    /// 加载场景管理器
    /// </summary>
    public LoadManager LoadMangerScript;
    /// <summary>
    /// 游戏场景管理器
    /// </summary>
    public GameLevelManager GameLevelManagerScript;
    /// <summary>
    /// 网络消息分发中心
    /// </summary>
    public NetMessageUtil NetMessageUtilScript;
    /// <summary>
    /// 登录脚本
    /// </summary>
    public UI_Login UI_LoginScript;
    /// <summary>
    /// 通用提示框
    /// </summary>
    public CommonHintDlg CommonHintDlgScript;
    /// <summary>
    /// 主场景UI
    /// </summary>
    public UI_Main UI_MainScript;
    /// <summary>
    /// 设置
    /// </summary>
    public UI_Options UI_OptionsScript;
    /// <summary>
    /// 匹配
    /// </summary>
    public UI_Match UI_MatchScript;
    /// <summary>
    /// 游戏常量
    /// </summary>
    public GameConst GameConst = new GameConst();
    /// <summary>
    /// 游戏总管理脚本
    /// </summary>
    public GameOther GameOtherScript;
    /// <summary>
    /// 游戏UI管理
    /// </summary>
    public UI_Fight UI_FightScript;
    /// <summary>
    /// 游戏手牌管理
    /// </summary>
    public CardOther CardOtherScript;
    /// <summary>
    /// 玩家战斗时头像组件
    /// </summary>
    public UI_Head UI_HeadScript;
    /// <summary>
    /// 获取玩家人数
    /// </summary>
    /// <returns></returns>
    public int GetPlayCount() {
        switch (GameSession.Instance.RoomType)
        {
            case GameProtocol.SConst.GameType.WINTHREEPOKER:
                return 2;
            default:
                return 4;
        }
    }
}
