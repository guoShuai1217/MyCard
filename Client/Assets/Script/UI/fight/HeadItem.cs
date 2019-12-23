using UnityEngine;
using System.Collections;
using GameProtocol.model.fight;
using UnityEngine.UI;

public class HeadItem : MonoBehaviour {
    Text nick;
    Text coin;
    // Use this for initialization
    void Start () {
	
	}
    /// <summary>
    /// 刷新玩家的信息
    /// </summary>
    public void UpdateItem(FightUserModel model)
    {
        if (!nick)
            nick = transform.Find("nick").GetComponent<Text>();
        if (!coin)
            coin = transform.Find("coin").GetComponent<Text>();
        nick.text = model.nickname;
        coin.text = model.coin.ToString();
    }

}
