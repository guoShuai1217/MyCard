using NetFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFrame.auto;
using GameProtocol.model.fight;
using GameProtocol.model.match;
using GameProtocol;
using Server.dao;
using ServerTools;
using Server.cache;

namespace Server.logic.fight
{
    public class FightRoom : IHandler
    {
        /// <summary>
        /// 队伍成员ID
        /// </summary>
        public List<int> TemeId = new List<int>();
        /// <summary>
        /// 房间号
        /// </summary>
        protected int RoomId = -1;
        /// <summary>
        /// 玩家ID和玩家信息的映射
        /// </summary>
        protected Dictionary<int, FightUserModel> UserFight = new Dictionary<int, FightUserModel>();
        /// <summary>
        /// 房间类型
        /// </summary>
        protected SConst.GameType GameType = SConst.GameType.WINTHREEPOKER;
        /// <summary>
        /// 确认准备好游戏开始
        /// </summary>
        private List<int> readrole = new List<int>();
        /// <summary>
        /// 游戏是否开始
        /// </summary>
        protected bool IsGameStart = false;
        /// <summary>
        /// 房间是否开始进行游戏
        /// </summary>
        protected bool IsRoomStart = false;
        /// <summary>
        /// 当前循环的玩家列表
        /// </summary>
        protected List<int> LoopOrder = new List<int>();
        /// <summary>
        /// 玩家最大人数
        /// </summary>
        protected int MaxPlayer = 0;
        /// <summary>
        /// 方位
        /// </summary>
        private List<int> Direction = new List<int>();

        public void ClientClose(UserToken token, string error)
        {
            
        }
        /// <summary>
        /// 房间请求
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        public void MessageReceive(UserToken token, SocketModel message)
        {
            switch (message.command)
            {
                //玩家请求准备
                case FightProtocol.ENTERFIGHT_CREQ:
                    {
                        token.write(TypeProtocol.FIGHT, FightProtocol.ENTERFIGHT_SRES, Enter(token));
                    }
                    break;
                //离开房间
                case FightProtocol.LEAVEFIGHT_CREQ:
                    {
                        Leave(token);
                    }
                    break;
                default:
                    {
                        GameMessageReceive(token, message);
                    }break;
            }
        }
        /// <summary>
        /// 初始化房间
        /// </summary>
        /// <param name="model"></param>
        public void Init(MatchInfoModel model)
        {
            //初始化房间类型
            GameType = model.GameType;
            //初始化队伍成员
            TemeId.Clear();
            TemeId.AddRange(model.Team);
            //初始化房间ID
            RoomId = model.RoomId;
            //初始化人数
            MaxPlayer = model.MaxPlayer;
            //添加方位数量
            for (int i = 0; i < MaxPlayer; i++)
            {
                Direction.Add(i);
            }
            //初始化玩家信息
            for (int i = 0; i < model.Team.Count; i++)
            {
                FightUserModel m = new FightUserModel();
                //获取用户信息
                RoleInfo ri = cache.CacheFactory.user.Get(model.Team[i]);
                //如果获取到，则将玩家信息赋值给用户信息
                if (ri != null)
                {
                    m.coin = ri.coin;
                    m.nickname = ri.nickname;
                    m.id = ri.id;
                }
                //否则，设置为默认信息
                else {
                    m.coin = 0;
                    m.nickname = "nickname";
                    m.id = model.Team[i];
                }
                //赋值玩家当前方位
                m.Direction = Direction[0];
                Direction.RemoveAt(0);
                UserFight.Add(m.id, m);
            }
            //广播玩家信息
            for (int i = 0; i < TemeId.Count; i++)
            {
                Broadcast(FightProtocol.PLAYERINFO_BRQ, UserFight[TemeId[i]]);
            }

            DebugUtil.Instance.LogToTime(RoomId + "房间初始化成功");
        }
        /// <summary>
        /// 向本房间所有玩家发送消息
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="obj"></param>
        protected void Broadcast(int Command,object obj)
        {
            for (int i = 0; i < TemeId.Count; i++)
            {
                UserToken token = cache.CacheFactory.user.GetToken(TemeId[i]);
                if(token!=null)
                {
                    token.write(TypeProtocol.FIGHT, Command, obj);
                }
            }
        }
        /// <summary>
        /// 向指定玩家发送消息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="Command"></param>
        /// <param name="obj"></param>
        protected void SendMessage(int uid, int Command, object obj)
        {
            UserToken token = cache.CacheFactory.user.GetToken(uid);
            if (token != null)
            {
                token.write(TypeProtocol.FIGHT, Command, obj);
            }
        }

        /// <summary>
        /// 玩家确认准备开始游戏
        /// </summary>
        /// <param name="token"></param>
        /// <returns>-1 准备失败，已经准备了</returns>
        /// <returns>-2 准备失败，不在本房间</returns>
        int Enter(UserToken token) {
            int uid = CacheFactory.user.GetIdToToken(token);
            if (readrole.Contains(uid)) {
                DebugUtil.Instance.LogToTime(uid + "玩家已经准备，无需再次准备");
                return -1;
            }
            if(!TemeId.Contains(uid))
            {
                DebugUtil.Instance.LogToTime(uid + "玩家不在此房间，无需准备");
                return -2;
            }
            //将玩家添加到准备列表
            readrole.Add(uid);
            //将准备列表广播给所有玩家
            Broadcast(FightProtocol.ENTERFIGHT_BRQ, readrole);
            DebugUtil.Instance.LogToTime(uid + "玩家准备成功");
            if (readrole.Count == TemeId.Count)
            {
                DebugUtil.Instance.LogToTime(RoomId + "房间全部准备，游戏即将开始");
                IsGameStart = true;
                IsRoomStart = true;
                //广播游戏开始
                Broadcast(FightProtocol.GAMESTART_BRQ, SConst.GameType.WINTHREEPOKER);
                StartGame();
            }
            return 0;
        }

        void Add() {

        }

        void Leave(UserToken token) {

        }

        protected void Close()
        {
            DebugUtil.Instance.LogToTime(RoomId + "房间即将解散");
            CacheFactory.fight.Close(RoomId);
        }
        /// <summary>
        /// 基于方位进行初始排序
        /// </summary>
        protected void SortLoopOrderInDirection() {
            LoopOrder.Clear();
            //根据玩家当前方位进行初始排序
            for (int i = 0; i < MaxPlayer; i++)
            {
                foreach (FightUserModel user in UserFight.Values)
                {
                    if (user.Direction == i)
                    {
                        LoopOrder.Add(user.id);
                    }
                }
            }
        }
        /// <summary>
        /// 基于用户ID的排序
        /// </summary>
        protected void SortLoopInUser(int uid) {
            //如果玩家数量等于0或者不包含此玩家则直接返回
            if (LoopOrder.Count == 0 || !LoopOrder.Contains(uid)) return;
            //添加排序规则
            while (LoopOrder[0] != uid)
            {
                LoopOrder.Add(LoopOrder[0]);
                LoopOrder.RemoveAt(0);
            }
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        protected virtual void StartGame() { DebugUtil.Instance.LogToTime(RoomId + "房间游戏开始"); }
        /// <summary>
        /// 游戏消息到达
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        protected virtual void GameMessageReceive(UserToken token,SocketModel message) { }
        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="isExist">是否正常结算</param>
        /// <param name="exption">结束消息</param>
        protected virtual void GameOver(bool isSettlment = false,string exption = "") { }
    }
}
