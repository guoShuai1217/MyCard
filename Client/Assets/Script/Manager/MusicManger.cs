using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
public class MusicManger : MonoBehaviour {
    /// <summary>
    /// 本地音乐的缓存
    /// </summary>
    Dictionary<string, AudioClip> ClipCache = new Dictionary<string, AudioClip>();
    /// <summary>
    /// 背景音乐播放组件
    /// </summary>
    AudioSource bgmsource;
	AudioSource BgmSource {
        get {
            if (bgmsource == null)
            {
                //获取挂置对象是否含有音乐播放组件,如果没有，则添加一个新的
                if (gameObject.GetComponent<AudioSource>())
                    bgmsource = gameObject.GetComponent<AudioSource>();
                else
                    bgmsource = gameObject.AddComponent<AudioSource>();
            }
            return bgmsource;
        }
    }

    /// <summary>
    /// 音效播放组件
    /// </summary>
    /// <typeparam name="AudioSource"></typeparam>
    /// <param name=""></param>
    /// <returns></returns>
    List<AudioSource> EffectSource = new List<AudioSource>();
    /// <summary>
    /// 音乐播放开关
    /// </summary>
    public bool IsPlayAudioBgm = true;
    /// <summary>
    /// 音效播放开关
    /// </summary>
    public bool IsPlayAudioEff = true;

    void Awake()
    {
        GameApp.Instance.MusicMangerScript = this;
        InitLoad();
    }

    #region 数据持久化
    /// <summary>
    /// 初始化加载数据
    /// </summary>
    public void InitLoad()
    {
        string path = Application.persistentDataPath;
        //去沙盒加载数据
        string data = FileUtil.LoadFile(path, "musicdata.txt");
        if (data == null || data == "")
            return;
        JsonData jd = JsonMapper.ToObject(data);
        //用music作为key存储音乐管理器的数据
        //用bgm作为key存储背景音乐的数据
        int bgmisplay = (int)jd["music"]["bgm"];
        //用effect做为key存储音效的数据
        int effisplay = (int)jd["music"]["effect"];
        //用0来表示关闭音乐，用1表示开启音乐
        IsPlayAudioBgm = bgmisplay == 0 ? false : true;
        IsPlayAudioEff = effisplay == 0 ? false : true;
    }
    #endregion

    public AudioClip LoadClip(string path)
    {
        AudioClip clip;
        if (ClipCache.TryGetValue(path, out clip))
            return clip;
        //获取一个本地音效资源
        clip = GameApp.Instance.ResourcesManagerScript.LoadClip(path);
        //添加在本地缓存
        ClipCache.Add(path, clip);
        return clip;
    }

    #region 背景音乐
    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBgmAudio(string name)
    {
        //获取一个本地音效资源
        AudioClip clip = LoadClip(name);
        //如果获取失败，则直接返回
        if (clip == null) return;
        //将音效资源放置到播放器组件上
        BgmSource.clip = clip;
        //循环播放
        BgmSource.loop = true;
        //设置音量为1，最小值为0，最大值为1
        BgmSource.volume = 1;
        //如果当前可以播放音乐，则进行播放
        if(IsPlayAudioBgm)
            BgmSource.Play();
    }
    /// <summary>
    /// 关闭背景音乐
    /// </summary>
    public void CloseBgmAudio()
    {
        BgmSource.Stop();
    }
    /// <summary>
    /// 设置是否播放音乐
    /// </summary>
    /// <param name="isplay"></param>
    public void SetPlayBgmAudio(bool isplay)
    {
        IsPlayAudioBgm = isplay;
        //如果设置为不播放,则停止当前播放
        if (!isplay)
            CloseBgmAudio();
        //如果当前播放器含有播放资源，则进行播放
        else if(BgmSource.clip != null)
        {
            BgmSource.loop = true;
            BgmSource.Play();
        }
        //根据是否播放背景音乐和是否播放音效来保存音乐数据
        string str = "{\"music\":{" + 
                            "\"bgm\":" + (IsPlayAudioBgm ? 1 : 0) + "," +
                            "\"effect\":" + (IsPlayAudioEff ? 1 : 0) + "}}";
        FileUtil.CreateFile(Application.persistentDataPath, "musicdata.txt", str);
    }
    #endregion

    #region 音效
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name"></param>
    public void PlayAudioEffect(string name)
    {
        if (!IsPlayAudioEff) return;
        AudioClip clip = LoadClip(name);
        if (clip == null) return;
        //获取一个空置的播放器
        AudioSource source = GetAudioSource();
        source.clip = clip;
        source.loop = false;
        source.Play();
    }
    /// <summary>
    /// 关闭音效
    /// </summary>
    /// <param name="name"></param>
    public void CloseAudioEffect(string name)
    {
        for (int i = 0; i < EffectSource.Count; i++)
        {
            //找到与传入进来的音效名称一致的播放器，停止播放
            if (EffectSource[i].clip.name == name)
            {
                EffectSource[i].Stop();
                return;
            }
        }
    }
    /// <summary>
    /// 是否播放音效
    /// </summary>
    /// <param name="isplay"></param>
    public void SetPlayEffectAudio(bool isplay) {
        IsPlayAudioEff = isplay;
        //根据是否播放背景音乐和是否播放音效来保存音乐数据
        string str = "{\"music\":{" +
                            "\"bgm\":" + (IsPlayAudioBgm ? 1 : 0) + "," +
                            "\"effect\":" + (IsPlayAudioEff ? 1 : 0) + "}}";
        FileUtil.CreateFile(Application.persistentDataPath, "musicdata.txt", str);
    }
    /// <summary>
    /// 获取一个音效播放器
    /// </summary>
    /// <returns></returns>
    AudioSource GetAudioSource() {
        for (int i = 0; i < EffectSource.Count; i++)
        {
            //获取一个没有进行播放音效的播放器
            if (!EffectSource[i].isPlaying)
            {
                return EffectSource[i];
            }
        }
        //如果音效播放器全部被占用，没有获取到，则直接创建一个新的
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.volume = 1;
        source.loop = false;
        //添加至音效播放器列表
        EffectSource.Add(source);
        return source;
    }
    #endregion
}
