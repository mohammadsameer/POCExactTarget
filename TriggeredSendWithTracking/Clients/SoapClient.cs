using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.BusinessObjects;


namespace TriggeredSendWithTracking.ETService
{
    public partial class SoapClient
    {
        public const string STATUS_CODE_OK = "OK";
        public const string STATUS_CODE_FAIL = "ERROR";
        public const string STATUS_CODE_ERRORS = "HAS ERROR";

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <param name="overall">The overall status returned from ET.</param>
        /// <param name="results">The result list return by ET.</param>
        /// <param name="message">The status message.</param>
        /// <returns>true if no errors occurred</returns>
        public bool GetResult(string overall, Result[] results, out string message)
        {
            string typeName = results.GetType().Name;
            List<string> resultList = new List<string>();

            switch (typeName)
            {
                case "ConfigureResult[]":
                case "DeleteResult[]":
                case "ExtractResult[]":
                case "Result[]":
                case "ScheduleResult[]":
                case "SystemStatusResult[]":
                    resultList = (
                        from r in results where r.StatusCode.ToLower().Contains("error") select r.StatusCode + ": " + r.StatusMessage
                        ).ToList();
                    break;

                default:
                    throw new ArgumentException("GetResult does not support " + typeName + ".");
            }

            string status = string.Empty;
            message = overall + "\n" + string.Join("\n", resultList);
            bool errorOccurred = message.ToLower().Contains("error");
            if (errorOccurred)
                status = Constants.ERROR_TAG;
            else
                status = Constants.SUCCESS_TAG;

            message = status + "\n" + message;

            return !errorOccurred; // true == success
        }

        public bool GetResult(string overall, PerformResult[] results, out string message)
        {
            List<string> resultList = new List<string>();

            resultList = (
                from r in results where r.StatusCode.ToLower().Contains("error") select r.Task.StatusMessage
                ).ToList();

            string status = string.Empty;
            message = overall + "\n" + string.Join("\n", resultList);
            bool errorOccurred = message.ToLower().Contains("error");
            if (errorOccurred)
                status = Constants.ERROR_TAG;
            else
                status = Constants.SUCCESS_TAG;

            message = status + "\n" + message;

            return !errorOccurred; // true == success
        }

        public bool GetResult(string overall, TriggeredSendCreateResult[] results, out string message)
        {
            List<string> resultList = new List<string>();
            var errorList = (
                from u in results where u.StatusCode.ToLower().Contains("error") select u
                ).ToList();

            var valueErrors = (
                    from e in errorList where e.SubscriberFailures != null select (from k in e.SubscriberFailures select k.Subscriber.EmailAddress + ": " + k.ErrorDescription)
                ).SelectMany(o => o).ToList();

            resultList = (from e in errorList select e.StatusMessage).ToList();
            resultList.AddRange(valueErrors);

            string status = string.Empty;
            message = overall + "\n" + string.Join("\n", resultList);
            bool errorOccurred = message.ToLower().Contains("error");
            if (errorOccurred)
                status = Constants.ERROR_TAG;
            else
                status = Constants.SUCCESS_TAG;

            message = status + "\n" + message;

            return !errorOccurred; // true == success
        }

        public bool GetResult(string overall, CreateResult[] results, out string message)
        {
            List<string> resultList = new List<string>();

            if (results[0].GetType() == typeof(DataExtensionCreateResult))
            {
                resultList = GetDataExtensionErrors(results);
            }
            else
            {
                resultList = (
                    from r in results where r.StatusCode.ToLower().Contains("error") select r.StatusCode + ": " + r.StatusMessage
                    ).ToList();
            }

            string status = string.Empty;
            message = overall + "\n" + string.Join("\n", resultList);
            bool errorOccurred = message.ToLower().Contains("error");
            if (errorOccurred)
                status = Constants.ERROR_TAG;
            else
                status = Constants.SUCCESS_TAG;

            message = status + "\n" + message;

            return !errorOccurred; // true == success
        }

        public bool GetResult(string overall, UpdateResult[] results, out string message)
        {
            List<string> resultList = new List<string>();

            Type resultType = results[0].GetType();
            var props = from p in typeof(DataExtensionUpdateResult).GetProperties() where p.Name.ToLower().Contains("error") select p;

            if (resultType == typeof(DataExtensionUpdateResult))
            {
                resultList = GetDataExtensionErrors(results);
            }
            else
            {
                resultList = (
                    from r in results where r.StatusCode.ToLower().Contains("error") select r.StatusCode + ": " + r.StatusMessage
                    ).ToList();
            }

            string status = string.Empty;
            message = overall + "\n" + string.Join("\n", resultList);
            bool errorOccurred = message.ToLower().Contains("error");
            if (errorOccurred)
                status = Constants.ERROR_TAG;
            else
                status = Constants.SUCCESS_TAG;

            message = status + "\n" + message;

            return !errorOccurred; // true == success
        }

        public bool GetResult(string overall, ScheduleResult[] results, out string message)
        {
            List<string> resultList = new List<string>();

            resultList = (
                from r in results where r.StatusCode.ToLower().Contains("error") select r.Task.StatusMessage
                ).ToList();

            string status = string.Empty;
            message = overall + "\n" + string.Join("\n", resultList);
            bool errorOccurred = message.ToLower().Contains("error");
            if (errorOccurred)
                status = Constants.ERROR_TAG;
            else
                status = Constants.SUCCESS_TAG;

            message = status + "\n" + message;

            return !errorOccurred; // true == success
        }

        public bool GetResult(string overall, APIObject[] results, out string message)
        {
            if (overall.ToLower().Contains("error"))
            {
                message = Constants.ERROR_TAG + "\n" + overall;
                return false;
            }

            if (results.Length < 1)
            {
                message = Constants.SUCCESS_TAG + "\nThe operation returned no results";
                return true;
            }

            message = Constants.SUCCESS_TAG;
            return true;
        }

        List<string> GetDataExtensionErrors<T>(T[] results) where T : Result
        {
            List<string> resultList = new List<string>();
            var props = from p in results[0].GetType().GetProperties() where p.Name.ToLower().Contains("error") select p;

            var errorList = (
                from u in results
                where u.StatusCode.ToLower().Contains("error")
                select new
                {
                    UpdateResult = u.StatusMessage,
                    Message = (from p in props where p.Name.Contains("ErrorMessage") select p.GetValue(u, null)).FirstOrDefault(),
                    Errors = (
                        from p in props
                        where p.PropertyType.Name.ToLower().Contains("dataextension")
                        select p.GetValue(u, null)
                        ).Where(p => p != null)
                }
                ).ToList();

            var dataextensionErrors =
                    (from error in
                         ((from e in errorList select e).Select(e => e.Errors).SelectMany(ei => ei))
                     select error).ToList();

            resultList = (from e in errorList select e.UpdateResult + ": " + e.Message).ToList();

            foreach (var listVar in dataextensionErrors)
            {
                DataExtensionError[] errList = (DataExtensionError[])listVar;
                foreach (DataExtensionError error in errList)
                    resultList.Add(error.Name + ": " + error.ErrorMessage);
            }
            return resultList;
        }
    }
}
