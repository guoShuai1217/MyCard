using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerTools
{
    /// <summary>
    /// 控制台工具
    /// </summary>
    public class ConsoleUtil
    {

        public ConsoleUtil()
        {
            //获取当前程序的运行路径
            FilePath = FileUtil.GetRunDirectory();
            //根据类型来判定将要输出的日志的根目录
            FilePath += "/DebugLog/FATAL";
        }
        #region 正常关闭
        /// <summary>
        /// 关闭事件原型
        /// </summary>
        /// <param name="ctrlType">传入的事件类型</param>
        /// <param name="ctrlType">0用户按下 Ctrl + C 来关闭当前程序</param>
        /// <param name="ctrlType">2用户按下 关闭按钮 来关闭当前程序</param>
        /// <param name="ctrlType">4用户关机来关闭当前程序</param>
        /// <returns></returns>
        public delegate bool CtrlHandlerDelegate(int ctrlType);
        /// <summary>
        /// 导入Win32 系统核心库,声明外部导入函数SetConsoleCtrlHandler
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="isAdd">是否为添加</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleCtrlHandler(CtrlHandlerDelegate callback, bool isAdd);
        /// <summary>
        /// 实现关闭程序的回调
        /// </summary>
        /// <param name="ctrlType"></param>
        /// <returns></returns>
        bool ConsoleHandler(int ctrlType) {
            switch (ctrlType)
            {
                    //CTRL + C
                case 0:
                    {
                        DebugUtil.Instance.LogToTime("程序强制关闭,按任意键继续", LogType.NOTICE);
                        ScheduleUtil.Instance.AddSchedule(delegate ()
                        {
                            DebugUtil.Instance.Close();
                        }, 1000);
                        Console.ReadKey(false);
                        //如果系统发给我们这些指令之后，仍希望处理一些事情，则返回true
                        return false;
                    }
                    //关闭按钮
                case 2:
                    {
                        DebugUtil.Instance.LogToTime("程序强制关闭,按任意键继续", LogType.NOTICE);
                        ScheduleUtil.Instance.AddSchedule(delegate ()
                        {
                            DebugUtil.Instance.Close();
                        }, 1000);
                        Console.ReadLine();
                        //如果没有事情处理，则返回false
                        return false;
                    }
                    //关机
                case 4:
                    {
                        DebugUtil.Instance.LogToTime("程序强制关闭,按任意键继续", LogType.NOTICE);
                        ScheduleUtil.Instance.AddSchedule(delegate ()
                        {
                            DebugUtil.Instance.Close();
                        }, 1000);
                        Console.ReadKey(false);
                        //如果系统发给我们这些指令之后，仍希望处理一些事情，则返回true
                        return true;
                    }
            }
            return false;
        }
        /// <summary>
        /// 注册关闭事件
        /// </summary>
        public void RegisterCtrlHandler() {
            CtrlHandlerDelegate handler = new CtrlHandlerDelegate(ConsoleHandler);
            SetConsoleCtrlHandler(handler, true);
        }
        #endregion

        #region 异常关闭
        private static StreamWriter stream;
        /// <summary>
        /// 文本输出路径
        /// </summary>
        private string FilePath = "";
        /// <summary>
        /// 打印捕捉到的错误日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void UnHandlerExceptionEventHandler(object sender,UnhandledExceptionEventArgs args)
        {
            try
            {
                string path = FilePath;
                //如果根目录不存在，则创建一个根目录文件夹
                FileUtil.CreateFolder(path);
                //将根目录路径和文件名组合
                path += "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                //如果文件不存在，则创建一个文件，否则加载该文件，并将文件对象赋值给流对象
                if (stream == null)
                    stream = !File.Exists(path) ? File.CreateText(path) : File.AppendText(path);
                //开始写入文件,在文件最后一行写入日志信息
                stream.WriteLine("*************** " + DateTime.Now.ToString("HH:mm:ss.ff") + " ***************");
                //写入事件发送者
                stream.WriteLine("Sender:" + sender);
                //写入错误信息
                stream.WriteLine("Args:" + args.ExceptionObject);
            }
            finally
            {
                if (stream != null)
                {
                    //施放缓冲区资源
                    stream.Flush();
                    //释放所占用资源
                    stream.Dispose();
                    //重置文件写入对象
                    stream = null;
                }
            }
            RunReset();
        }
        /// <summary>
        /// 执行我们的重启机制
        /// </summary>
        void RunReset()
        {
            //声明一个有参数的函数的线程
            Thread appreset = new Thread(new ParameterizedThreadStart(RunCmd));
            //获取到当前运行目录
            object runpath = Environment.CurrentDirectory + "\\";
            //将参数传入并开始执行
            appreset.Start(runpath);
            //开始尝试关闭线程
            Thread.CurrentThread.Abort();
        }
        /// <summary>
        /// 执行启动程序
        /// </summary>
        /// <param name="runpath"></param>
        void RunCmd(object runpath)
        {
            //将运行路径转换为string类型
            string path = runpath as string;
            //获取到当前的进程名
            string processname = Process.GetCurrentProcess().ProcessName + ".exe";
            Console.WriteLine("The system will reset");
            //创建一个新的进程
            Process pc = new Process();
            //赋值启动的文件路径
            pc.StartInfo.FileName = path + processname;
            //开始启动
            pc.Start();
            //开始关闭进程
            RunKill("Server.exe");
            //退出当前窗口
            Environment.Exit(0);
        }
        /// <summary>
        /// 关掉当前的进程
        /// </summary>
        /// <param name="name"></param>
        void RunKill(string name)
        {
            try
            {
                //查找进程
                foreach (Process pc in Process.GetProcessesByName(name))
                {
                    //关闭
                    pc.Kill();
                }
            }
            catch (Exception e)
            {
                
            }
        }
        #endregion
    }
}
