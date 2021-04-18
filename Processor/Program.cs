using Processor.QueusManager;
using Processor.Workers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            Start();
            keepAlive();
        }

        private static void keepAlive()
        {
            while (true)
            {
                Console.Write("\r |");
                Task.Delay(10).Wait();
                Console.Write("\r /");
                Task.Delay(10).Wait();
                Console.Write("\r -");
                Task.Delay(10).Wait();
                Console.Write("\r \\");
            }
        }
        public static void Start()
        {
            _ = Task.WhenAll(
                RunDataProvider(),
                RunJobDispatcher(),
                RunWorkerAgent(),
                Monitor());

        }
        private static async Task RunWorkerAgent()
        {
            await Task.Run(() =>
            {
                WorkerAgent.Run();
            });
        }
        private static async Task RunJobDispatcher()
        {
            await Task.Run(() =>
            {
                JobDispatcherAgent.Run();
            });
        }
        private static async Task RunDataProvider()
        {
            await Task.Run(() =>
            {
                _ = DataProvider.DataProviderAgent.Run();
            });
        }
        private static async Task Monitor()
        {
            await Task.Run(() =>
            {
                Stopwatch sp = new Stopwatch();
                sp.Start();
                while (true)
                {
                    if (sp.Elapsed.Seconds == 1)
                    {
                        Console.Clear();
                        var msg1 = DataProvider.DataProviderAgent.Monitor();
                        var msg2 = QueuesContainer.Monitor();
                        var msg3 = WorkerAgent.Monitor();
                        Console.WriteLine($@"
{msg1}

{msg2}

{msg3}");
                        sp.Restart();
                    }


                }

            });
        }

    }

}
