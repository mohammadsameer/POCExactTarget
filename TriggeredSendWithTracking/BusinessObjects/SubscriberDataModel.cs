using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggeredSendWithTracking.BusinessObjects
{
    public class SubscriberDataModel
    {
        public string SubscriberEmail { get; set; }
        public string SubscriberKey { get; set; }
        public List<KeyValuePair<string, string>> ReplacementValues { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}
