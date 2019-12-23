using GameProtocol;
using GameProtocol.model.match;
using NetFrame;
using ServerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.cache
{
    public class MatchCache
    {
        /// <summary>
        /// 房间号和房间的队列
        /// </summary>
        public Dictionary<int, MatchInfoModel> MatchInfo = new Dictionary<int, MatchInfoModel>();
        /// <summary>
        /// 玩家和房间的映射
        /// </summary>
        public Dictionary<int, int> UserToMatch = new Dictionary<int, int>();
        /// <summary>
        /// 是否开始游戏
        /// 匹配队列ID和游戏开始的映射
        /// </summary>
        public Dictionary<int, bool> IsStartGame = new Dictionary<int, bool>();
        /// <summary>
        /// 置随机数种子
        /// </summary>
        Random ran = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// 是否在匹配队列中
        /// </summary>
        /// <returns>-1 没有在队列中</returns>
        /// <returns>返回一个房间号</returns>
        public int IsOnMatchArray(int userid)
        {
            if (UserToMatch.ContainsKey(userid) && MatchInfo.ContainsKey(UserToMatch[userid]))
            {
                return UserToMatch[userid];
            }
            return -1;
        }
        /// <summary>
        /// 获取一个进入房间的最小货币数量
        /// </summary>
        /// <param name="type">房间类型</param>
        /// <returns></returns>
        public int GetRoomCoinAtType(SConst.GameType type)
        {
            switch (type)
            {
                case SConst.GameType.WINTHREEPOKER:
                    return 1000;
                case SConst.GameType.XZDD:
                    return 100;
                default: return 1000000000;
            }
        }
        /// <summary>
        /// 添加匹配到匹配队列
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="roomtype"></param>
        public void AddMatch(int userid, SConst.GameType roomtype, ref ResponseStartMatchInfo info)
        {
            #region 如果当前有匹配队列
            if (MatchInfo.Count > 0)
            {
                //创建一条匹配队列房间号的列表
                List<int> Roomid = new List<int>(MatchInfo.Keys);
                for (int i = 0; i < Roomid.Count; i++)
                {
                    //如果匹配队列和将要进行的游戏类型一致
                    if (MatchInfo[Roomid[i]].GameType == roomtype)
                    {
                        //如果当前队列中有队伍未满员
                        if (MatchInfo[Roomid[i]].Team.Count < MatchInfo[Roomid[i]].MaxPlayer)
                        {
                            //将玩家添加到匹配队列中
                            MatchInfo[Roomid[i]].Team.Add(userid);
                            //添加返回给客户端的信息
                            info.PlayerCount = MatchInfo[Roomid[i]].Team.Count;
                            info.MaxPlayer = MatchInfo[Roomid[i]].MaxPlayer;
                            info.Type = roomtype;
                            //添加玩家和房间号的映射
                            UserToMatch.Add(userid, Roomid[i]);
                            DebugUtil.Instance.LogToTime(userid + "请求加入匹配成功,队列号" + Roomid[i]);
                            IsFinsih(Roomid[i]);
                            return;
                        }
                    }
                }
            }
            #endregion
            #region 创建一个新的匹配队列
            MatchInfoModel model = new MatchInfoModel();
            //设定当前具有玩家数量来开启游戏
            model.MaxPlayer = 2;
            model.RoomId = GetMatchId();
            model.GameType = roomtype;
            model.Team.Add(userid);
            //添加返回给客户端的信息
            info.PlayerCount = model.Team.Count;
            info.MaxPlayer = model.MaxPlayer;
            info.Type = roomtype;
            //添加房间号和房间的映射
            MatchInfo.Add(model.RoomId, model);
            //添加玩家和房间号的映射
            UserToMatch.Add(userid, model.RoomId);
            //添加游戏是否开始的映射
            if (!IsStartGame.ContainsKey(model.RoomId))
                IsStartGame.Add(model.RoomId, false);
            DebugUtil.Instance.LogToTime(userid + "请求创建匹配成功,队列号" + model.RoomId);
            IsFinsih(model.RoomId);
            #endregion
        }
        /// <summary>
        /// 是否匹配完成
        /// </summary>
        /// <param name="roomid"></param>
        private void IsFinsih(int roomid)
        {
            //将队伍成员信息广播给所有队伍成员
            for (int i = 0; i < MatchInfo[roomid].Team.Count; i++)
            {
                UserToken token = CacheFactory.user.GetToken(MatchInfo[roomid].Team[i]);
                token.write(TypeProtocol.MATCH, MatchProtocol.MATCHINFO_BRQ, MatchInfo[roomid]);
            }
            //匹配成功
            if (MatchInfo[roomid].Team.Count == MatchInfo[roomid].MaxPlayer)
            {
                DebugUtil.Instance.LogToTime(roomid + "队列当前匹配成功");
                //等待一秒钟之后，创建游戏房间
                ScheduleUtil.Instance.AddSchedule(delegate () {
                    for (int i = 0; i < MatchInfo[roomid].Team.Count; i++)
                    {
                        UserToken token = CacheFactory.user.GetToken(MatchInfo[roomid].Team[i]);
                        token.write(TypeProtocol.MATCH, MatchProtocol.MATCHFINISH_BRQ, MatchInfo[roomid]);
                    }
                    SetStartGame(roomid);
                    ScheduleUtil.Instance.AddSchedule(delegate () {
                        CacheFactory.fight.Create(MatchInfo[roomid]);
                    }, 1000);
                }, 1000);
            }
        }
        /// <summary>
        /// 获取匹配队列ID
        /// </summary>
        /// <returns></returns>
        private int GetMatchId()
        {
            //生成一个六位数的房间号
            int id = ran.Next(100000, 999999);
            while(MatchInfo.ContainsKey(id))
                id = ran.Next(100000, 999999);
            return id;
        }
        /// <summary>
        /// 获取是否游戏已经开始
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        private bool GetStartGame(int roomid)
        {
            if (IsStartGame.ContainsKey(roomid))
                return false;
            return IsStartGame[roomid];
        }
        /// <summary>
        /// 获取是否游戏已经开始
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        private bool GetStartGame(UserToken token)
        {
            int userid = CacheFactory.user.GetIdToToken(token);
            int roomid = UserToMatch[userid];
            if (IsStartGame.ContainsKey(roomid))
                return false;
            return IsStartGame[roomid];
        }
        /// <summary>
        /// 判断一个玩家是否在匹配队列
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool GetHaveMatch(UserToken token) {
            //获取玩家ID
            int userid = CacheFactory.user.GetIdToToken(token);
            //判断是否含有玩家匹配队列映射关系
            if (!UserToMatch.ContainsKey(userid)) return false;
            //判断匹配队列是否存在
            if (!MatchInfo.ContainsKey(UserToMatch[userid])) return false;
            //判断匹配队列中是否有该玩家
            if (!MatchInfo[UserToMatch[userid]].Team.Contains(userid)) return false;
            return true;
        }
        /// <summary>
        /// 将游戏设置为已经开始
        /// </summary>
        /// <param name="roomid"></param>
        public void SetStartGame(int roomid) {
            if (IsStartGame.ContainsKey(roomid)) IsStartGame[roomid] = true;
        }
        /// <summary>
        /// 离开匹配队列
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public int LeaveMatch(UserToken token) {
            int userid = CacheFactory.user.GetIdToToken(token);
            //判断是否含有匹配队列
            if (!GetHaveMatch(token)) {
                DebugUtil.Instance.LogToTime(userid + "请求离开匹配失败，当前没有匹配队列");
                return -2;
            }
            //判断是否已经开始
            if (GetStartGame(token)) {
                DebugUtil.Instance.LogToTime(userid + "请求离开匹配失败，游戏已经开始");
                return -1;
            }
            int roomid = UserToMatch[userid];
            MatchInfoModel model = MatchInfo[roomid];
            model.Team.Remove(userid);
            UserToMatch.Remove(userid);
            //如果当前没有玩家了，则移除匹配队列
            if (model.Team.Count == 0)
            {
                MatchInfo.Remove(roomid);
                IsStartGame.Remove(roomid);
                DebugUtil.Instance.LogToTime(userid + "请求离开匹配成功，移除匹配队列" + roomid);
            }
            //否则，广播给此队列所有玩家
            else {
                //将队伍成员信息广播给所有队伍成员
                for (int i = 0; i < MatchInfo[roomid].Team.Count; i++)
                {
                    UserToken tokens = CacheFactory.user.GetToken(MatchInfo[roomid].Team[i]);
                    tokens.write(TypeProtocol.MATCH, MatchProtocol.MATCHINFO_BRQ, MatchInfo[roomid]);
                }
                DebugUtil.Instance.LogToTime(userid + "请求离开匹配成功，匹配队列" + roomid);
            }
            return 0;
        }
        /// <summary>
        /// 关闭匹配队列
        /// </summary>
        /// <param name="roomid"></param>
        public void CloseMatch(int roomid)
        {
            if (!MatchInfo.ContainsKey(roomid)) return;
            for (int i = 0; i < MatchInfo[roomid].Team.Count; i++)
            {
                int userid = MatchInfo[roomid].Team[i];
                UserToken token = CacheFactory.user.GetToken(userid);
                token.write(TypeProtocol.MATCH, MatchProtocol.MATCHCLOSE_BRQ, null);
                //移除玩家映射
                if (UserToMatch.ContainsKey(userid))
                    UserToMatch.Remove(userid);
            }
            MatchInfo.Remove(roomid);
            DebugUtil.Instance.LogToTime(roomid + "匹配队列已被移除", LogType.NOTICE);
        }
    }
}
