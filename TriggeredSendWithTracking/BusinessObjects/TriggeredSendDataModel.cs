using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggeredSendWithTracking.BusinessObjects
{
    public class TriggeredSendDataModel
    {
        public string TriggerSendDefinitionExternalKey { get; set; }
      //  public string EmailTemplateExternalKey { get; set; }
        public string DataExtensionExternalKey { get; set; }
        public string EmailExternalKey { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }

        public bool isCcNeed { get; set; }
        public bool isBccNeed { get; set; }
    }
}
