using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.Contracts;
using TriggeredSendWithTracking.ETService;
using TriggeredSendWithTracking.Factory;

namespace TriggeredSendWithTracking.Clients
{
    public class SharedRequestClient : ISharedRequestClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;

        public SharedRequestClient(IExactTargetConfiguration config)
        {
            _client = SoapClientFactory.Manufacture(config);
            _config = config;
        }

        public bool DoesObjectExist(string propertyName, string value, string objectType)
        {
            var properties = GetRetrivableProperties(objectType);

            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                    ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                    : null,
                ObjectType = objectType,
                Properties = properties.ToArray(),

                Filter = new SimpleFilterPart
                {
                    Property = propertyName,
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { value }
                }
            };

            string requestId;
            APIObject[] results;

            _client.Retrieve(request, out requestId, out results);

            return results != null && results.Any();
        }

        public string RetrieveObjectId(string propertyName, string value, string objectType)
        {

            var properties = GetRetrivableProperties(objectType);

            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                            ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                            : null,
                ObjectType = objectType,
                Properties = properties.ToArray(),
                Filter = new SimpleFilterPart
                {
                    Property = propertyName,
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { value }
                }
            };

            string requestId;
            APIObject[] results;

            _client.Retrieve(request, out requestId, out results);

            if (results != null && results.Any())
            {
                if (objectType == "Email")
                    return Convert.ToString(results.First().ID);

                return results.First().ObjectID;
            }

            return string.Empty;
        }

        public T RetrieveObject<T>(string propertyName, string value, string objectType)
        {
            var properties = GetRetrivableProperties(objectType);
            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                            ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                            : null,
                ObjectType = objectType,
                Properties = properties.ToArray(),
                Filter = new SimpleFilterPart
                {
                    Property = propertyName,
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { value }
                }
            };

            string requestId;
            APIObject[] results;

            _client.Retrieve(request, out requestId, out results);

            if (results != null && results.Any())
            {
                return (T)(object)results.First();
            }
            return default(T);
        }

        public IList<TrackingEvent> RetrieveTrackingEventData(Type eventType, DateTime sinceWhen, String eventTypeString, ClientID clientId = null, string TriggeredSendDefinitionObjectID = "")
        {

            //String filterField = "CreatedDate";
            String filterField = "EventDate";
            var properties = GetRetrivableProperties(eventTypeString).ToArray();


            SimpleFilterPart filter = new SimpleFilterPart();
            //Use this only if you are retrieving for TriggeredSend
            filter.Property = "TriggeredSendDefinitionObjectID";
            String[] vlaues = { TriggeredSendDefinitionObjectID };


            //filter.Property = "SendID";
            //String[] vlaues = { "28980" };
            filter.Value = vlaues;

            var dateFilter = new SimpleFilterPart
            {
                Property = filterField,
                SimpleOperator = SimpleOperators.greaterThanOrEqual,
                DateValue = new DateTime[] {
                        sinceWhen
                    }
            };

            ComplexFilterPart cfilter = new ComplexFilterPart();
            cfilter.LeftOperand = filter;
            cfilter.LogicalOperator = LogicalOperators.AND;
            cfilter.RightOperand = dateFilter;


            RetrieveRequest retrieveRequest = new RetrieveRequest
            {
                ObjectType = eventType.Name,
                Properties = properties,
                Filter = cfilter,
                ClientIDs = new ClientID[] { clientId }
            };

            APIObject[] results = null;
            String requestId = null, message;

            SoapClient client = _client;

            String status = "";
            IList<TrackingEvent> returnList = new List<TrackingEvent>();
            do
            {
                status = client.Retrieve(retrieveRequest, out requestId, out results);

                if (!client.GetResult(status, results, out message))
                {
                    //WriteToLog(PIMCO.SMS.Logging.Category.Exception, message);
                    //throw new ExactTargetException(message);
                }

                for (int i = 0; i < results.Length; i++)
                {
                    returnList.Add((TrackingEvent)results[i]);
                }

                //This calls the API again to get the next 2500 records
                retrieveRequest = new RetrieveRequest();
                retrieveRequest.ContinueRequest = requestId;

            } while (status.Equals("MoreDataAvailable"));

            return returnList.ToList();
        }

        public IList<string> GetRetrivableProperties(string type)
        {
            Type apiObjectType = typeof(ETService.APIObject);
            Type et = Type.GetType(apiObjectType.Namespace + "." + type);

            string[] excludedProperties = new string[] {
                "IsHTMLPaste"
            };
            string requestID;
            ObjectDefinitionRequest objDefs = new ObjectDefinitionRequest();
            objDefs.ObjectType = et.Name;

            ObjectDefinition[] definitions = _client.Describe(new ObjectDefinitionRequest[] { objDefs }, out requestID);


            var retrievables = (
                from p in definitions[0].Properties
                where p.IsRetrievableSpecified && p.IsRetrievable
                select p.Name
                ).Except(excludedProperties).ToList();

            return retrievables;
        }
    }
}
