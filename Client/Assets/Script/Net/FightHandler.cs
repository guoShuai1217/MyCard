using UnityEngine;
using System.Collections;
using ClientNetFrame;
using System;
using GameProtocol;
using GameProtocol.model.fight;
using System.Collections.Generic;

public class FightHandler : MonoBehaviour, IHandler
{
    /// <summary>
    /// 消息缓存列表
    /// </summary>
    List<SocketModel> SocketModelList = new List<SocketModel>();
    void Update()
    {
        //在房间加载完毕之后开始执行消息刷新
        if (GameApp.Instance.UI_FightScript)
        {
            while (SocketModelList.Count > 0)
            {
                MessageReceiveCallBack(SocketModelList[0]);
                SocketModelList.RemoveAt(0);
            }
        }
    }

    public void MessageReceiveCallBack(SocketModel model)
    {
        switch (model.command)
        {
            //玩家信息
            case FightProtocol.PLAYERINFO_BRQ:
                {
                    FightUserModel user = model.GetMessage<FightUserModel>();
                    GameApp.Instance.UI_FightScript.UpdateTeam(user);
                }
                break;
            //玩家请求准备
            case FightProtocol.ENTERFIGHT_SRES:
                {
                    int res = model.GetMessage<int>();
                    switch (res)
                    {
                        case 0:
                        case -1:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("已准备");
                            break;
                        case -2:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("房间错误");
                            break;
                    }
                }
                break;
            //确认准备的玩家列表
            case FightProtocol.ENTERFIGHT_BRQ:
                {
                    List<int> arr = model.GetMessage<List<int>>();
                }
                break;
            //玩家摸到的牌
            case FightProtocol.TPDRAWCARD_BRQ:
                {
                    List<PokerModel> poker = new List<PokerModel>();
                }
                break;
            //摸牌的玩家
            case FightProtocol.TPDRAWCARDUSER_BRQ:
                {
                    int id = model.GetMessage<int>();
                    GameApp.Instance.CardOtherScript.GetCardOther<TPCardOther>().DrawCard(id);
                }
                break;
            //获取底注
            case FightProtocol.TPBETBASECOIN_BRQ:
                {
                    int coin = model.GetMessage<int>();
                    GameApp.Instance.CardOtherScript.GetCardOther<TPCardOther>().BetBaseCoin(coin);
                }
                break;
            //返回下注请求结果
            case FightProtocol.TPBETCOIN_SRES:
                {
                    int res = model.GetMessage<int>();
                    switch (res)
                    {
                        case -4:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("小于当前最小金额");
                            break;
                        case -5:
                            GameApp.Instance.CommonHintDlgScript.OpenHintBox("大于当前最大金额");
                            break;
                    }
                }
                break;
                //玩家下注广播
            case FightProtocol.TPBETCOIN_BRQ:
                {
                    TPBetModel m = model.GetMessage<TPBetModel>();
                    GameApp.Instance.CardOtherScript.GetCardOther<TPCardOther>().BetCoin(m.coin);
                    //播放音效，默认加注，否则跟注
                    string path = GameResource.TPAudioResourcePath + GameData.Instance.MusicTag[GameResource.MusicTag.TPADDBETCOIN];
                    if (!m.isAdd)
                        path = GameResource.TPAudioResourcePath + GameData.Instance.MusicTag[GameResource.MusicTag.TPWITHBETCOIN];
                    GameApp.Instance.MusicMangerScript.PlayAudioEffect(path);
                }
                break;
            case FightProtocol.TPCHECKCARD_SRES:
                {
                    string path = GameResource.TPAudioResourcePath + GameData.Instance.MusicTag[GameResource.MusicTag.TPCHECKCARD];
                    GameApp.Instance.MusicMangerScript.PlayAudioEffect(path);
                    GameApp.Instance.CardOtherScript.GetCardOther<TPCardOther>().CheckCard(model.GetMessage<List<PokerModel>>());
                }break;
            case FightProtocol.TPCHECKCARD_BRQ:
                {
                    GameApp.Instance.CardOtherScript.GetCardOther<TPCardOther>().RechangeStatus(0, model.GetMessage<int>());
                }
                break;
            case FightProtocol.TPDISCARD_BRQ:
                {
                    GameApp.Instance.CardOtherScript.GetCardOther<TPCardOther>().RechangeStatus(1, model.GetMessage<int>());
                }
                break;
            case FightProtocol.TPCOMCARD_BRQ:
                {
                    TPCompareModel tpcm = model.GetMessage<TPCompareModel>();
                    string path = GameResource.TPAudioResourcePath + GameData.Instance.MusicTag[GameResource.MusicTag.TPCOMCARD];
                    GameApp.Instance.MusicMangerScript.PlayAudioEffect(path);
                    GameApp.Instance.CardOtherScript.GetCardOther<TPCardOther>().Compare(tpcm);
                }
                break;
                //游戏结算
            case FightProtocol.GAMESETTLMENT_BRQ:
                {
                    GameApp.Instance.UI_FightScript.GetUIFight<TPUI_Fight>().GameOver(model.GetMessage<List<TPSettlmentModel>>());
                }break;
        }
    }

    public void MessageReceive(SocketModel model)
    {
        //将服务器回执信息添加到列表中
        SocketModelList.Add(model);
    }
}
