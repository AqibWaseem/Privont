using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class TrueDialogIncomeMessage
    {
        public DateTime CallbackTimestamp { get; set; }
        public Guid CallbackToken { get; set; }
        public int CallbackType { get; set; }
        public string CallbackURL { get; set; }
        public Guid TransactionId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public Guid MessageId { get; set; }
        public string Message { get; set; }
        public int ChannelId { get; set; }
        public string ChannelCode { get; set; }
        public int? ContactId { get; set; }
        public string PhoneNumber { get; set; }
    }
}