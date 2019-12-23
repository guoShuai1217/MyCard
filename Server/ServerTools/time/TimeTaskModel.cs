using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools
{
    /// <summary>
    /// 计时器任务模型
    /// </summary>
    public class TimeTaskModel
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public int Id;
        /// <summary>
        /// 执行时间  单位毫秒
        /// </summary>
        public long Time;
        /// <summary>
        /// 任务事件
        /// </summary>
        private TimeTask Event;

        public TimeTaskModel() { }
        public TimeTaskModel(int Id, long Time, TimeTask task) {
            this.Id = Id;
            this.Time = Time;
            this.Event = task;
        }

        /// <summary>
        /// 执行函数
        /// </summary>
        public void Run()
        {
            Event();
        }
    }
}
