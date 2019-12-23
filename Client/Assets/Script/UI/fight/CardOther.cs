using UnityEngine;
using System.Collections;
using GameProtocol;
using GameProtocol.model.fight;
using System.Collections.Generic;

public class CardOther : MonoBehaviour {
    /// <summary>
    /// 队伍成员信息
    /// </summary>
    protected Dictionary<int, FightUserModel> TeamInfo = new Dictionary<int, FightUserModel>();

    //定义泛型 object == CardOther，通过泛型来获取子类
    private object cardOther { get { return GameApp.Instance.CardOtherScript; } }
    public T GetCardOther<T>()
    {
        return (T)cardOther;
    }

    private void Awake()
    {
        GameApp.Instance.CardOtherScript = this;
    }
    /// <summary>
    /// ID缓存
    /// </summary>
    List<int> cacheId = new List<int>();
    /// <summary>
    /// 刷新玩家信息
    /// </summary>
    /// <param name="model"></param>
    public void UpdateData(FightUserModel model) {
        if (TeamInfo.ContainsKey(model.id))
        {
            //TODO:刷新玩家信息
            UpdateItem(model);
            return;
        }
        //添加队伍成员
        TeamInfo.Add(model.id, model);
        //将队伍成员的ID进行缓存，直到收到玩家自己的信息
        if (!TeamInfo.ContainsKey(GameSession.Instance.UserInfo.id))
        {
            cacheId.Add(model.id);
            return;
        }
        for (int i = 0; i < cacheId.Count; i++)
        {
            UpdateItem(TeamInfo[cacheId[i]]);
        }
        cacheId.Clear();
        UpdateItem(model);
    }
    /// <summary>
    /// 根据游戏刷新具体的功能组件
    /// </summary>
    /// <param name="model"></param>
    protected virtual void UpdateItem(FightUserModel model) {   }
}
