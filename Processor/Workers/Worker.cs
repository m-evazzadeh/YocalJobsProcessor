using Processor.Models;
using System;
using System.Threading.Tasks;

namespace Processor.Workers
{
    public delegate Task ProcessJob(Job job);
    /// <summary>
    /// job processor. get a job and process it
    /// </summary>
    public class Worker
    {
        public async Task ProcessJob(Job job)
        {

            await Task.Delay(10 * (int)job.Category);
            if (job.MessageId == 30 || job.MessageId == 500 || job.MessageId == 231)//error mechanism
                throw new Exception("can't process this job");

            //do somthing
        }
    }
}
