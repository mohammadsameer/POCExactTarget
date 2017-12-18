using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggeredSendWithTracking.Contracts
{
    public interface IDeliveryProfileClient
    {
        string TryCreateBlankDeliveryProfile(string externalKey);
    }
}
