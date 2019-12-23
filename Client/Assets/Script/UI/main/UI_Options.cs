using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour {
    /// <summary>
    /// 音乐开关
    /// </summary>
    Toggle MusicOpenTog;
    Toggle MusicCloseTog;
    /// <summary>
    /// 音效开关
    /// </summary>
    Toggle EffectOpenTog;
    Toggle EffectCloseTog;
    /// <summary>
    /// 定位开关
    /// </summary>
    Toggle PostionOpenTog;
    Toggle PostionCloseTog;
    /// <summary>
    /// 关闭显示页面
    /// </summary>
    Button CloseBtn;
	void Awake () {
        GameApp.Instance.UI_OptionsScript = this;
        MusicOpenTog = transform.Find("music/opentog").GetComponent<Toggle>();
        MusicCloseTog = transform.Find("music/closetog").GetComponent<Toggle>();
        EffectOpenTog = transform.Find("effect/opentog").GetComponent<Toggle>();
        EffectCloseTog = transform.Find("effect/closetog").GetComponent<Toggle>();
        PostionOpenTog = transform.Find("postion/opentog").GetComponent<Toggle>();
        PostionCloseTog = transform.Find("postion/closetog").GetComponent<Toggle>();
        CloseBtn = transform.Find("closebtn").GetComponent<Button>();
        AddOnClick();
    }

    void AddOnClick()
    {
        MusicOpenTog.onValueChanged.RemoveAllListeners();
        MusicCloseTog.onValueChanged.RemoveAllListeners();
        EffectOpenTog.onValueChanged.RemoveAllListeners();
        EffectCloseTog.onValueChanged.RemoveAllListeners();
        PostionOpenTog.onValueChanged.RemoveAllListeners();
        PostionCloseTog.onValueChanged.RemoveAllListeners();
        CloseBtn.onClick.RemoveAllListeners();
        //音乐打开
        MusicOpenTog.onValueChanged.AddListener(delegate (bool isOn) {
            if (isOn)
                GameApp.Instance.MusicMangerScript.SetPlayBgmAudio(true);
            else
                GameApp.Instance.MusicMangerScript.SetPlayBgmAudio(false);
        });
        //音效打开
        EffectOpenTog.onValueChanged.AddListener(delegate (bool isOn) {
            if (isOn)
                GameApp.Instance.MusicMangerScript.SetPlayEffectAudio(true);
            else
                GameApp.Instance.MusicMangerScript.SetPlayEffectAudio(false);
        });
        //执行关闭
        CloseBtn.onClick.AddListener(delegate () {
            GameApp.Instance.GameLevelManagerScript.CloseSystemUI(GameResource.SystemUIType.UIOPTIONSPANEL);
        });
    }

    public void UpdateData() {
        //获取是否播放音乐
        bool isplayermusic = GameApp.Instance.MusicMangerScript.IsPlayAudioBgm;
        //获取是否播放音效
        bool isplayereff = GameApp.Instance.MusicMangerScript.IsPlayAudioEff;
        if (isplayermusic)
            MusicOpenTog.isOn = true;
        else
            MusicCloseTog.isOn = true;

        if (isplayereff)
            EffectOpenTog.isOn = true;
        else
            EffectCloseTog.isOn = true;
    }
	
}
