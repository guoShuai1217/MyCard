using ServerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFrame;
using NetFrame.auto;
using GameProtocol;
using GameProtocol.model.fight;
using Server.tool.poker;

namespace Server.logic.fight
{
    /// <summary>
    /// 赢三张游戏
    /// </summary>
    public class TPFightRoom:FightRoom
    {
        /// <summary>
        /// 底注
        /// </summary>
        int BaseCoin = 1;
        /// <summary>
        /// 当前扑克列表
        /// </summary>
        List<PokerModel> PokerList = new List<PokerModel>();
        /// <summary>
        /// 赢三张工具类
        /// </summary>
        TPokerUtil TPokerUtil = new TPokerUtil();
        /// <summary>
        /// 现在的金额
        /// </summary>
        int NowScore = 1;
        /// <summary>
        /// 最大的可下注金额
        /// </summary>
        int MaxScore = 40;
        /// <summary>
        /// 看牌的玩家列表
        /// </summary>
        List<int> CheckList = new List<int>();
        /// <summary>
        /// 弃牌的玩家列表
        /// </summary>
        List<int> DisCardList = new List<int>();
        /// <summary>
        /// 下注金额的集合
        /// </summary>
        Dictionary<int, List<int>> BetCoinList = new Dictionary<int, List<int>>();
        /// <summary>
        /// 进行游戏请求的逻辑处理
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        protected override void GameMessageReceive(UserToken token, SocketModel message)
        {
            int userid = cache.CacheFactory.user.GetIdToToken(token);
            switch (message.command)
            {
                //请求看牌
                case FightProtocol.TPCHECKCARD_CREQ:
                    {
                        ReqCheckCard(userid);
                    }
                    break;
                //请求比牌
                case FightProtocol.TPCOMCARD_CREQ:
                    {
                        ReqCompare(userid, message.GetMessage<int>());
                    }
                    break;
                //请求下注
                case FightProtocol.TPBETCOIN_CREQ:
                    {
                        DebugUtil.Instance.Log("请求下注" + message.GetMessage<int>());
                        ReqBet(userid, message.GetMessage<int>());
                    }
                    break;
                //请求弃牌
                case FightProtocol.TPDISCARD_CREQ:
                    {
                        ReqDisCard(userid);
                    }
                    break;
            }
        }
        /// <summary>
        /// 游戏开始
        /// </summary>
        protected override void StartGame()
        {
            DebugUtil.Instance.LogToTime(RoomId + "房间赢三张游戏开始");
            //获取扑克列表
            PokerList = TPokerUtil.GetPokerList();
            //进行初始排序
            SortLoopOrderInDirection();
            //最多金币玩家ID
            int MaxCoinId = -1;
            //查找最多金币玩家ID
            foreach (FightUserModel user in UserFight.Values)
            {
                //清理掉之前的数据
                user.poker.Clear();
                if (MaxCoinId == -1)
                    MaxCoinId = user.id;
                //查找最多金币玩家ID
                if (user.id != MaxCoinId && user.coin > UserFight[MaxCoinId].coin)
                    MaxCoinId = user.id;
            }
            //根据金币规则进行排序
            SortLoopInUser(MaxCoinId);
            //开始发牌
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < LoopOrder.Count; j++)
                {
                    int userid = LoopOrder[j];
                    UserFight[userid].poker.Add(PokerList[0]);
                    PokerList.RemoveAt(0);
                }
            }
            //打印玩家的手牌
            for (int j = 0; j < LoopOrder.Count; j++)
            {
                string card = LoopOrder[j] + ":";
                for (int i = 0; i < UserFight[LoopOrder[j]].poker.Count; i++)
                {
                    card += "V" + UserFight[LoopOrder[j]].poker[i].Value + "C" + UserFight[LoopOrder[j]].poker[i].Color;
                }
                DebugUtil.Instance.LogToTime("玩家手牌" + card);
                //通知玩家自己摸到的牌
                //SendMessage(LoopOrder[j], FightProtocol.TPDRAWCARD_BRQ, UserFight[LoopOrder[j]].poker);
                //通知所有玩家该玩家摸了牌
                Broadcast(FightProtocol.TPDRAWCARDUSER_BRQ, LoopOrder[j]);
            }
            //广播下底注
            foreach (FightUserModel model in UserFight.Values)
            {
                model.coin -= BaseCoin;
            }
            Broadcast(FightProtocol.TPBETBASECOIN_BRQ, LoopOrder.Count * BaseCoin);
            //广播玩家剩余筹码
            for (int i = 0; i < LoopOrder.Count; i++)
                Broadcast(FightProtocol.PLAYERINFO_BRQ, UserFight[LoopOrder[i]]);
        }
        /// <summary>
        /// 请求看牌
        /// </summary>
        /// <param name="uid">看牌的玩家</param>
        private void ReqCheckCard(int uid) {
            if (!UserFight.ContainsKey(uid) || !LoopOrder.Contains(uid))
            {
                DebugUtil.Instance.LogToTime(uid + "请求看牌失败，没有此玩家");
                return;
            }
            if (CheckList.Contains(uid))
            {
                DebugUtil.Instance.LogToTime(uid + "请求看牌失败，已经看过牌了");
                return;
            }
            //告知玩家自己的手牌
            SendMessage(uid, FightProtocol.TPCHECKCARD_SRES, UserFight[uid].poker);
            //通知所有人，自己看过牌了
            Broadcast(FightProtocol.TPCHECKCARD_BRQ, uid);
            //将看牌的玩家添加到列表中
            CheckList.Add(uid);
            DebugUtil.Instance.LogToTime(uid + "请求看牌成功" + RoomId);
        }
        /// <summary>
        /// 请求弃牌
        /// </summary>
        /// <param name="uid"></param>
        private void ReqDisCard(int uid)
        {
            if (!UserFight.ContainsKey(uid) || !LoopOrder.Contains(uid))
            {
                DebugUtil.Instance.LogToTime(uid + "请求弃牌失败，没有此玩家");
                return;
            }
            if (DisCardList.Contains(uid))
            {
                DebugUtil.Instance.LogToTime(uid + "请求弃牌失败，已经弃过牌了");
                return;
            }
            if (LoopOrder.Count < 2)
            {
                DebugUtil.Instance.LogToTime(uid + "请求弃牌失败，玩家不足了，最后一个人直接胜利");
                return;
            }
            //广播玩家弃牌消息
            Broadcast(FightProtocol.TPDISCARD_BRQ, uid);
            //将玩家添加到弃牌列表中
            DisCardList.Add(uid);
            //将玩家从当前玩家列表中删除
            LoopOrder.Remove(uid);
            DebugUtil.Instance.LogToTime(uid + "请求弃牌成功");
            //TODO:NEXT LOOPORDER
            if (LoopOrder.Count == 1)
                GameOver();
        }
        /// <summary>
        /// 请求下注
        /// </summary>
        /// <param name="uid">请求下注的玩家</param>
        /// <param name="coin">请求下注的金额</param>
        private void ReqBet(int uid,int coin)
        {
            //初始请求的下注金额
            bool initCoin = coin == -1 ? false : true;
            if (!UserFight.ContainsKey(uid) || !LoopOrder.Contains(uid))
            {
                DebugUtil.Instance.LogToTime(uid + "请求错误，没有此玩家");
                SendMessage(uid, FightProtocol.TPBETCOIN_SRES, -1);
                return;
            }
            if (LoopOrder[0] != uid)
            {
                DebugUtil.Instance.LogToTime(uid + "请求错误，当前不是此玩家");
                SendMessage(uid, FightProtocol.TPBETCOIN_SRES, -2);
                return;
            }
            if(!IsGameStart)
            {
                DebugUtil.Instance.LogToTime(uid + "请求错误，游戏尚未开始");
                SendMessage(uid, FightProtocol.TPBETCOIN_SRES, -3);
                return;
            }
            //是否看过牌
            if (CheckList.Contains(uid))
            {
                //跟注
                if (coin == -1)
                {
                    coin = NowScore * 2;
                    if (coin == 4)
                        coin = 5;
                }
                initCoin = coin == (NowScore * 2) ? false : true;
                if (coin == 5 && NowScore == 2)
                    initCoin = false;
                //是否小于最小可下注金额
                if (coin < NowScore * 2)
                {
                    DebugUtil.Instance.LogToTime(uid + "请求下注失败，当前可下注金额最小为" + (NowScore * 2));
                    SendMessage(uid, FightProtocol.TPBETCOIN_SRES, -4);
                    return;
                }
                //是否大于最大可下注金额
                if (coin > MaxScore)
                {
                    DebugUtil.Instance.LogToTime(uid + "请求下注失败，当前最大可下注金额为" + MaxScore);
                    SendMessage(uid, FightProtocol.TPBETCOIN_SRES, -5);
                    return;
                }
                if (coin == 5)
                {
                    NowScore = 2;
                }
            }
            else
            {
                if (coin == -1)
                    coin = NowScore;
                initCoin = coin == NowScore ? false : true;
                //是否小于最小可下注金额
                if (coin < NowScore)
                {
                    DebugUtil.Instance.LogToTime(uid + "请求下注失败，当前可下注金额最小为" + NowScore);
                    SendMessage(uid, FightProtocol.TPBETCOIN_SRES, -4);
                    return;
                }
                //是否大于最大可下注金额
                if (coin > MaxScore / 2)
                {
                    DebugUtil.Instance.LogToTime(uid + "请求下注失败，当前最大可下注金额为" + (MaxScore/2));
                    SendMessage(uid, FightProtocol.TPBETCOIN_SRES, -5);
                    return;
                }
                NowScore = coin;
            }
            
            Bet(uid, coin, initCoin);
        }
        /// <summary>
        /// 处理下注事件
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="coin"></param>
        /// <param name="initCoin"></param>
        private void Bet(int uid,int coin,bool initCoin) {
            //将当前下注玩家和下注金额添加到集合中，等待结算
            if (!BetCoinList.ContainsKey(uid))
                BetCoinList.Add(uid, new List<int>());
            BetCoinList[uid].Add(coin);
            //声明待广播的数据,将下注数据广播
            TPBetModel tpm = new TPBetModel();
            tpm.id = uid;
            tpm.coin = coin;
            tpm.isAdd = initCoin;
            Broadcast(FightProtocol.TPBETCOIN_BRQ, tpm);
            //更新玩家筹码后广播给所有玩家
            UserFight[uid].coin -= coin;
            Broadcast(FightProtocol.PLAYERINFO_BRQ, UserFight[uid]);
            //将当前玩家移动到最后面，让下一家发话
            LoopOrder.Add(LoopOrder[0]);
            LoopOrder.RemoveAt(0);
            DebugUtil.Instance.LogToTime(uid + "下注" + coin + "房间号" + RoomId + "是否看牌" + CheckList.Contains(uid));
        }
        /// <summary>
        /// 请求比牌
        /// </summary>
        /// <param name="uid">请求比牌的玩家</param>
        /// <param name="cid">被比牌的玩家</param>
        private void ReqCompare(int uid, int cid)
        {
            if (!UserFight.ContainsKey(uid) || !LoopOrder.Contains(uid))
            {
                DebugUtil.Instance.LogToTime(uid + "请求错误，没有此玩家");
                SendMessage(uid, FightProtocol.TPCOMCARD_SRES, -1);
                return;
            }
            if (!UserFight.ContainsKey(cid) || !LoopOrder.Contains(cid))
            {
                DebugUtil.Instance.LogToTime(uid + "请求错误，没有此玩家");
                SendMessage(uid, FightProtocol.TPCOMCARD_SRES, -1);
                return;
            }
            if (LoopOrder[0] != uid)
            {
                DebugUtil.Instance.LogToTime(uid + "请求错误，当前不是此玩家");
                SendMessage(uid, FightProtocol.TPCOMCARD_SRES, -2);
                return;
            }
            if (!IsGameStart)
            {
                DebugUtil.Instance.LogToTime(uid + "请求错误，游戏尚未开始");
                SendMessage(uid, FightProtocol.TPCOMCARD_SRES, -3);
                return;
            }
            int coin = NowScore;
            if (CheckList.Contains(uid))
                coin *= 2;
            if (coin == 4)
                coin = 5;
            //将当前下注玩家和下注金额添加到集合中，等待结算
            if (!BetCoinList.ContainsKey(uid))
                BetCoinList.Add(uid, new List<int>());
            BetCoinList[uid].Add(coin);
            //声明待广播的数据,将下注数据广播
            TPBetModel tpm = new TPBetModel();
            tpm.id = uid;
            tpm.coin = coin;
            tpm.isAdd = false;
            Broadcast(FightProtocol.TPBETCOIN_BRQ, tpm);
            //更新玩家筹码后广播给所有玩家
            UserFight[uid].coin -= coin;
            Broadcast(FightProtocol.PLAYERINFO_BRQ, UserFight[uid]);

            //首轮可比  三轮可比 五轮可比
            //默认首轮可比，将玩家的手牌和被比玩家的手牌传入，获取结果
            bool GetResult = TPokerUtil.GetComparePoker(UserFight[uid].poker, UserFight[cid].poker);
            DebugUtil.Instance.LogToTime(uid + "请求和" + cid + "比牌，比牌结果" + GetResult);
            for (int i = 0; i < LoopOrder.Count; i++)
            {
                TPCompareModel model = new TPCompareModel();
                model.userId = uid;
                model.compId = cid;
                model.Result = GetResult;
                //如果是比牌或者被比牌的玩家，则可以看到牌
                if (LoopOrder[i] == uid || LoopOrder[i] == cid)
                {
                    model.PokerList1.AddRange(UserFight[uid].poker);
                    model.PokerList2.AddRange(UserFight[cid].poker);
                }
                SendMessage(LoopOrder[i], FightProtocol.TPCOMCARD_BRQ, model);
            }
            if (GetResult)
                LoopOrder.Remove(cid);
            else
                LoopOrder.Remove(uid);
            if (LoopOrder.Count == 1)
                GameOver();
        }
        /// <summary>
        /// 获取结算
        /// </summary>
        private void GetSettlmet() {
            if (LoopOrder.Count != 1) return;
            List<TPSettlmentModel> model = new List<TPSettlmentModel>();
            //将底注结算给玩家
            int Coin = TemeId.Count * BaseCoin;
            //失败的玩家
            for (int i = 0; i < TemeId.Count; i++)
            {
                if (TemeId[i] == LoopOrder[0]) continue;
                TPSettlmentModel m = new TPSettlmentModel();
                m.id = TemeId[i];
                m.nickname = UserFight[TemeId[i]].nickname;
                //分数先减去底注
                m.score -= BaseCoin;
                //UserFight[TemeId[i]].coin -= BaseCoin;
                if (BetCoinList.ContainsKey(TemeId[i]))
                {
                    for (int j = 0; j < BetCoinList[TemeId[i]].Count; j++)
                    {
                        //再减去玩家下注的分数
                        Coin += BetCoinList[TemeId[i]][j];
                        m.score -= BetCoinList[TemeId[i]][j];
                        //UserFight[TemeId[i]].coin-= BetCoinList[TemeId[i]][j];
                    }
                }
                m.poker.AddRange(UserFight[TemeId[i]].poker);
                model.Add(m);
            }
            //胜利的玩家
            TPSettlmentModel m2 = new TPSettlmentModel();
            m2.id = LoopOrder[0];
            //分数
            m2.score = Coin - BaseCoin;
            m2.nickname = UserFight[LoopOrder[0]].nickname;
            //添加玩家赢的筹码
            UserFight[LoopOrder[0]].coin += Coin;
            //返回玩家下注的筹码
            if (BetCoinList.ContainsKey(LoopOrder[0]))
            {
                for (int j = 0; j < BetCoinList[LoopOrder[0]].Count; j++)
                {
                    UserFight[LoopOrder[0]].coin += BetCoinList[LoopOrder[0]][j];
                }
            }
            m2.poker.AddRange(UserFight[LoopOrder[0]].poker);
            model.Add(m2);
            for (int i = 0; i < TemeId.Count; i++)
                cache.CacheFactory.user.Get(TemeId[i]).coin = UserFight[TemeId[i]].coin;
            ScheduleUtil.Instance.AddSchedule(delegate ()
            {
                Broadcast(FightProtocol.GAMESETTLMENT_BRQ, model);
                Close();
            }, 5000);
        }
        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="isSettlment"></param>
        /// <param name="exption"></param>
        protected override void GameOver(bool isSettlment = true, string exption = "")
        {
            DebugUtil.Instance.LogToTime(RoomId + "房间赢三张游戏结束");
            if (isSettlment)
                GetSettlmet();
            else
                Close();
        }


    }
}
