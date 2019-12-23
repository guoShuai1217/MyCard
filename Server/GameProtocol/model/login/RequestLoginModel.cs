using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.model.login
{
    /// <summary>
    /// 请求登录模型
    /// </summary>
    [Serializable]
    public class RequestLoginModel
    {
        /// <summary>
        /// 渠道
        /// 0表示普通账号
        /// 1表示微信登陆
        /// 2表示手机登录
        /// ......
        /// </summary>
        public int Ditch = 0;
        /// <summary>
        /// 账号
        /// </summary>
        public string Username = "";
        /// <summary>
        /// 密码
        /// </summary>
        public string Password = "";
    }
}
