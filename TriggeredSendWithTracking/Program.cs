﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggeredSendWithTracking.BusinessObjects;
using TriggeredSendWithTracking.Clients;
using TriggeredSendWithTracking.Contracts;
using TriggeredSendWithTracking.ETService;
using TriggeredSendWithTracking.Utlities;

namespace TriggeredSendWithTracking
{
    class Program
    {
        #region"constants"
        public static ISharedRequestClient _SharedClent { get; set; }
        public static ITriggeredSendDefinitionClient triggeredSendDefinitionClient { get; set; }
        public static IDataExtensionClient _dataExtensionClient { get; set; }
        public static IDeliveryProfileClient _deliveryProfileClient { get; set; }
        #endregion

        #region"Main                        "
        static void Main(string[] args)
        {
            TriggeredSendDataModel Mdl = new TriggeredSendDataModel()
            {
                DataExtensionExternalKey = "RIBEventTest",
                FromEmail = "sameer.mohammad@pimco.com",
                FromName = "Master Tester",
                EmailExternalKey = "RIB_Events",              // email name, id, customer key.
                                                              // EmailTemplateExternalKey = "RIB_Events",  //template name ,val
                TriggerSendDefinitionExternalKey = "RIB_EventsNew",
                // CcEmails = "Steven.Jackson@pimco.com",

                isCcNeed = true,
                isBccNeed = true

            }; // 

            List<SubscriberDataModel> Subscriberlist = new List<SubscriberDataModel>();
            var rep = new List<KeyValuePair<string, string>>();
            rep.Add(new KeyValuePair<string, string>("name", "Sameer"));
            rep.Add(new KeyValuePair<string, string>("email_subject", "Test"));
            rep.Add(new KeyValuePair<string, string>("event_markup", "test Mark up"));
            rep.Add(new KeyValuePair<string, string>("date", DateTime.Now.ToString()));
            rep.Add(new KeyValuePair<string, string>("First_Name", "Sameer"));
            rep.Add(new KeyValuePair<string, string>("html_markup", "Test"));
            rep.Add(new KeyValuePair<string, string>("view_email_url", "Test"));
            rep.Add(new KeyValuePair<string, string>("insert date", "Test"));
            rep.Add(new KeyValuePair<string, string>("manage_url", "Test"));
            rep.Add(new KeyValuePair<string, string>("CCAddress", "sam232b@gmail.com"));
            rep.Add(new KeyValuePair<string, string>("BCCAddress", "kkmir09@gmail.com"));


            Subscriberlist.Add(new SubscriberDataModel() { SubscriberEmail = "sameer.mohammad@pimco.com", SubscriberKey = "sameer.mohammad@pimco.com", ReplacementValues = rep });


            SendUsingPreDefinedKeys(Mdl, Subscriberlist);

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static void SendUsingPreDefinedKeys(TriggeredSendDataModel TriggerData, List<SubscriberDataModel> Subscriberlist)
        {
            var config = GetConfig();
            GetClient(config);

            if (!CheckIsExists(TriggerData))
            {
                throw new Exception("Dependent object not Exists");
            }
            StartTriggerSend(TriggerData);
            SendMail(TriggerData, Subscriberlist, config);
        }
        #endregion

        #region"SendMail                    "

        private static void SendMail(TriggeredSendDataModel triggerData, List<SubscriberDataModel> subscriberlist, IExactTargetConfiguration config)
        {
            var emailTrigger = new EmailTrigger(config);
            var lst = GetSubscriberList(subscriberlist, triggerData);
            emailTrigger.TriggerCustom(triggerData, lst);

        }

        #endregion

        #region"GetSubscriberList           "
        private static List<Subscriber> GetSubscriberList(List<SubscriberDataModel> subscriberlist, TriggeredSendDataModel triggerData)
        {
            List<Subscriber> lst = new List<Subscriber>();
            if (subscriberlist != null)
            {
                foreach (var sub in subscriberlist)
                {
                    var subscriber = new Subscriber
                    {
                        Addresses = new SubscriberAddress[] { new SubscriberAddress() { Address = "", AddressType = "" } },
                        EmailAddress = sub.SubscriberEmail,
                        SubscriberKey = sub.SubscriberKey ?? sub.SubscriberEmail,
                        Attributes =
                            sub.ReplacementValues.Select(value => new ETService.Attribute
                            {
                                Name = value.Key,
                                Value = value.Value
                            }).ToArray()
                    };
                    subscriber.Owner = new Owner()
                    {
                        FromAddress = sub.FromEmail ?? triggerData.FromEmail,
                        FromName = sub.FromName ?? triggerData.FromName,
                    };

                    lst.Add(subscriber);
                }
            }
            return lst;
        }


        private static void StartTriggerSend(TriggeredSendDataModel TriggerData)
        {
            try
            {
                var TS = _SharedClent.RetrieveObject<TriggeredSendDefinition>("CustomerKey", TriggerData.TriggerSendDefinitionExternalKey, "TriggeredSendDefinition");
                if (TS != null)
                {
                    if (TS.TriggeredSendStatus != TriggeredSendStatusEnum.Active)
                    {
                        triggeredSendDefinitionClient.StartTriggeredSend(TS.CustomerKey);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region"CheckIsExists               "
        private static bool CheckIsExists(TriggeredSendDataModel TriggerData)
        {
            if (TriggerData != null)
            {
                //  var isEmailTemplateExternalKey = _SharedClent.DoesObjectExist("CustomerKey", TriggerData.EmailTemplateExternalKey, "Template");
                var isDataExtension = _SharedClent.DoesObjectExist("CustomerKey", TriggerData.DataExtensionExternalKey, "DataExtension");
                var isTriggeredSendDefinition = _SharedClent.DoesObjectExist("CustomerKey", TriggerData.TriggerSendDefinitionExternalKey, "TriggeredSendDefinition");
                var isEmail = _SharedClent.DoesObjectExist("Name", TriggerData.EmailExternalKey, "Email");
                string EmailID;
                int ID = 0;

                if (isEmail)
                {
                    EmailID = _SharedClent.RetrieveObjectId("Name", TriggerData.EmailExternalKey, "Email");
                    ID = Convert.ToInt32(EmailID);
                }

                if (!isEmail || !isDataExtension)
                    return false;

                if (!isTriggeredSendDefinition)
                {
                    var dpkey = ConfigurationManager.AppSettings["DP"].ToString();
                    _deliveryProfileClient.TryCreateBlankDeliveryProfile(dpkey);

                    triggeredSendDefinitionClient.CreateTriggeredSendDefinition(
                        TriggerData.TriggerSendDefinitionExternalKey,
                        ID,
                        TriggerData.DataExtensionExternalKey,
                        dpkey,
                        TriggerData.TriggerSendDefinitionExternalKey,
                        "",
                        TriggerData.isCcNeed,
                        TriggerData.isBccNeed
                        );
                    return true;
                }
                return true;

            }
            return false;
        }

        #endregion

        #region"GetClient                   "
        private static void GetClient(IExactTargetConfiguration config)
        {
            _SharedClent = new SharedRequestClient(config);
            triggeredSendDefinitionClient = new TriggeredSendDefinitionClient(config);
            _dataExtensionClient = new DataExtensionClient(config);
            _deliveryProfileClient = new DeliveryProfileClient(config);
        }
        #endregion

        #region"GetConfig                   "
        private static IExactTargetConfiguration GetConfig()
        {
            SimpleAES ObjAes = new SimpleAES();
            // Needs to get Loaded from Config File
            return new ExactTargetConfiguration
            {
                ApiUserName = "webtech@pimco.com",   // Generic ApiUserName
                ApiPassword = ObjAes.DecryptString("133171215054227028068033180158000111090232083231"),
                EndPoint = "https://webservice.s6.exacttarget.com/Service.asmx",//  Proper End Point Required From SMS
                ClientId = 6191809
            };
        }
        #endregion
    }
}
