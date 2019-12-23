using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameProtocol;

public class UI_Main : MonoBehaviour {
    /// <summary>
    /// 头像图片
    /// </summary>
    Image HeadImg;
    /// <summary>
    /// 昵称标签
    /// </summary>
    Text NickNameText;
    /// <summary>
    /// id标签
    /// </summary>
    Text IdText;
    /// <summary>
    /// 金币标签
    /// </summary>
    Text CoinText;
    /// <summary>
    /// 钻石标签
    /// </summary>
    Text CashText;
    /// <summary>
    /// 设置按钮
    /// </summary>
    Button UI_OptionsBtn;
    /// <summary>
    /// 炸金花按钮
    /// </summary>
    Button UI_TPokerBtn;

    private void Awake()
    {
        GameApp.Instance.UI_MainScript = this;
        //预先为组件赋值
        HeadImg = transform.Find("head").GetComponent<Image>();
        NickNameText = transform.Find("nickname").GetComponent<Text>();
        IdText = transform.Find("id").GetComponent<Text>();
        CoinText = transform.Find("coin/cointext").GetComponent<Text>();
        CashText = transform.Find("cash/cashtext").GetComponent<Text>();
        UI_OptionsBtn = transform.Find("system/setting").GetComponent<Button>();
        UI_TPokerBtn = transform.Find("ThreePoker").GetComponent<Button>();
        GameSession.Instance.UserInfoChangeHandler += UpdateData;
    }
    void Start () {
        UpdateData();
        UI_OptionsBtn.onClick.AddListener(delegate () {
            GameApp.Instance.GameLevelManagerScript.LoadSystemUI(GameResource.SystemUIType.UIOPTIONSPANEL,delegate() {
                GameObject go;
                if (GameApp.Instance.GameLevelManagerScript.SystemUICache.TryGetValue(GameResource.SystemUIType.UIOPTIONSPANEL, out go))
                {
                    if (!go.GetComponent<UI_Options>())
                        go.AddComponent<UI_Options>();
                    GameApp.Instance.UI_OptionsScript.UpdateData();
                }
            });
        });
        UI_TPokerBtn.onClick.AddListener(delegate () {
            this.Write(TypeProtocol.MATCH, MatchProtocol.STARTMATCH_CREQ, SConst.GameType.WINTHREEPOKER);
        });
        string path = GameResource.AudioResourcePath + GameData.Instance.MusicTag[GameResource.MusicTag.MAINBACKGROUDMUSIC];
        GameApp.Instance.MusicMangerScript.PlayBgmAudio(path);
    }

    private void OnDestroy()
    {
        GameSession.Instance.UserInfoChangeHandler -= UpdateData;
    }

    void UpdateData()
    {
        if (HeadImg == null || GameSession.Instance.UserInfo == null)
            return;
        NickNameText.text = GameSession.Instance.UserInfo.nickname;
        IdText.text = "ID:" + (100000 + GameSession.Instance.UserInfo.id);
        CoinText.text = GameSession.Instance.UserInfo.coin.ToString();
        CashText.text = GameSession.Instance.UserInfo.cash.ToString();
    }

}
