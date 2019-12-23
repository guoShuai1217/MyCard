using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    public class FightProtocol
    {
        #region 公共协议40
        /// <summary>
        /// 广播用户信息
        /// FightUserModel
        /// </summary>
        public const int PLAYERINFO_BRQ = 4001;
        /// <summary>
        /// 用户请求确认进入游戏
        /// null
        /// </summary>
        public const int ENTERFIGHT_CREQ = 4002;
        /// <summary>
        /// 返回确认结果
        /// int
        /// -1准备失败，已经准备了
        /// -2 准备失败，不在本房间
        /// </summary>
        public const int ENTERFIGHT_SRES = 4003;
        /// <summary>
        /// 广播确认信息
        /// List<int> 准备的玩家
        /// </summary>
        public const int ENTERFIGHT_BRQ = 4004;
        /// <summary>
        /// 请求离开游戏
        /// </summary>
        public const int LEAVEFIGHT_CREQ = 4005;
        /// <summary>
        /// 确认离开
        /// </summary>
        public const int LEAVEFIGHT_SRES = 4006;
        /// <summary>
        /// 广播游戏房间已解散
        /// </summary>
        public const int GAMEDISSOLVE_BRQ = 4007;
        /// <summary>
        /// 广播游戏开始
        /// SConst.GameType
        /// </summary>
        public const int GAMESTART_BRQ = 4008;
        /// <summary>
        /// 广播游戏结束
        /// </summary>
        public const int GAMESETTLMENT_BRQ = 4009;
        #endregion

        #region 金花41
        /// <summary>
        /// 玩家自己摸到的牌
        /// List<PokerModel> 玩家摸到的牌
        /// </summary>
        public const int TPDRAWCARD_BRQ = 4100;
        /// <summary>
        /// 金花下底注广播
        /// int 底注数量
        /// </summary>
        public const int TPBETBASECOIN_BRQ = 4101;
        /// <summary>
        /// 广播玩家摸牌
        /// int 摸牌的玩家
        /// </summary>
        public const int TPDRAWCARDUSER_BRQ = 4102;
        /// <summary>
        /// 刷新当前按钮状态：弃牌、看牌、比牌、下注
        /// </summary>
        public const int TPUPDATESTATUS_BRQ = 4103;
        /// <summary>
        /// 请求看牌
        /// null
        /// </summary>
        public const int TPCHECKCARD_CREQ = 4104;
        /// <summary>
        /// 返回看牌结果
        /// List<PokerModel>
        /// </summary>
        public const int TPCHECKCARD_SRES = 4105;
        /// <summary>
        /// 广播看牌
        /// int 看牌的玩家
        /// </summary>
        public const int TPCHECKCARD_BRQ = 4106;
        /// <summary>
        /// 请求下注
        /// int 下注的金额 -1为跟注
        /// </summary>
        public const int TPBETCOIN_CREQ = 4107;
        /// <summary>
        /// 返回下注结果
        /// -1请求错误，没有此玩家
        /// -2请求错误，当前不是此玩家
        /// -3游戏未开始
        /// -4低于当前可下最小金额
        /// -5大于当前可下最大金额
        /// </summary>
        public const int TPBETCOIN_SRES = 4108;
        /// <summary>
        /// 广播下注
        /// TPBetModel
        /// </summary>
        public const int TPBETCOIN_BRQ = 4109;
        /// <summary>
        /// 请求弃牌
        /// </summary>
        public const int TPDISCARD_CREQ = 4110;
        /// <summary>
        /// 广播弃牌
        /// </summary>
        public const int TPDISCARD_BRQ = 4111;
        /// <summary>
        /// 请求比牌
        /// </summary>
        public const int TPCOMCARD_CREQ = 4112;
        /// <summary>
        /// 请求比牌结果
        /// </summary>
        public const int TPCOMCARD_SRES = 4113;
        /// <summary>
        /// 比牌广播
        /// </summary>
        public const int TPCOMCARD_BRQ = 4114;
        #endregion
    }
}
