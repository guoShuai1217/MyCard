using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerTools
{
    /// <summary>
    /// 任务委托
    /// </summary>
    public delegate void TimeTask();
    /// <summary>
    /// 计时器模块
    /// </summary>
    public class ScheduleUtil
    {
        //单例模式
        private static ScheduleUtil instance;
        public static ScheduleUtil Instance { get { if (instance == null) instance = new ScheduleUtil();return instance; } }
        //计时器线程
        Thread TimeThread;
        //待执行的任务
        Dictionary<int, TimeTaskModel> TaskDic = new Dictionary<int, TimeTaskModel>();
        //待移除的任务
        List<int> removelist = new List<int>();

        int Index = 0;

        ScheduleUtil()
        {
            //开始计时器线程
            TimeThread = new Thread(timeStart);
            TimeThread.Start();
        }

        void timeStart()
        {
            while (true)
            {
                //每20毫秒执行一次任务
                Thread.Sleep(20);
                CallBack();
                //获取现行刻度
                //本次刻度数 - 上次刻度数 * 10000 = 毫秒数
                //long time = DateTime.Now.Ticks;
                //int i = 0;
                //long endtime = DateTime.Now.Ticks;
                //while (i < 10)
                //{
                //    endtime = DateTime.Now.Ticks;
                //    i = (int)(endtime - time) / 10000;
                //}
                //Console.WriteLine((int)(endtime - time) / 10000);
            }
        }
        /// <summary>
        /// 执行任务回调
        /// </summary>
        void CallBack()
        {
            //线程锁,以防止数据竞争
            lock (removelist)
            {
                lock (TaskDic)
                {
                    //执行前将待移除的任务移除
                    for (int i = 0; i < removelist.Count; i++)
                        TaskDic.Remove(removelist[i]);
                    //清空待移除列表
                    removelist.Clear();
                    //获取现行时间
                    long endtime = DateTime.Now.Ticks;
                    List<int> IdKey = new List<int>(TaskDic.Keys.ToList());
                    for (int i = 0; i < IdKey.Count; i++)
                    {
                        //如果待执行的时间大于等于当前时间
                        if (TaskDic[IdKey[i]].Time <= endtime)
                        {
                            //将本任务添加至移除列表
                            removelist.Add(TaskDic[IdKey[i]].Id);
                            //执行任务
                            try
                            {
                                TaskDic[IdKey[i]].Run();
                            }
                            //抛出异常
                            catch (Exception e)
                            {
                                DebugUtil.Instance.LogToTime(e, LogType.ERROR);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 添加一个计时器任务
        /// </summary>
        /// <param name="task">待执行的任务</param>
        /// <param name="time">任务执行时间_精度毫秒</param>
        public int AddSchedule(TimeTask task,long time) {
            lock (TaskDic)
            {
                //任务Id
                Index++;
                //根据现行时间添加任务时间
                long nowtime = DateTime.Now.Ticks;
                nowtime += time * 10000;
                //创建一个新的任务
                TimeTaskModel model = new TimeTaskModel(Index, nowtime, task);
                //将任务添加至任务字典
                TaskDic.Add(Index, model);
                return Index;
            }
        }
        /// <summary>
        /// 根据任务ID移除任务
        /// </summary>
        /// <param name="taskid">任务ID</param>
        /// <returns></returns>
        public bool RemoveSchedule(int taskid)
        {
            //如果该任务已存在于待移除列表，则直接返回成功
            if (removelist.Contains(taskid))
                return true;
            //如果该任务包含在待执行列表，将该任务添加至移除列表，返回成功
            if (TaskDic.ContainsKey(taskid))
            {
                removelist.Add(taskid);
                return true;
            }
            //否则返回失败
            return false;
        }

        



    }
}
