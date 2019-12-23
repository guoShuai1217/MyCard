using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    /// <summary>
    /// 登录模块行为协议
    /// </summary>
    public class AccountProtocol
    {
        /// <summary>
        /// 客户端请求快速注册
        /// req:null
        /// </summary>
        public const int QUICKREG_CREQ = 1001;
        /// <summary>
        /// 服务器返回注册结果
        /// res:ResponseRegisterModel
        /// </summary>
        public const int QUICKREG_SRES = 1002;
        /// <summary>
        /// 客户端请求登录
        /// req:RequestLoginModel
        /// </summary>
        public const int ENTER_CREQ = 1003;
        /// <summary>
        /// 服务器返回登录结果
        /// res:int Status  
        /// 0表示登录成功
        /// -1请求错误
        /// -2账号密码不合法
        /// -3表示没有此账号
        /// -4表示密码错误
        /// -5表示账号已登录
        /// </summary>
        public const int ENTER_SRES = 1004;
    }
}
