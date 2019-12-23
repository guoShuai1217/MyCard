using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    /// <summary>
    /// 一级协议，定义模块
    /// </summary>
    public class TypeProtocol
    {
        /// <summary>
        /// 账号模块
        /// </summary>
        public const byte ACCOUNT = 1;
        /// <summary>
        /// 用户模块
        /// </summary>
        public const byte USER = 2;
        /// <summary>
        /// 匹配模块
        /// </summary>
        public const byte MATCH = 3;
        /// <summary>
        /// 战斗模块
        /// </summary>
        public const byte FIGHT = 4;
    }
}
