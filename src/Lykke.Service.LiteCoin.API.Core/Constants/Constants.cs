using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Core.Constants
{
    public static class Constants
    {
        public static class AssetsContants
        {
            public const string LiteCoint = "LTC";
        }

        public static class QueueNames
        {
            public const string SendCashoutStartedNotification = "send-cashout-started-notification";
            public const string SendCashoutCompletedNotification = "send-cashout-completed-notification";
            public const string SendCashinCompletedNotification = "send-cashin-completed-notification";

        }
    }
}
