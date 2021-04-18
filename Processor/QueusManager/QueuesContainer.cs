using Processor.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Processor.QueusManager
{
    /// <summary>
    /// get job and distpach to different channels and manage channels
    /// </summary>
    public static class QueuesContainer
    {
        public static Dictionary<QueueIdentity, Queue<Job>> Queues = new Dictionary<QueueIdentity, Queue<Job>>();

        static KeyValuePair<QueueIdentity, Queue<Job>> FindQueue(Job job)
        {
            var result = Queues
                .FirstOrDefault(x => x.Key.Entity == job.Entity);
            if (result.Key?.Entity != null)
                return result;

            AddQueue(job);

            return Queues
                .Single(x => x.Key.Entity == job.Entity);
        }

        private static void AddQueue(Job job)
        {
            var queueIdentity = new QueueIdentity() { Entity = job.Entity, IsLock = false };
            lock (Queues)
            {
                Queues.Add(queueIdentity, new Queue<Job>());
            }
        }

        public static void Enqueue(Job job)
        {
            var queue = FindQueue(job);
            lock (queue.Value)
            {
                queue.Value.Enqueue(job);
            }
        }

        public static Job DequeueJob()
        {
            lock (Queues)
            {
                var findQueue = Queues
                                .Where(x => !x.Key.IsLock && x.Value.Count > 0)
                                .ToList();

                var queue = findQueue
                            .OrderBy(x => x.Value.Peek().SentDate)
                            .OrderBy(x => x.Value.Peek().Category)
                            .FirstOrDefault();

                if (queue.Key == null || queue.Key?.Entity == null)
                    return null;

                queue.Key.IsLock = true;
                return queue.Value?.Dequeue();
            }
        }

        public static void UnLock(Entity entity)
        {
            var queue = Queues
                            .Where(x => x.Key.Entity == entity)
                            .Single();

            lock (queue.Key)
                queue.Key.IsLock = false;

        }

        public static string Monitor()
        {
            var msg = $@"total queue: {Queues.ToList().Count} {Environment.NewLine} ";

            foreach (var queue in Queues.ToList())
            {
                msg += $"queue: {queue.Key.Entity}       has: {queue.Value.Count}         IsLock: {queue.Key.IsLock}";
                msg += System.Environment.NewLine;
            }
            return msg;
        }


    }
}
