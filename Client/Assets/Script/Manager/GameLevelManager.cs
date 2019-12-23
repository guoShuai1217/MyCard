using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model;

public class GameLevelManager : MonoBehaviour {
    /// <summary>
    /// 系统UI缓存
    /// </summary>
    public Dictionary<GameResource.SystemUIType, GameObject> SystemUICache = new Dictionary<GameResource.SystemUIType, GameObject>();
    /// <summary>
    /// 用来存储当前显示的系统的队列
    /// 当前显示最靠前的系统一定在队列的最后一项
    /// </summary>
    List<GameResource.SystemUIType> SystemList = new List<GameResource.SystemUIType>();

    void Awake () {
        GameApp.Instance.GameLevelManagerScript = this;
        GameResource.Instance.Register();
    }

    void Start()
    {
        //告知Unity不要将GameCanvas删除掉
        DontDestroyOnLoad(transform.parent);
        //设置分辨率
        Screen.SetResolution(1280, 720, false);
        //在游戏开始3秒  加载进去LOGO场景
        GameApp.Instance.TimeManagerScript.AddSchedule(delegate () {
            LoadScene(GameResource.SceneName.LOGO);
        }, 3000);
    }
    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="tag"></param>
    public void LoadScene(GameResource.SceneName tag)
    {
        //如果当前场景名称和要加载的场景名称一致，则直接返回
        if (GameData.Instance.GameLevelName == GameData.Instance.SceneName[tag])
            return;
        //要加载的资源
        List<LoadResourceModel> rModel = new List<LoadResourceModel>();
        //加载完成后的回调
        LoadManager.CallBack call = null;
        switch (tag)
        {
            case GameResource.SceneName.LOGO:
                {
                    LoadResourceModel rM = new LoadResourceModel();
                    rM.type = GameResource.ResourceType.PREFAB;
                    rM.path = GameResource.UIResourcePath +
                        GameData.Instance.CanvasName[GameResource.CanvasTag.CANVASLOGO];
                    rModel.Add(rM);
                    call = delegate ()
                    {
                        //场景是空白场景，UI或其他资源为场景加载后进行实例化生成，添加到本场景
                        GameObject go = GameApp.Instance.ResourcesManagerScript.LoadInstantiateGameObject(rM.path, null, Vector3.zero);
                        //UI和代码同样也是分离的，方便的代码热更新
                        go.AddComponent<UI_Logo>();
                    };
                } break;
            case GameResource.SceneName.LOGIN:
                {
                    LoadResourceModel rM = new LoadResourceModel();
                    rM.type = GameResource.ResourceType.PREFAB;
                    rM.path = GameResource.UIResourcePath +
                        GameData.Instance.CanvasName[GameResource.CanvasTag.CANVASLOGIN];
                    rModel.Add(rM);
                    call = delegate ()
                    {
                        //场景是空白场景，UI或其他资源为场景加载后进行实例化生成，添加到本场景
                        GameObject go = GameApp.Instance.ResourcesManagerScript.LoadInstantiateGameObject(rM.path, null, Vector3.zero);
                        //UI和代码同样也是分离的，方便的代码热更新
                        go.AddComponent<UI_Login>();
                    };
                }
                break;
            case GameResource.SceneName.MAIN:
                {
                    LoadResourceModel rM = new LoadResourceModel();
                    rM.type = GameResource.ResourceType.PREFAB;
                    rM.path = GameResource.UIResourcePath +
                        GameData.Instance.CanvasName[GameResource.CanvasTag.CANVASMAIN];
                    rModel.Add(rM);
                    call = delegate ()
                    {
                        //场景是空白场景，UI或其他资源为场景加载后进行实例化生成，添加到本场景
                        GameObject go = GameApp.Instance.ResourcesManagerScript.LoadInstantiateGameObject(rM.path, null, Vector3.zero);
                        //UI和代码同样也是分离的，方便的代码热更新
                        go.AddComponent<UI_Main>();
                    };
                }
                break;
            case GameResource.SceneName.BATTLE:
                {
                    LoadResourceModel rM = new LoadResourceModel();
                    rM.type = GameResource.ResourceType.PREFAB;
                    rM.path = GameResource.UIResourcePath +
                        GameData.Instance.CanvasName[GameResource.CanvasTag.CANCASBATTLE];
                    rModel.Add(rM);

                    LoadResourceModel rM1 = new LoadResourceModel();
                    rM1.type = GameResource.ResourceType.PREFAB;
                    rM1.path = GameResource.UIResourcePath +
                        GameData.Instance.SystemUI[GameResource.SystemUIType.UIFIGHT_TP];
                    rModel.Add(rM1);

                    LoadResourceModel rM2 = new LoadResourceModel();
                    rM2.type = GameResource.ResourceType.PREFAB;
                    rM2.path = GameResource.UIResourcePath +
                        GameData.Instance.SystemUI[GameResource.SystemUIType.CARDOTHER_TP];
                    rModel.Add(rM2);

                    call = delegate ()
                    {
                    //场景是空白场景，UI或其他资源为场景加载后进行实例化生成，添加到本场景
                    GameObject go = GameApp.Instance.ResourcesManagerScript.LoadInstantiateGameObject(rM.path, null, Vector3.zero);
                    //UI和代码同样也是分离的，方便的代码热更新
                    go.AddComponent<GameOther>();
                    };
                }
                break;
        }
        //开始加载场景
        GameApp.Instance.LoadMangerScript.StartLoadScene(tag, rModel, call);
    }
    /// <summary>
    /// 加载系统UI
    /// </summary>
    /// <param name="type"></param>
    /// <param name="call"></param>
    public void LoadSystemUI(GameResource.SystemUIType type, LoadManager.CallBack call = null)
    {
        //设置父对象为system
        Transform tfp = transform.parent.Find("system");
        GameObject go;
        //尝试获取缓存中是否含有加载对象
        if (!SystemUICache.TryGetValue(type, out go))
        {
            //如果没有，则加载生成之后，将页面对象添加到缓存中
            string path = GameResource.UIResourcePath + GameData.Instance.SystemUI[type];
            go = GameApp.Instance.ResourcesManagerScript.LoadInstantiateGameObject(path, tfp, Vector3.zero);
            SystemUICache.Add(type, go);
        }
        else
        {
            //否则直接将该页面调至最前并显示出来
            go.transform.SetAsLastSibling();
            go.SetActive(true);
        }
        //如果当前队列中该页面已经显示，则在队列中剔除
        for (int i = 0; i < SystemList.Count; i++)
        {
            if (SystemList[i] == type)
            {
                SystemList.RemoveAt(i);
                break;
            }
        }
        //将页面显示添加在队列最后一位
        SystemList.Add(type);
        if (call != null)
            call();
    }
    /// <summary>
    /// 关闭系统UI
    /// </summary>
    /// <param name="type"></param>
    public void CloseSystemUI(GameResource.SystemUIType type =  GameResource.SystemUIType.NULL,LoadManager.CallBack call = null)
    {
        //如果当前没有页面显示，则直接返回
        if (SystemList.Count <= 0)
            return;
        //如果直接关闭当前最前一层UI
        if (type == GameResource.SystemUIType.NULL )
        {
            GameObject go;
            if (!SystemUICache.TryGetValue(SystemList[SystemList.Count - 1], out go))
                return;
            go.SetActive(false);
            SystemList.RemoveAt(SystemList.Count - 1);
        }
        else
        {
            //关闭制定UI页面
            GameObject go;
            if (!SystemUICache.TryGetValue(type, out go))
                return;
            go.SetActive(false);
            //将UI页面从队列中剔除
            for (int i = 0; i < SystemList.Count; i++)
            {
                if (SystemList[i] == type)
                {
                    SystemList.RemoveAt(i);
                    break;
                }
            }
        }
        if (call != null)
            call();
    }
}
