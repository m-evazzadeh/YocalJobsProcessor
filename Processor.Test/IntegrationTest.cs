using Microsoft.VisualStudio.TestTools.UnitTesting;
using Processor.Models;
using Processor.QueusManager;
using Processor.Workers;
using System;
using System.Threading.Tasks;

namespace Processor.Test.QueusManager
{
    [TestClass]
    public class IntegrationTest
    {
        [TestMethod]
        public void Run()
        {
            JobsCache.Clear();

            var job1 = new Job { MessageId = 10, Category = Category.Express, Entity = Entity.Product0, Payload = "10", SentDate = DateTime.Now };
            var job2 = new Job { MessageId = 11, Category = Category.Express, Entity = Entity.Product1, Payload = "11", SentDate = DateTime.Now.AddSeconds(1) };
            var job3 = new Job { MessageId = 12, Category = Category.Normal, Entity = Entity.Product2, Payload = "12", SentDate = DateTime.Now.AddSeconds(2) };
            var job4 = new Job { MessageId = 30, Category = Category.Express, Entity = Entity.Product3, Payload = "13", SentDate = DateTime.Now.AddSeconds(3) };

            Assert.IsNull(JobsCache.Peek());

            JobsCache.Enqueue(job1);
            Assert.IsTrue(JobsCache.Count() == 1);
            Assert.IsTrue(JobsCache.Peek().ToString() == job1.ToString());

            JobsCache.Enqueue(job2);
            Assert.IsTrue(JobsCache.Count() == 2);
            Assert.IsFalse(JobsCache.Peek().ToString() == job2.ToString());

            Assert.IsTrue(QueuesContainer.Queues.Count == 0);

            _ = Task.Run(() => JobDispatcherAgent.Run());
            while (QueuesContainer.Queues.Count != 2) { }
            Assert.IsTrue(QueuesContainer.Queues.Count == 2);


            _ = Task.Run(() => WorkerAgent.Run());
            while (WorkerAgent.doneCount != 2) { }
            Assert.IsTrue(WorkerAgent.doneCount == 2);


            JobsCache.Enqueue(job3);
            while (QueuesContainer.Queues.Count != 3) { }
            Assert.IsTrue(QueuesContainer.Queues.Count == 3);
            while (WorkerAgent.doneCount != 3) { }
            Assert.IsTrue(WorkerAgent.doneCount == 3);


            JobsCache.Enqueue(job4);
            while (QueuesContainer.Queues.Count != 4) { }
            Assert.IsTrue(QueuesContainer.Queues.Count == 4);
            while (WorkerAgent.ErrorList.Count != 1) { }
            Assert.IsTrue(WorkerAgent.ErrorList.Count == 1);
        }

    }
}
