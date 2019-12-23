using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.model.fight
{
    /// <summary>
    /// 赢三张比牌模型
    /// </summary>
    [Serializable]
    public class TPCompareModel
    {
        //比牌的玩家
        public int userId = -1;
        //被比牌的玩家
        public int compId = -1;
        //比牌的结果
        public bool Result = false;
        //比牌的玩家的牌
        public List<PokerModel> PokerList1 = new List<PokerModel>();
        //被比牌的玩家的牌
        public List<PokerModel> PokerList2 = new List<PokerModel>();
    }
}
