using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TriggeredSendWithTracking.Utlities
{
    public enum RequestQueueing
    {
        No = 0,
        Yes,
    }

    public enum Priority
    {
        Normal = 0,
        High
    }
    public enum TrackingEvent
    {
        [EnumMember]
        [Description("Contains SMTP and other information pertaining to the specific event of an email message bounce")]
        BounceEvent,
        [EnumMember]
        [Description("Contains time and date information, as well as a URL ID and a URL, regarding a click on a link contained in a message")]
        ClickEvent,
        [EnumMember]
        [Description("Contains information on a specific instance of a delivered message")]
        DeliveredEvent,
        [EnumMember]
        [Description("Indicates a subscriber used the Forward To A Friend feature to send an email to another person")]
        ForwardedEmailEvent,
        [EnumMember]
        [Description("Specifies an opt-in event related to a Forward To A Friend event")]
        ForwardedEmailOptInEvent,
        [EnumMember]
        [Description("Contains information on when email message failed to be sent")]
        NotSentEvent,
        [EnumMember]
        [Description("Contains information about the opening of a message send by a subscriber")]
        OpenEvent,
        [EnumMember]
        [Description("Contains tracking data related to a send, including information on individual subscribers")]
        SentEvent,
        [EnumMember]
        [Description("Contains information on a specific SMS message sent by a subscriber")]
        SMSMOEvent,
        [EnumMember]
        [Description("Contains information on a specific SMS message sent to a subscriber")]
        SMSMTEvent,
        [EnumMember]
        [Description("Contains information on when a survey response took place")]
        SurveyEvent,
        [EnumMember]
        [Description("Contains information regarding a specific unsubscription action taken by a subscriber")]
        UnsubEvent
    }
}
