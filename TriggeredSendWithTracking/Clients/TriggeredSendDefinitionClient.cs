using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.Contracts;
using TriggeredSendWithTracking.ETService;
using TriggeredSendWithTracking.Factory;
using TriggeredSendWithTracking.Utlities;

namespace TriggeredSendWithTracking.Clients
{
    public class TriggeredSendDefinitionClient : ITriggeredSendDefinitionClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;
        private readonly ISharedRequestClient _sharedCoreRequestClient;

        public TriggeredSendDefinitionClient(IExactTargetConfiguration config)
        {
            _config = config;
            _client = SoapClientFactory.Manufacture(config);
            _sharedCoreRequestClient = new SharedRequestClient(config);
        }


        public int CreateTriggeredSendDefinition(
          string externalId,
          int emailId,
          string dataExtensionCustomerKey,
          string deliveryProfileCustomerKey,
          string name,
          string description, bool NeedCC = false, bool NeedBcc = false)
        {
            var ts = new ETService.TriggeredSendDefinition
            {
                Client = _config.ClientId.HasValue ? new ClientID { ID = _config.ClientId.Value, IDSpecified = true } : null,
                Email = new ETService.Email { ID = emailId, IDSpecified = true },
                SendSourceDataExtension = new ETService.DataExtension { CustomerKey = dataExtensionCustomerKey },
                Name = name,
                Description = description,
                CustomerKey = externalId,
                TriggeredSendStatus = TriggeredSendStatusEnum.Active,

                SendClassification = new SendClassification
                {
                    CustomerKey = "Default Transactional"
                },

                IsMultipart = true,
                IsMultipartSpecified = true,

                DeliveryProfile = new ETService.DeliveryProfile
                {
                    CustomerKey = deliveryProfileCustomerKey
                },
                IsWrapped = true,
                IsWrappedSpecified = true
            };

            if (NeedCC)
                ts.CCEmail = "%%CCAddress%%";

            string requestId, status;
            var result = _client.Create(new CreateOptions(), new APIObject[] { ts }, out requestId, out status);

            ExactTargetResultChecker.CheckResult(result.FirstOrDefault());

            return result != null ? result.First().NewID : 0;
        }

        public void StartTriggeredSend(string externalKey)
        {
            var ts = new ETService.TriggeredSendDefinition
            {
                Client = _config.ClientId.HasValue ? new ClientID { ID = _config.ClientId.Value, IDSpecified = true } : null,
                CustomerKey = externalKey,
                TriggeredSendStatus = TriggeredSendStatusEnum.Active,
                TriggeredSendStatusSpecified = true
            };

            string requestId, overallStatus;
            var result = _client.Update(new UpdateOptions(), new APIObject[] { ts }, out requestId, out overallStatus);
            ExactTargetResultChecker.CheckResult(result.FirstOrDefault());
        }

        public void UpdateTriggerSendDefinition(ETService.TriggeredSendDefinition tsd)
        {
            string requestId, overallStatus;
            var result = _client.Update(new UpdateOptions(), new APIObject[] { tsd }, out requestId, out overallStatus);
            ExactTargetResultChecker.CheckResult(result.FirstOrDefault());
        }
    }
}
