using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.model.match
{
    /// <summary>
    /// 匹配信息模型
    /// </summary>
    [Serializable]
    public class MatchInfoModel
    {
        /// <summary>
        /// 房间号
        /// </summary>
        public int RoomId = 0;
        /// <summary>
        /// 最大人数
        /// </summary>
        public int MaxPlayer = 0;
        /// <summary>
        /// 当前玩家ID列表
        /// </summary>
        public List<int> Team = new List<int>();
        /// <summary>
        /// 游戏类型
        /// </summary>
        public SConst.GameType GameType = SConst.GameType.WINTHREEPOKER;
    }
}
