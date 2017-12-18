using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggeredSendWithTracking.Contracts
{
    public interface IExactTargetConfiguration
    {
        string EndPoint { get; set; }
        int? ClientId { get; set; }
        string ApiUserName { get; set; }
        string ApiPassword { get; set; }
        string SoapBinding { get; set; }
    }
}
