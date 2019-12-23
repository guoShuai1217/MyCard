using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    /// <summary>
    /// 匹配模块二级执行逻辑
    /// </summary>
    public class MatchProtocol
    {
        /// <summary>
        /// 请求开始匹配
        /// SConst.RoomType
        /// </summary>
        public const int STARTMATCH_CREQ = 3001;
        /// <summary>
        /// 返回请求匹配结果
        /// ResponseStartMatchInfo
        /// </summary>
        public const int STARTMATCH_SRES = 3002;
        /// <summary>
        /// 请求离开匹配
        /// </summary>
        public const int LEAVEMATCH_CREQ = 3003;
        /// <summary>
        /// 返回请求离开结果
        /// 0离开成功
        /// -1游戏已经开始
        /// -2不在房间内
        /// </summary>
        public const int LEAVEMATCH_SRES = 3004;
        /// <summary>
        /// 同步匹配队列信息
        /// MatchInfoModel
        /// </summary>
        public const int MATCHINFO_BRQ = 3005;
        /// <summary>
        /// 匹配完成
        /// SConst.GameType
        /// </summary>
        public const int MATCHFINISH_BRQ = 3006;
        /// <summary>
        /// 匹配被关闭
        /// </summary>
        public const int MATCHCLOSE_BRQ = 3007;
    }
}
