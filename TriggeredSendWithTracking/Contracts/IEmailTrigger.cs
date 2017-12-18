using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.BusinessObjects;
using TriggeredSendWithTracking.ETService;
using TriggeredSendWithTracking.Utlities;

namespace TriggeredSendWithTracking.Contracts
{

    public interface IEmailTrigger
    {
        void Trigger(ExactTargetTriggeredEmail exactTargetTriggeredEmail, RequestQueueing requestQueueing, Utlities.Priority priority);

        void TriggerCustom(TriggeredSendDataModel exactTargetTriggeredEmail, List<Subscriber> lst, RequestQueueing requestQueueing = RequestQueueing.No, Utlities.Priority priority = Utlities.Priority.Normal);
    }
}
