using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.ETService;

namespace TriggeredSendWithTracking.ETService
{
    public partial class APIObject
    {
        static Dictionary<Type, IList<string>> propertyCache = new Dictionary<Type, IList<string>>();

        public static Dictionary<Type, IList<string>> PropertyCache
        {
            get { return propertyCache; }
        }

        public static IList<string> ToCSV<T>(IList<T> apiObjects) where T : APIObject
        {
            List<string> objectList = new List<string>();

            Type t = typeof(T);
            var props = (from p in t.GetProperties()
                         where !p.Name.ToLower().Contains("specified")
                             && !p.PropertyType.Name.Contains("[]")
                             && !p.Name.Contains("Object")
                             && !p.Name.Contains("Partner")
                         //&& !p.Name.ToLower().Contains("id")
                         select p).ToList();

            var names = (from p in props select p.Name).ToList();
            String addNames = string.Join(",", names);
            addNames = "URL," + addNames;
            objectList.Add(addNames);

            List<string> values = new List<string>();
            foreach (var obj in apiObjects)
            {
                values.Clear();
                try
                {
                    TrackingEvent te = (TrackingEvent)(object)obj;
                    if (te.EventType == EventType.Click)
                    {
                        ClickEvent ce = (ClickEvent)(TrackingEvent)(object)obj;
                        values.Add(ce.URL);
                    }
                    else
                    {
                        values.Add("");
                    }
                }
                catch (Exception e)
                {
                    values.Add("");
                }

                foreach (var p in props)
                {
                    Type propType = p.PropertyType;
                    object o = p.GetValue(obj, null);
                    if (o == null)
                        values.Add(string.Empty);
                    else if (propType.Name.ToLower() == "clientid")
                    {
                        ClientID id = o as ClientID;
                        values.Add(id.ID.ToString());
                    }
                    else if ((propType.Namespace.ToLower().Contains("etapi")) && (!propType.FullName.ToLower().Contains("eventtype")))
                    {
                        try
                        {
                            ETService.APIObject apio = (ETService.APIObject)o;
                            string key = apio.CustomerKey;
                            if (key != null)
                                values.Add(key.Trim());
                            else
                                values.Add(apio.ID.ToString());
                        }
                        catch (Exception)
                        {
                            values.Add(p.PropertyType.Name);
                        }
                    }
                    else
                        values.Add(o.ToString().Trim());
                }

                objectList.Add("\"" + string.Join("\",\"", values) + "\"");
            }

            return objectList;
        }

        public static IList<string> ToTickDelimited<T>(IList<T> apiObjects) where T : APIObject
        {
            List<string> objectList = new List<string>();

            Type t = typeof(T);
            var props = (from p in t.GetProperties()
                         where !p.Name.ToLower().Contains("specified")
                             && !p.PropertyType.Name.Contains("[]")
                             && !p.Name.Contains("Object")
                             && !p.Name.Contains("Partner")
                         //&& !p.Name.ToLower().Contains("id")
                         select p).ToList();

            var names = (from p in props select p.Name).ToList();
            String addNames = string.Join("`", names);
            addNames = "URL`" + addNames;
            objectList.Add(addNames);

            List<string> values = new List<string>();
            foreach (var obj in apiObjects)
            {
                values.Clear();
                try
                {
                    TrackingEvent te = (TrackingEvent)(object)obj;
                    if (te.EventType == EventType.Click)
                    {
                        ClickEvent ce = (ClickEvent)(TrackingEvent)(object)obj;
                        values.Add(ce.URL);
                    }
                    else
                    {
                        values.Add("");
                    }
                }
                catch (Exception e)
                {
                    values.Add("");
                }

                foreach (var p in props)
                {
                    Type propType = p.PropertyType;
                    object o = p.GetValue(obj, null);
                    if (o == null)
                        values.Add(string.Empty);
                    else if (propType.Name.ToLower() == "clientid")
                    {
                        ClientID id = o as ClientID;
                        values.Add(id.ID.ToString());
                    }
                    else if ((propType.Namespace.ToLower().Contains("etapi")) && (!propType.FullName.ToLower().Contains("eventtype")))
                    {
                        try
                        {
                            ETService.APIObject apio = (ETService.APIObject)o;
                            string key = apio.CustomerKey;
                            if (key != null)
                                values.Add(key.Trim());
                            else
                                values.Add(apio.ID.ToString());
                        }
                        catch (Exception)
                        {
                            values.Add(p.PropertyType.Name);
                        }
                    }
                    else
                        values.Add(o.ToString().Trim());
                }

                objectList.Add("\"" + string.Join("\"`\"", values) + "\"");
            }

            return objectList;
        }
    }
}
