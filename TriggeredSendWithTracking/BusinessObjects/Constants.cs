using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggeredSendWithTracking.BusinessObjects
{
    static public class Constants
    {
        static public string ERROR_TAG { get { return "[ERROR:]"; } }
        static public string SUCCESS_TAG { get { return "[SUCCESS:]"; } }

        public const string UnknownRegion = "NONE";
        public const string DigestPubType = "DIGEST";
        public const string RIBDigestPubType = "RIB";
        public const string EmailColumnName = "Email";
        public const string SubscriberKeyColumnName = "SmsSubscriberKey";
    }
}
