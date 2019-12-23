using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerTools
{
    public enum LogType
    {
        FATAL = 0,//未捕获的异常  LV.0
        ERROR = 1,//捕获到的错误  LV.1
        WARRING = 2,//运行警告  LV.2
        NOTICE = 3,//高级别提醒 LV.3
        DEBUG = 4,//普通        LV.4
    }
    /// <summary>
    /// 日志输出系统
    /// </summary>
    public class DebugUtil
    {
        /// <summary>
        /// 单例对象
        /// </summary>
        private static DebugUtil instance;
        /// <summary>
        /// 日志线程
        /// </summary>
        private Thread LogThread;
        /// <summary>
        /// 待打印的日志缓存池
        /// </summary>
        private List<LogClass> LogMessage = new List<LogClass>();
        /// <summary>
        /// 日志在控制台输出的颜色
        /// </summary>
        private Dictionary<LogType, ConsoleColor> WriteColor = new Dictionary<LogType, ConsoleColor>();
        /// <summary>
        /// 是否打印日志
        /// </summary>
        private bool IsWriteDebug = true;
        /// <summary>
        /// 打印日志到本地的数据流对象
        /// </summary>
        private StreamWriter steamWrite;
        /// <summary>
        /// 是否正在写入中
        /// </summary>
        private bool IsWrite;

        public static DebugUtil Instance {
            get {
                if (instance == null)
                    instance = new DebugUtil();
                return instance;
            }
        }

        DebugUtil() {
            //白色
            WriteColor.Add(LogType.DEBUG, ConsoleColor.White);
            //绿色
            WriteColor.Add(LogType.NOTICE, ConsoleColor.Green);
            //黄色
            WriteColor.Add(LogType.WARRING, ConsoleColor.Yellow);
            //红色
            WriteColor.Add(LogType.ERROR, ConsoleColor.Red);
            //蓝色
            WriteColor.Add(LogType.FATAL, ConsoleColor.Blue);
            //初始化一个线程函数，将Start函数以委托的形式赋予给线程
            LogThread = new Thread(new ThreadStart(Start));
            //日志线程开始启动
            LogThread.Start();
        }
        //开启线程
        void Start() {
            do
            {
                if (LogMessage.Count > 0 && !IsWrite)
                {
                    //TODO:打印日志
                    //开始打印
                    IsWrite = true;
                    //将第0个日志打印
                    WriteMessage(LogMessage[0]);
                    //删除位于第0位下标的日志
                    LogMessage.RemoveAt(0);
                }
                //打印日志时阻塞线程10毫秒
                Thread.Sleep(10);
            } while (IsWriteDebug);
        }

        public void Close()
        {
            IsWriteDebug = false;
        }

        /// <summary>
        /// 添加一个日志输出
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        public void Log(object str, LogType type = LogType.DEBUG)
        {
            if (type == LogType.FATAL) return;
            LogMessage.Add(new LogClass(str, type));
        }

        /// <summary>
        /// 添加一个日志输出
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        public void LogToTime(object str, LogType type = LogType.DEBUG)
        {
            if (type == LogType.FATAL) return;
            LogMessage.Add(new LogClass(DateTime.Now.ToString("hh:mm:ss.ff") + "     " + str , type));
        }

        /// <summary>
        /// 开始打印日志
        /// </summary>
        /// <param name="log"></param>
        void WriteMessage(LogClass log)
        {
            if (log == null)
            {
                Console.WriteLine("ERROR: Message is null");
                return;
            }
            //将控制台打印信息的颜色调整为对应日志级别的颜色
            Console.ForegroundColor = WriteColor[log.type];
            //将日志打印至控制台
            Console.WriteLine(log.msg);
            //重置下一次控制台打印信息的颜色
            Console.ResetColor();
            //开始写入本地
            WriteSteamToFold(log);
        }
        /// <summary>
        /// 将待打印的信息输出至本地,将日志永久化存储
        /// </summary>
        void WriteSteamToFold(LogClass log)
        {
            try
            {
                //获取当前程序的运行路径
                string path = FileUtil.GetRunDirectory();
                //根据类型来判定将要输出的日志的根目录
                path += "/DebugLog/" + log.type;
                //如果根目录不存在，则创建一个根目录文件夹
                FileUtil.CreateFolder(path);
                //将根目录路径和文件名组合
                path += "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                //如果文件不存在，则创建一个文件，否则加载该文件，并将文件对象赋值给流对象
                if (steamWrite == null)
                    steamWrite = !File.Exists(path) ? File.CreateText(path) : File.AppendText(path);
                //开始写入文件,在文件最后一行写入日志信息
                steamWrite.WriteLine(log.msg);
            }
            finally
            {
                if (steamWrite != null)
                {
                    //施放缓冲区资源
                    steamWrite.Flush();
                    //释放所占用资源
                    steamWrite.Dispose();
                    //重置文件写入对象
                    steamWrite = null;
                }
                IsWrite = false;
            }
        }


    }
}
