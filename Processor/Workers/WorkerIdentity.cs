using Processor.Models;

namespace Processor.Workers
{
    public class WorkerIdentity
    {
        public string Id { get; set; }
        public Category Category { get; set; }
        public bool IsBusy { get; set; }

    }
}
