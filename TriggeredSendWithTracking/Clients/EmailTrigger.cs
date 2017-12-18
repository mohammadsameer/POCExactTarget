using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.BusinessObjects;
using TriggeredSendWithTracking.Contracts;
using TriggeredSendWithTracking.ETService;
using TriggeredSendWithTracking.Factory;
using TriggeredSendWithTracking.Utlities;

namespace TriggeredSendWithTracking.Clients
{
    public class EmailTrigger
    {
        private readonly IExactTargetConfiguration _config;

        public EmailTrigger(IExactTargetConfiguration config)
        {
            _config = config;
        }

        public void TriggerCustom(TriggeredSendDataModel exactTargetTriggeredEmail, List<Subscriber> lst, RequestQueueing requestQueueing = RequestQueueing.No, Utlities.Priority priority = Utlities.Priority.Normal)
        {
            var clientId = _config.ClientId;
            var client = SoapClientFactory.Manufacture(_config);

            var tsd = new TriggeredSendDefinition
            {
                Client = clientId.HasValue ? new ClientID { ID = clientId.Value, IDSpecified = true } : null,
                CustomerKey = exactTargetTriggeredEmail.TriggerSendDefinitionExternalKey,
            };

            var ts = new TriggeredSend
            {
                Client = clientId.HasValue ? new ClientID { ID = clientId.Value, IDSpecified = true } : null,
                TriggeredSendDefinition = tsd,
                Subscribers = lst.ToArray(),
            };

            var co = new CreateOptions
            {
                RequestType = requestQueueing == RequestQueueing.No ? RequestType.Synchronous : RequestType.Asynchronous,
                RequestTypeSpecified = true,
                QueuePriority = priority == Utlities.Priority.High ? ETService.Priority.High : ETService.Priority.Medium,
                QueuePrioritySpecified = true
            };

            string requestId, status;
            var result = client.Create(
                co,
                new APIObject[] { ts },
                out requestId, out status);

            ExactTargetResultChecker.CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject
        }
    }
}
