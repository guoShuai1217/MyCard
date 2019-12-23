using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.dao
{
    public class RoleInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int id = -1;
        /// <summary>
        /// 账号
        /// </summary>
        public string username;
        /// <summary>
        /// 密码
        /// </summary>
        public string password;
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname;
        /// <summary>
        /// 头像
        /// </summary>
        public string head = "default";
        /// <summary>
        /// 金币
        /// </summary>
        public int coin = 10000;
        /// <summary>
        /// 钻石
        /// </summary>
        public int cash = 10000;
        /// <summary>
        /// 性别 0男1女2未知
        /// </summary>
        public int sex = 0;
        /// <summary>
        /// 手机号
        /// </summary>
        public string phone = "";
        /// <summary>
        /// 排行分数
        /// </summary>
        public int rank = 0;
    }
}
