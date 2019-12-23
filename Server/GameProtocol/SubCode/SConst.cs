using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    /// <summary>
    /// 定义服务器常量
    /// </summary>
    public class SConst
    {
        public enum GameType
        {
            /// <summary>
            /// 赢三张
            /// </summary>
            WINTHREEPOKER = 0,
            /// <summary>
            /// 血战到底
            /// </summary>
            XZDD = 1,
        }
    }
}
