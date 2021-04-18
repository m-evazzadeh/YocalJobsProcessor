using Processor.Models;

namespace Processor.QueusManager
{
    public class QueueIdentity
    {
        public Entity Entity { get; set; }
        public bool IsLock { get; set; }
    }
}
