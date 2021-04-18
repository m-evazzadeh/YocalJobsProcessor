using Processor.Models;
using Processor.QueusManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Processor.Config.Settings;

namespace Processor.Workers
{
    /// <summary>
    /// coordinator between queues and worker
    /// </summary>
    public static class WorkerAgent
    {
        public static Dictionary<WorkerIdentity, ProcessJob> WorkerContainer = new Dictionary<WorkerIdentity, ProcessJob>();
        public static List<Job> ErrorList = new List<Job>();
        public static long doneCount = 0;
        static void Init()
        {
            if (WorkerContainer.Any(x => x.Key.IsBusy))
                throw new Exception("worker is working now !!!");

            WorkerContainer.Clear();

            #region init workers for express category
            for (int i = 0; i < WorkersConfig.CountOfWorkersExpressCategory; i++)
            {
                var workerIdentity = new WorkerIdentity()
                {
                    Id = $"{Category.Express.ToString()}{i}",
                    Category = Category.Express,
                    IsBusy = false
                };
                WorkerContainer.Add(workerIdentity, new Worker().ProcessJob);
            }
            #endregion

            #region init workers for normal category
            for (int i = 0; i < WorkersConfig.CountOfWorkersForNormalCategory; i++)
            {
                var workerIdentity = new WorkerIdentity()
                {
                    Id = $"{Category.Normal.ToString()}{i}",
                    Category = Category.Normal,
                    IsBusy = false
                };
                WorkerContainer.Add(workerIdentity, new Worker().ProcessJob);
            }
            #endregion

            #region init workers for low category
            for (int i = 0; i < WorkersConfig.CountOfWorkersLowCategory; i++)
            {
                var workerIdentity = new WorkerIdentity()
                {
                    Id = $"{Category.Low.ToString()}{i}",
                    Category = Category.Low,
                    IsBusy = false
                };
                WorkerContainer.Add(workerIdentity, new Worker().ProcessJob);
            }
            #endregion
        }
        public static void Run()
        {
            Init();

            if (WorkerContainer == null || !WorkerContainer.Any())
                throw new Exception("there are not any worker");

            while (true)
            {
                var job = QueuesContainer.DequeueJob();
                if (job != null)
                {
                    #region find worker
                    var worker = new KeyValuePair<WorkerIdentity, ProcessJob>();
                    while (worker.Key == null)
                    {
                        worker = WorkerContainer
                                .Where(x => !x.Key.IsBusy && x.Key.Category == job.Category)
                                .FirstOrDefault();

                        if (worker.Key == null)
                            worker = WorkerContainer
                                        .Where(x => !x.Key.IsBusy)
                                        .FirstOrDefault();
                    }
                    #endregion
                    if (worker.Key != null)
                    {
                        worker.Key.IsBusy = true;
                        var doing = worker.Value.Invoke(job);
                        _ = doing.ContinueWith(t =>
                            {
                                if (t.IsCompleted)
                                {
                                    worker = DoneJob(job, worker);
                                }
                                if (t.IsFaulted)
                                {
                                    #region retry
                                    try
                                    {
                                        Task.Delay(Config.Settings.WorkersConfig.RetryTime).Wait();
                                        t.Wait();
                                        worker = DoneJob(job, worker);
                                    }
                                    catch
                                    {
                                        ErrorJob(job, worker);
                                    }
                                    #endregion
                                }
                            });
                    }
                }
            }

        }

        private static void ErrorJob(Job job, KeyValuePair<WorkerIdentity, ProcessJob> worker)
        {
            QueuesContainer.UnLock(job.Entity);
            worker.Key.IsBusy = false;
            LogError(job);
        }

        private static KeyValuePair<WorkerIdentity, ProcessJob> DoneJob(Job job, KeyValuePair<WorkerIdentity, ProcessJob> worker)
        {
            QueuesContainer.UnLock(job.Entity);
            worker.Key.IsBusy = false;
            LogDone(job);
            return worker;
        }

        private static void LogError(Job job)
        {
            ErrorList.Add(job);
        }

        private static void LogDone(Job job)
        {
            ++doneCount;
        }
        public static string Monitor()
        {

            var msg = $@"total worker: {WorkerContainer.Count} {Environment.NewLine} ";
            foreach (var worker in WorkerContainer.ToList())
            {
                msg += $@"worker: {worker.Key.Id}       isBusy: {worker.Key.IsBusy} {Environment.NewLine}";

            }
            msg += $"{Environment.NewLine} done:  {doneCount}";
            msg += $"{Environment.NewLine} error: {ErrorList.Count}";

            return msg;
        }
    }

}
