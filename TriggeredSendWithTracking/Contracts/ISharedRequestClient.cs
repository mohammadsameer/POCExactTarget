using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggeredSendWithTracking.Contracts
{
    public interface ISharedRequestClient
    {
        bool DoesObjectExist(string propertyName, string value, string objectType);
        string RetrieveObjectId(string propertyName, string value, string objectType);
        T RetrieveObject<T>(string propertyName, string value, string objectType);
    }
}
