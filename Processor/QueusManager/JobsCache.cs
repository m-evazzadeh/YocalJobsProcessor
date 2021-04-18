using Processor.Models;
using System.Collections.Generic;
using System.Linq;

namespace Processor.QueusManager
{
    /// <summary>
    /// cache all recive jobs to queue
    /// <para>we can use message broker like MSMQ or Rabbitmq or Kafka</para>
    /// </summary>
    public static class JobsCache
    {
        static Queue<Job> catche { get; set; } = new Queue<Job>();

        public static void Clear()
        {
            catche.Clear();
        }

        public static void PushMultiJob(IEnumerable<Job> jobs)
        {
            foreach (var job in jobs.OrderBy(x => x.SentDate).ToList())
            {
                Enqueue(job);
            }
        }

        public static void Enqueue(Job job)
        {
            lock (catche)
                catche.Enqueue(job);

        }
        public static Job Dequeue()
        {
            if (Count() > 0)
                return catche.Dequeue();

            return null;
        }

        public static Job Peek()
        {
            if (Count() > 0)
                return catche.Peek();

            return null;
        }

        public static int Count()
        {
            return catche.Count();
        }

    }
}
