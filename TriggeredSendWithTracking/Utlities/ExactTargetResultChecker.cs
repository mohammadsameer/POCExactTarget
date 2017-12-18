using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.ETService;

namespace TriggeredSendWithTracking.Utlities
{
    public class ExactTargetResultChecker
    {
        public static void CheckResult(Result result)
        {
            if (result == null)
            {
                Console.WriteLine("Received an unexpected null result from ExactTarget");
            }

            if (result.StatusCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var triggeredResult = result as TriggeredSendCreateResult;
            var subscriberFailures = triggeredResult == null
                ? Enumerable.Empty<string>()
                : triggeredResult.SubscriberFailures.Select(f => " ErrorCode:" + f.ErrorCode + " ErrorDescription:" + f.ErrorDescription);

            Console.WriteLine(string.Format("ExactTarget response indicates failure. StatusCode:{0} StatusMessage:{1} SubscriberFailures:{2}",
                result.StatusCode,
                result.StatusMessage,
                string.Join("|", subscriberFailures)));
        }
    }
}
