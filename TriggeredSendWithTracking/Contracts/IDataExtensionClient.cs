using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggeredSendWithTracking.Contracts
{
    public interface IDataExtensionClient
    {
        void CreateDataExtension(string dataExtensionTemplateObjectId,
            string externalKey,
            string name,
            HashSet<string> fields);

        bool DoesDataExtensionExist(string externalKey);

        string RetrieveTriggeredSendDataExtensionTemplateObjectId();
    }
}
