using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.model.fight
{
    /// <summary>
    /// 赢三张结算
    /// </summary>
    [Serializable]
    public class TPSettlmentModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int id = -1;
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string nickname = "";
        /// <summary>
        /// 用户头像
        /// </summary>
        public string head = "";
        /// <summary>
        /// 用户分数
        /// </summary>
        public int score = 0;
        /// <summary>
        /// 用户手牌
        /// </summary>
        public List<PokerModel> poker = new List<PokerModel>();
    }
}
