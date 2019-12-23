using NetFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFrame.auto;
using GameProtocol;
using GameProtocol.model.login;
using ServerTools;

namespace Server.logic.user
{
    public class UserHandler : IHandler
    {
        public void ClientClose(UserToken token, string error)
        {
            
        }

        public void MessageReceive(UserToken token, SocketModel message)
        {
            if (!cache.CacheFactory.user.IsOnline(token)) return;
            switch (message.command)
            {
                case UserProtocol.GETINFO_CREQ:
                    {
                        UserModel um = business.BizFactory.user.GetModel(token);
                        if (um != null)
                        {
                            DebugUtil.Instance.LogToTime(um.id + "获取用户信息");
                            token.write(TypeProtocol.USER, UserProtocol.GETINFO_SRES, um);
                        }
                        else {
                            DebugUtil.Instance.LogToTime(token.conn.RemoteEndPoint + "获取用户信息", LogType.WARRING);
                        }
                    }break;
            }
        }
    }
}
