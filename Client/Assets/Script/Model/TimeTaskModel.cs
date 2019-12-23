using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class TimeTaskModel
    {
        public int Id;
        public long Time;
        public TimeManager.TaskEvent Event;

        public TimeTaskModel() { }
        public TimeTaskModel(int Id, long Time, TimeManager.TaskEvent Event)
        {
            this.Id = Id;
            this.Time = Time;
            this.Event = Event;
        }

        public void Run() {
            Event();
        }
    }
}
