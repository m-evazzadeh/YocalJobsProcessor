using Processor.Config;
using Processor.QueusManager;
using System.Linq;
using System.Threading.Tasks;

namespace Processor.DataProvider
{
    /// <summary>
    /// agent to get data by schedule 
    /// <para>we can use Quartz.NET</para>
    /// <seealso cref="https://www.quartz-scheduler.net/"/>
    /// </summary>
    public static class DataProviderAgent
    {
        static DataProvider provider;
        public async static Task Run()
        {
            Init();
            while (true)
            {
                Task.Delay(Settings.GetDataFromDataProviderintervalTime).Wait();
                var data = await provider.GetJobs();
                if (data != null && data.Any())
                    JobsCache.PushMultiJob(data);
            }
        }

        private static void Init()
        {
            provider = new DataProvider();
            JobsCache.Clear();
        }

        public static string Monitor()
        {
            return $"JobsCache count: {JobsCache.Count()}";
        }

    }

}
