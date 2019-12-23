using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.model.fight
{
    /// <summary>
    /// 下注信息广播
    /// </summary>
    [Serializable]
    public class TPBetModel
    {
        /// <summary>
        /// 下注的玩家
        /// </summary>
        public int id = -1;
        /// <summary>
        /// 下注的金额
        /// </summary>
        public int coin = 0;
        /// <summary>
        /// 是否为加注
        /// </summary>
        public bool isAdd = false;
    }
}
