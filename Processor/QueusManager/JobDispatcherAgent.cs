namespace Processor.QueusManager
{
    /// <summary>
    /// agent to get jobs (message) from cashe and send to dispatcher
    /// </summary>
    public static class JobDispatcherAgent
    {
        public static void Run()
        {
            while (true)
            {
                if (JobsCache.Count() > 0)
                {
                    var job = JobsCache.Dequeue();
                    if (job != null)
                        QueuesContainer.Enqueue(job);
                }
            }

        }
    }
}
