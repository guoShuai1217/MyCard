using UnityEngine;
using System.Collections;

public class ResourcesManager : MonoBehaviour {

	void Awake () {
        GameApp.Instance.ResourcesManagerScript = this;
	}

    /// <summary>
    /// 图片资源的加载
    /// </summary>
    /// <param name="path">图片路径</param>
    /// <returns></returns>
    public Sprite LoadSprite(string path)
    {
        //通知Load函数根据路径开始加载资源，类型为Sprite，加载完成后，将资源类型转换为Sprite
        return Resources.Load(path, typeof(Sprite)) as Sprite;
    }
    /// <summary>
    /// 音频资源加载
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public AudioClip LoadClip(string path)
    {
        //通知Load函数根据路径开始加载资源，类型为AudioClip，加载完成后，将资源类型转换为AudioClip
        return Resources.Load(path, typeof(AudioClip)) as AudioClip;
    }
    /// <summary>
    /// 加载文本资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string LoadText(string path)
    {
        //通知Load函数根据路径开始加载资源，类型为TextAsset，加载完成后，将资源类型转换为TextAsset
        TextAsset txt = Resources.Load(path, typeof(TextAsset)) as TextAsset;
        //如果资源不存在，则返回空文本
        if (txt == null)
            return string.Empty;
        //否则返回资源中的文本
        return txt.text;
    }
    /// <summary>
    /// 加载预制体
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public GameObject LoadGameObject(string path)
    {
        //如果资源已经从本地加载过，则直接返回该资源对象
        GameObject go;
        if (GameData.Instance.LoadGameObjectCache.TryGetValue(path, out go))
            return go;
        //加载资源
        go = (GameObject)Resources.Load(path);
        //将资源添加到缓存中，以便下次使用
        GameData.Instance.LoadGameObjectCache.Add(path, go);
        return go;
    }
    /// <summary>
    /// 将预制体生成至组件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public GameObject LoadInstantiateGameObject(string path,Transform parent,Vector3 pos) {
        //加载该预制体
        GameObject go = LoadGameObject(path);
        //生成组件
        GameObject obj = Instantiate(go);
        //如果父对象不为空，则设置父对象
        if (parent != null)
            obj.transform.parent = parent;
        //设置初始父坐标
        obj.transform.localPosition = pos;
        //设置缩放
        obj.transform.localScale = Vector3.one;
        //设置显示
        obj.SetActive(true);
        return obj;
    }
}
