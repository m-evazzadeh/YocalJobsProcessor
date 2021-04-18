using Processor.ExternalSource;
using Processor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.DataProvider
{
    /// <summary>
    /// this is adapert to get data from 3rd party  (for example web api)
    /// </summary>
    public class DataProvider
    {
        public async Task<IEnumerable<Job>> GetJobs()
        {
            return await new JobsGenerator().GetJobs();
        }
    }

}
