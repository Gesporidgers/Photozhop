﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photozhop.Utility
{
    class TaskQueue
    {
        private List<Task> tasks = new List<Task>();
        private Task queue;
        
        private void Queue()
        {
            while (true)
            {
                if (tasks.Count > 0)
                {
                    Task t = tasks.Last();
                    int i = tasks.IndexOf(t);

                    t.RunSynchronously();

                    tasks.Clear();
                }
                if (tasks.Count == 0)
                    queue.Wait(1);
            }
        }

        public void StartQueue()
        {
            queue = new Task(Queue);
            queue.Start();
        }

        public void AddTask(Task tt)
        {
            tasks.Add(tt);
        }
    }
}
