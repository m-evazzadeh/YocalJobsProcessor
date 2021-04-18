using System;

namespace Processor.Config
{
    public static class Settings
    {
        /// <summary>
        /// max of jobs submitted by web api per call
        /// </summary>
        public static int ExternalSourceMaxOfJobs { get; } = 1000;
        /// <summary>
        /// Delay for retrive data from data provider for example http service
        /// </summary>
        public static TimeSpan GetDataFromDataProviderintervalTime { get; } = new TimeSpan(0, 0, 1);

        public static class WorkersConfig
        {
            /// <summary>
            /// if a job has a error retry after this time span
            /// </summary>
            public static TimeSpan RetryTime { get; } = new TimeSpan(0, 0, 3);
            /// <summary>
            /// count of worker with type express category
            /// </summary>
            public static byte CountOfWorkersExpressCategory { get; } = 5;
            /// <summary>
            /// count of worker with type normal category
            /// </summary>
            public static byte CountOfWorkersForNormalCategory { get; } = 3;
            /// <summary>
            /// count of worker with type low category
            /// </summary>
            public static byte CountOfWorkersLowCategory { get; } = 2;
        }
    }
}
