using NetFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFrame.auto;
using Server.cache;
using GameProtocol;
using Server.business;
using GameProtocol.model.match;

namespace Server.logic.match
{
    public class MatchHandler : IHandler
    {
        public void ClientClose(UserToken token, string error)
        {
            BizFactory.match.LeaveMatch(token);
        }

        public void MessageReceive(UserToken token, SocketModel message)
        {
            if (!CacheFactory.user.IsOnline(token)) return;
            switch (message.command)
            {
                //请求开始匹配
                case MatchProtocol.STARTMATCH_CREQ:
                    {
                        //获取请求匹配的结果
                        ResponseStartMatchInfo result = BizFactory.match.StartMatch(token, (SConst.GameType)message.GetMessage<int>());
                        token.write(TypeProtocol.MATCH, MatchProtocol.STARTMATCH_SRES, result);
                    }
                    break;
                //请求离开匹配
                case MatchProtocol.LEAVEMATCH_CREQ:
                    {
                        int result = BizFactory.match.LeaveMatch(token);
                        token.write(TypeProtocol.MATCH, MatchProtocol.LEAVEMATCH_SRES, result);
                    }
                    break;
            }
        }

    }
}
