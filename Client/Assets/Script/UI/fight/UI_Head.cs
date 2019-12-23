using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameProtocol.model.fight;

public class UI_Head : MonoBehaviour {
    /// <summary>
    /// 头像位置的集合
    /// </summary>
    public List<Vector3> PosList = new List<Vector3>();
    /// <summary>
    /// 玩家头像集合
    /// </summary>
    Dictionary<int, GameObject> HeadList = new Dictionary<int, GameObject>();

    private void Awake()
    {
        GameApp.Instance.UI_HeadScript = this;
    }
    void Start () {
        //初始化玩家头像位置
        switch (GameSession.Instance.RoomType)
        {
            //赢三张
            case GameProtocol.SConst.GameType.WINTHREEPOKER:
                {
                    //玩家从自己到最后一家的头像位置
                    PosList.Add(new Vector3(-388, -385));
                    PosList.Add(new Vector3(-843, -144));
                    PosList.Add(new Vector3(-843, 265));
                    PosList.Add(new Vector3(843, 265));
                    PosList.Add(new Vector3(843, -144));
                } break;
        }
	}
    /// <summary>
    /// 当前待刷新的玩家信息和玩家自己的方位
    /// </summary>
    /// <param name="model"></param>
    /// <param name="userdir"></param>
    public void UpdateItme(FightUserModel model,int userdir)
    {
        //移除原有信息
        if (HeadList.ContainsKey(model.id))
        {
            HeadList[model.id].GetComponent<HeadItem>().UpdateItem(model);
            return;
        }
        //头像待刷新的位置
        Vector3 pos;
        //如果是玩家自己，则直接使用第零个
        if (model.id == GameSession.Instance.UserInfo.id)
        {
            pos = PosList[0];
        }
        else {
            //如果玩家方位大于自己的方位的话
            if (model.Direction > userdir)
            {
                //直接用玩家的方位减去自己的方位即为玩家的头像位置
                pos = PosList[model.Direction - userdir];
            }
            else {
                //用玩家最大人数减去自己的方位再加上玩家的方位，即为玩家的位置
                pos = PosList[PosList.Count - userdir + model.Direction];
            }
        }
        //加载头像到页面中
        string path = GameResource.ItemResourcePath + GameData.Instance.ItemName[GameResource.ItemTag.TPHEAD];
        GameObject go = GameApp.Instance.ResourcesManagerScript.LoadInstantiateGameObject(path, transform, pos);
        go.AddComponent<HeadItem>().UpdateItem(model);
        HeadList.Add(model.id, go);
    }
	
}
