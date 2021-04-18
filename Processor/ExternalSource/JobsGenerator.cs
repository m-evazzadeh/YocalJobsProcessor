using Processor.Config;
using Processor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Processor.ExternalSource
{
    /// <summary>
    /// this is like web api 
    /// </summary>
    public class JobsGenerator
    {
        readonly int rangeOfEntity = (int)Enum.GetValues(typeof(Entity)).Cast<Entity>().Last() + 1;
        readonly int rangeOfCategory = (int)Enum.GetValues(typeof(Category)).Cast<Category>().Last() + 1;
        public async Task<IEnumerable<Job>> GetJobs()
        {
            Random r = new Random();
            var counter = r.Next(1, Settings.ExternalSourceMaxOfJobs);
            List<Job> result = new List<Job>();
            for (int i = 0; i < counter; i++)
            {
                result.Add(await GetJob(DateTime.Now, r, i));
            }
            return await Task.FromResult(result);
        }

        private Task<Job> GetJob(DateTime SentDate, Random r, long messageId)
        {
            return Task.FromResult(
            new Job()
            {
                SentDate = SentDate,
                MessageId = messageId,
                Entity = (Entity)r.Next(0, rangeOfEntity),
                Category = (Category)r.Next(0, rangeOfCategory),
                Payload = r.Next(1, 1000).ToString("D4"),
            });
        }
    }
}
