using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.Contracts;
using TriggeredSendWithTracking.ETService;

namespace TriggeredSendWithTracking.Factory
{
    public class SoapClientFactory
    {
        public static SoapClient Manufacture(IExactTargetConfiguration config)
        {
            var client = new SoapClient(config.SoapBinding ?? "ExactTarget.Soap", config.EndPoint);
            if (client.ClientCredentials == null) return null;
            client.ClientCredentials.UserName.UserName = config.ApiUserName;
            client.ClientCredentials.UserName.Password = config.ApiPassword;
            return client;
        }
    }
}
