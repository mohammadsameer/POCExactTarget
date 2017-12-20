using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.ETService;

namespace TriggeredSendWithTracking.Contracts
{
    public interface ISharedRequestClient
    {
        bool DoesObjectExist(string propertyName, string value, string objectType);
        string RetrieveObjectId(string propertyName, string value, string objectType);
        T RetrieveObject<T>(string propertyName, string value, string objectType);
        IList<string> GetRetrivableProperties(string type);
        IList<TrackingEvent> RetrieveTrackingEventData(Type eventType, DateTime sinceWhen, String eventTypeString, ClientID clientId = null, string TriggeredSendDefinitionObjectID = "");
    }
}
