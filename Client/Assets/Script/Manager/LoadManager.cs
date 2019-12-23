using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour {
    /// <summary>
    /// 回调
    /// </summary>
    public delegate void CallBack();
    /// <summary>
    /// 已加载的资源数量
    /// </summary>
    int LoadResourceNumber = 0;
    /// <summary>
    /// 资源加载进度值 
    /// </summary>
    int ResProgressValue = 0;
    /// <summary>
    /// 场景加载进度值
    /// </summary>
    int SceneProgressValue = 0;
    /// <summary>
    /// 加载进度条组件
    /// </summary>
    Slider ProgressSilder;
    /// <summary>
    /// 加载进度显示组件
    /// </summary>
    Text ProgressText;
    /// <summary>
    /// 加载进度组件
    /// </summary>
    GameObject ProgressGameObject;
    /// <summary>
    /// 是否开始加载进度
    /// </summary>
    bool IsStartLoading = false;
    /// <summary>
    /// 最大资源加载进度
    /// </summary>
    int MaxResourceProgressValue = 100;
    /// <summary>
    /// 资源加载显示进度
    /// </summary>
    int ResProgress = 0;
    /// <summary>
    /// 异步加载场景的对象
    /// </summary>
    AsyncOperation async;

    void Awake () {
        GameApp.Instance.LoadMangerScript = this;
        //获取组件
        ProgressSilder = transform.Find("go/ValueSlider").GetComponent<Slider>();
        ProgressText = transform.Find("go/ValueText").GetComponent<Text>();
        ProgressGameObject = transform.Find("go").gameObject;
        ProgressGameObject.SetActive(false);
    }
    /// <summary>
    /// 开始加载下一个场景
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="res"></param>
    public void StartLoadScene(GameResource.SceneName scene,List<LoadResourceModel> res, CallBack loadcall = null)
    {
        MaxResourceProgressValue = 100;
        //重置进度条显示
        ProgressSilder.value = 0;
        //重置进度显示
        ProgressText.text = "正在加载中... 0%";
        //重置资源加载值
        ResProgressValue = 0;
        //重置资源加载进度值
        LoadResourceNumber = 0;
        //重置场景加载进度值
        SceneProgressValue = 0;
        //重置资源加载显示进度
        ResProgress = 0;
        //开始加载场景
        IsStartLoading = true;
        switch (scene)
        {
            case GameResource.SceneName.MAIN:
                MaxResourceProgressValue = 0;
                break;
            default:
                ProgressGameObject.SetActive(true);
                break;
        }
        
        StartCoroutine(LoadScene(scene, res, loadcall));
    }
    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    IEnumerator LoadScene(GameResource.SceneName scene,List<LoadResourceModel> list,CallBack loadcall)
    {
        //展示用的进度
        int displaypro = 0;
        //实际的进度
        int topro = 0;
        //开始加载下一个场景
        async = SceneManager.LoadSceneAsync(GameData.Instance.SceneName[scene]);
        //暂时不进去下一个场景
        async.allowSceneActivation = false;
        //在加载进度不足90%时，进行进度条缓动动画
        while (async.progress < 0.9f)
        {
            topro = (int)(async.progress * 100f);
            //如果我们的显示进度尚未达到实际进度时，每帧增加百分之一
            while (displaypro < topro)
            {
                displaypro++;
                SceneProgressValue = displaypro / 2;
                yield return new WaitForFixedUpdate();
            }
        }
        //加载最后一段进度
        topro = 100;
        while (displaypro < topro)
        {
            displaypro++;
            SceneProgressValue = displaypro / 2;
            yield return new WaitForFixedUpdate();
        }
        //加载资源
        LoadResource(list);
        displaypro = 0;
        //如果我们的显示进度尚未达到实际进度时，每帧增加百分之一
        while (ResProgressValue <= 100 && displaypro < MaxResourceProgressValue)
        {
            displaypro++;
            ResProgress = displaypro / 2;
            yield return new WaitForFixedUpdate();
        }
        //全部加载完毕后，进入下一个场景
        async.allowSceneActivation = true;
        //等待场景真正加载完毕
        while (!async.isDone)
            yield return new WaitForFixedUpdate();
        IsStartLoading = false;
        //全部加载完毕后，执行回调
        if (loadcall != null)
            loadcall();
        //将加载页面隐藏
        ProgressGameObject.SetActive(false);
    }
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="res"></param>
    void LoadResource(List<LoadResourceModel> res) {
        for (int i = 0; i < res.Count; i++)
        {
            switch (res[i].type)
            {
                case GameResource.ResourceType.TEXTASSET:
                    LoadResourceCallBack(res.Count);
                    break;
                case GameResource.ResourceType.AUDIO:
                    {
                        GameApp.Instance.MusicMangerScript.LoadClip(res[i].path);
                        LoadResourceCallBack(res.Count);
                    }
                    break;
                case GameResource.ResourceType.SPRITE:
                    LoadResourceCallBack(res.Count);
                    break;
                case GameResource.ResourceType.PREFAB:
                    {
                        GameApp.Instance.ResourcesManagerScript.LoadGameObject(res[i].path);
                        LoadResourceCallBack(res.Count);
                    }
                    break;
            }
        }
        if (res.Count == 0)
            LoadResourceCallBack(0);
    }
    /// <summary>
    /// 加载资源完成后回调，更新加载数量
    /// </summary>
    /// <param name="rModelCount"></param>
    void LoadResourceCallBack(int rModelCount) {
        //已加载数量 / 待加载数量 * 100% 即为 资源加载进度
        float num = 100f / rModelCount;
        LoadResourceNumber++;
        ResProgressValue = (int)(LoadResourceNumber * num);
    }
    /// <summary>
    /// 更新进度显示
    /// </summary>
    void FixedUpdate()
    {
        if (IsStartLoading)
        {
            ProgressSilder.value = SceneProgressValue / 100f + ResProgress / 100f;
            ProgressText.text = "正在加载中... " + (SceneProgressValue + ResProgress) + "%";
        }
    }
}
