using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.ETService;

namespace TriggeredSendWithTracking.Contracts
{
    public interface ITriggeredSendDefinitionClient
    {
        int CreateTriggeredSendDefinition(string externalId,
             int emailId,
             string dataExtensionCustomerKey,
             string deliveryProfileCustomerKey,
             string name,
             string description, bool isCCNeed = false, bool isBccNeed = false);

        void StartTriggeredSend(string externalKey);
        void UpdateTriggerSendDefinition(ETService.TriggeredSendDefinition tsd);
    }
}
