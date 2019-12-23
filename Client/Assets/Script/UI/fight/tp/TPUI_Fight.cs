using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameProtocol;
using System.Collections.Generic;
using GameProtocol.model.fight;

public class TPUI_Fight : UI_Fight {
    private void Awake()
    {
        GameApp.Instance.UI_FightScript = this;
        StartRoom();
    }

    private void Start()
    {
        AddOnClick();
    }

    private void OnDestroy()
    {
        if (UpdateTimeId != -1)
            GameApp.Instance.TimeManagerScript.Remove(UpdateTimeId);
    }
    /// <summary>
    /// 为按钮添加回调
    /// </summary>
    public void AddOnClick() {
        //获取弃牌按钮
        Button DiscardBtn = GameInfoPanel.transform.Find("system/disButton").GetComponent<Button>();
        DiscardBtn.onClick.AddListener(delegate () {
            //向服务器请求弃牌
            this.Write(TypeProtocol.FIGHT, FightProtocol.TPDISCARD_CREQ, null);
        });
        //获取比牌按钮
        Button CompareBtn = GameInfoPanel.transform.Find("system/comButton").GetComponent<Button>();
        CompareBtn.onClick.AddListener(delegate () {
            GameApp.Instance.CardOtherScript.GetCardOther<TPCardOther>().UIFightReqCompare();
        });
        //获取跟注按钮
        Button BetCoinBtn = GameInfoPanel.transform.Find("system/betButton").GetComponent<Button>();
        BetCoinBtn.onClick.AddListener(delegate () {
            //向服务器请求跟注
            this.Write(TypeProtocol.FIGHT, FightProtocol.TPBETCOIN_CREQ, -1);
        });

        //获取加注1按钮
        Button BetCoin1Btn = GameInfoPanel.transform.Find("system/AddBetPanel/Button1").GetComponent<Button>();
        BetCoin1Btn.onClick.AddListener(delegate () {
            //向服务器请求下注1
            this.Write(TypeProtocol.FIGHT, FightProtocol.TPBETCOIN_CREQ, 1);
        });
        //获取加注2按钮
        Button BetCoin2Btn = GameInfoPanel.transform.Find("system/AddBetPanel/Button2").GetComponent<Button>();
        BetCoin2Btn.onClick.AddListener(delegate () {
            //向服务器请求下注2
            this.Write(TypeProtocol.FIGHT, FightProtocol.TPBETCOIN_CREQ, 2);
        });
        //获取加注5按钮
        Button BetCoin5Btn = GameInfoPanel.transform.Find("system/AddBetPanel/Button5").GetComponent<Button>();
        BetCoin5Btn.onClick.AddListener(delegate () {
            //向服务器请求下注5
            this.Write(TypeProtocol.FIGHT, FightProtocol.TPBETCOIN_CREQ, 5);
        });
        //获取加注10按钮
        Button BetCoin10Btn = GameInfoPanel.transform.Find("system/AddBetPanel/Button10").GetComponent<Button>();
        BetCoin10Btn.onClick.AddListener(delegate () {
            //向服务器请求下注10
            this.Write(TypeProtocol.FIGHT, FightProtocol.TPBETCOIN_CREQ, 10);
        });
        //获取加注20按钮
        Button BetCoin20Btn = GameInfoPanel.transform.Find("system/AddBetPanel/Button20").GetComponent<Button>();
        BetCoin20Btn.onClick.AddListener(delegate () {
            //向服务器请求下注20
            this.Write(TypeProtocol.FIGHT, FightProtocol.TPBETCOIN_CREQ, 20);
        });

        //获取加注40按钮
        Button BetCoin40Btn = GameInfoPanel.transform.Find("system/AddBetPanel/Button40").GetComponent<Button>();
        BetCoin40Btn.onClick.AddListener(delegate () {
            //向服务器请求下注40
            this.Write(TypeProtocol.FIGHT, FightProtocol.TPBETCOIN_CREQ, 40);
        });
        //加注单选框
        Toggle addToggle = GameInfoPanel.transform.Find("system/addToggle").GetComponent<Toggle>();
        addToggle.onValueChanged.AddListener(delegate (bool ison) {
            //根据单选框的选中状态来显示/隐藏加注页面
            GameInfoPanel.transform.Find("system/AddBetPanel").gameObject.SetActive(ison);
        });


    }

    public void GameOver(List<TPSettlmentModel> list) {
        string path = GameResource.UIResourcePath + GameData.Instance.SystemUI[GameResource.SystemUIType.TPSETTLMENT];
        GameObject go = GameApp.Instance.ResourcesManagerScript.LoadInstantiateGameObject(path, transform, Vector3.zero);
        go.AddComponent<TPSettlment>().ShowGameOver(list);
    }

}
