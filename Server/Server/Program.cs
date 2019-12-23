using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFrame;
using ServerTools;
namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //开启的服务器端口
            int port = 6650;
            //最大客户端连接人数
            int maxClient = 1000;
            DebugUtil.Instance.LogToTime("服务器初始化", LogType.NOTICE);
            DebugUtil.Instance.LogToTime("Port:" + port, LogType.NOTICE);
            //初始化一个服务器通讯程序
            ServerStart server = new ServerStart(maxClient);
            #region 为服务器添加日志输出委托
            DebugMessage.debug = delegate (object obj)
            {
                DebugUtil.Instance.LogToTime(obj);
            };
            DebugMessage.notice = delegate (object obj)
            {
                DebugUtil.Instance.LogToTime(obj, LogType.NOTICE);
            };
            DebugMessage.error = delegate (object obj)
            {
                DebugUtil.Instance.LogToTime(obj, LogType.ERROR);
            };
            #endregion
            //初始化一个消息分发中心
            server.center = new logic.HandlerCenter();
            //开启服务器
            server.Start(port);
            DebugUtil.Instance.LogToTime("初始化控制台", LogType.NOTICE);
            //控制台工具
            ConsoleUtil cons = new ConsoleUtil();
            //注册正常关闭监听函数
            cons.RegisterCtrlHandler();
            //注册异常关闭监听函数
            //为当前应用作用域添加一个异常函数
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(cons.UnHandlerExceptionEventHandler);
            DebugUtil.Instance.LogToTime("服务器执行成功", LogType.NOTICE);
            while (true) { }
        }
    }
}
