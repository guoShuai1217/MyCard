using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using GameProtocol.model.fight;
using GameProtocol;

public class TPSettlment : MonoBehaviour {

	void Awake () {
        OnAddClick();

    }

    public void OnAddClick() {
        Button btn = transform.Find("close").GetComponent<Button>();
        btn.onClick.AddListener(delegate () {
            this.Write(TypeProtocol.USER, UserProtocol.GETINFO_CREQ, null);
        });
    }

    public void ShowGameOver(List<TPSettlmentModel> list) {
        //获取待添加的父节点
        Transform tfp = transform.Find("ScrollView/Viewport/Content");
        //路径
        string path = GameResource.ItemResourcePath + GameData.Instance.ItemName[GameResource.ItemTag.TPSETTLMENTITEM];
        for (int i = 0; i < list.Count; i++)
        {
            GameObject go = GameApp.Instance.ResourcesManagerScript.LoadInstantiateGameObject(path, tfp, Vector3.zero);
            Text nick = go.transform.Find("nickname").GetComponent<Text>();
            Text score = go.transform.Find("Text").GetComponent<Text>();
            nick.text = list[i].nickname;
            score.text = list[i].score.ToString();
            //刷新手牌的扑克
            for (int j = 0; j < list[i].poker.Count; j++)
            {
                //获取到扑克组件
                Image img = go.transform.Find("poker" + (j + 1)).GetComponent<Image>();
                string popath = GameResource.PokerResourcePath + "_" + list[i].poker[j].Value + "_" + list[i].poker[j].Color;
                img.sprite = GameApp.Instance.ResourcesManagerScript.LoadSprite(popath);
            }
        }
    }
}
