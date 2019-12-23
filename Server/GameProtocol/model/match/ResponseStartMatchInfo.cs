using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.model.match
{
    /// <summary>
    /// 服务器返回匹配信息
    /// </summary>
    [Serializable]
    public class ResponseStartMatchInfo
    {
        /// <summary>
        /// 返回状态码
        /// 0开始匹配
        /// -1 当前余额不足
        /// -2 玩家已经在房间中
        /// </summary>
        public int Status = -1;
        /// <summary>
        /// 游戏类型
        /// </summary>
        public SConst.GameType Type = SConst.GameType.WINTHREEPOKER;
        /// <summary>
        /// 最大人数
        /// </summary>
        public int MaxPlayer = 0;
        /// <summary>
        /// 当前人数
        /// </summary>
        public int PlayerCount = 0;
    }
}
