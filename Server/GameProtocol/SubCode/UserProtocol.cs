using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    public class UserProtocol
    {
        /// <summary>
        /// 玩家登陆后获取玩家信息
        /// req:null
        /// </summary>
        public const int GETINFO_CREQ = 2001;
        /// <summary>
        /// 返回玩家信息
        /// 
        /// </summary>
        public const int GETINFO_SRES = 2002;
    }
}
