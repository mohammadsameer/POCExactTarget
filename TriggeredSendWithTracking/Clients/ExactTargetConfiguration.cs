using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.Contracts;

namespace TriggeredSendWithTracking.Clients
{
    public class ExactTargetConfiguration : IExactTargetConfiguration
    {
        public string EndPoint { get; set; }
        public int? ClientId { get; set; }
        public string ApiUserName { get; set; }
        public string ApiPassword { get; set; }
        public string SoapBinding { get; set; }
    }
}
