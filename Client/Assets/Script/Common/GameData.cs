using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 程序运行时的本地数据缓存单例
/// </summary>
public class GameData {
    private static GameData instance;
    public static GameData Instance { get { if (instance == null) instance = new GameData(); return instance; } }
    /// <summary>
    /// 是否打印日志到本地
    /// </summary>
    public bool IsDebugWrite = true;
    /// <summary>
    /// 游戏当前场景
    /// </summary>
    public string GameLevelName = "Start";
    /// <summary>
    /// 预制体资源加载缓存
    /// </summary>
    public Dictionary<string, GameObject> LoadGameObjectCache = new Dictionary<string, GameObject>();
    /// <summary>
    /// 用来存储场景名称
    /// </summary>
    public Dictionary<GameResource.SceneName, string> SceneName = new Dictionary<GameResource.SceneName, string>();
    /// <summary>
    /// 用来存储UICanvas
    /// </summary>
    public Dictionary<GameResource.CanvasTag, string> CanvasName = new Dictionary<GameResource.CanvasTag, string>();
    /// <summary>
    /// 用来存储系统功能UI
    /// </summary>
    public Dictionary<GameResource.SystemUIType, string> SystemUI = new Dictionary<GameResource.SystemUIType, string>();
    /// <summary>
    /// 用来存储组件名称
    /// </summary>
    public Dictionary<GameResource.ItemTag, string> ItemName = new Dictionary<GameResource.ItemTag, string>();
    /// <summary>
    /// 音乐资源
    /// </summary>
    public Dictionary<GameResource.MusicTag, string> MusicTag = new Dictionary<GameResource.MusicTag, string>();
}
