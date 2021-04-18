using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor.Models
{
    public class Job
    {
        public DateTime SentDate { get; set; }
        public long MessageId { get; set; }
        public Entity Entity { get; set; }
        public Category Category { get; set; }
        public string Payload { get; set; }

        public override string ToString()
        {
            return $"Entity: {Entity} Category: {Category} SentDate: {SentDate}";
        }
    }
}
