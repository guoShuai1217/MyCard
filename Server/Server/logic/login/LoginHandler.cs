using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFrame;
using NetFrame.auto;
using Server.business;
using GameProtocol.model.login;
using ServerTools;
using GameProtocol;

namespace Server.logic.login
{
    /// <summary>
    /// 处理客户端二级业务消息分发
    /// </summary>
    public class LoginHandler : IHandler
    {
        /// <summary>
        /// 用户断开连接
        /// </summary>
        /// <param name="token"></param>
        /// <param name="error"></param>
        public void ClientClose(UserToken token, string error)
        {
            BizFactory.login.OffLine(token);
        }
        /// <summary>
        /// 用户消息到达
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        public void MessageReceive(UserToken token, SocketModel message)
        {
            //处理客户端的请求
            switch (message.command)
            {
                //处理客户端的登录请求
                case GameProtocol.AccountProtocol.ENTER_CREQ:
                    DebugUtil.Instance.LogToTime("用户请求登录消息到达");
                    //对三级消息体的转换
                    RequestLoginModel rlm = message.GetMessage<RequestLoginModel>();    //获取登录的结果
                    int result = BizFactory.login.Login(token, rlm);
                    //完成了一次对客户端的消息返回
                    token.write(TypeProtocol.ACCOUNT, AccountProtocol.ENTER_SRES, result);
                    break;
                //处理客户端的快速注册请求
                case GameProtocol.AccountProtocol.QUICKREG_CREQ:
                    DebugUtil.Instance.LogToTime("用户请求快速注册消息到达");
                    ResponseRegisterModel rrmodel = BizFactory.login.reg(token);
                    //完成了一次对客户端请求注册的消息返回
                    token.write(TypeProtocol.ACCOUNT, AccountProtocol.QUICKREG_SRES, rrmodel);
                    break;
            }
        }
    }
}
