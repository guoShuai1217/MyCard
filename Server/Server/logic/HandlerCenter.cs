using System;
using NetFrame;
using NetFrame.auto;
using GameProtocol;

namespace Server.logic
{
    /// <summary>
    /// 服务器消息一级分发执行中心
    /// </summary>
    public class HandlerCenter : NetFrame.AbsHandlerCenter
    {
        //登录
        IHandler LoginHandler;
        //用户
        IHandler UserHandler;
        //匹配
        IHandler MatchHandler;
        //战斗
        IHandler FightHandler;
        public HandlerCenter()
        {
            LoginHandler = new logic.login.LoginHandler();
            UserHandler = new logic.user.UserHandler();
            MatchHandler = new logic.match.MatchHandler();
            FightHandler = new logic.fight.FightHandler();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="token"></param>
        /// <param name="error"></param>
        public override void ClientClose(UserToken token, string error)
        {
            Console.WriteLine("有客户端断开连接" + token.conn.RemoteEndPoint);
            //按照先后顺序，依次退出
            FightHandler.ClientClose(token, error);
            MatchHandler.ClientClose(token, error);
            UserHandler.ClientClose(token, error);
            LoginHandler.ClientClose(token, error);
        }
        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="token"></param>
        public override void ClientConnect(UserToken token)
        {
            Console.WriteLine("有客户端开始连接" + token.conn.RemoteEndPoint);
        }
        /// <summary>
        /// 消息到达
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        public override void MessageReceive(UserToken token, object message)
        {
            Console.WriteLine("有客户端消息到达" + token.conn.RemoteEndPoint);
            //将类型转换为socketmodel
            SocketModel model = message as SocketModel;
            //分发一级模块业务
            switch (model.type)
            {
                //处理登录模块的业务请求
                case TypeProtocol.ACCOUNT:
                    LoginHandler.MessageReceive(token, model);
                    break;
                //处理用户模块的业务请求
                case TypeProtocol.USER:
                    UserHandler.MessageReceive(token, model);
                    break;
                //处理用户匹配模块的业务请求
                case TypeProtocol.MATCH:
                    MatchHandler.MessageReceive(token, model);
                    break;
                //处理用户战斗模块的业务请求
                case TypeProtocol.FIGHT:
                    FightHandler.MessageReceive(token, model);
                    break;
            }
        }
    }
}
