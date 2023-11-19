using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class TrueDialogIncomeMessage
    {
        public DateTime CallbackTimestamp {  get; set; }
        public string CallbackToken { get; set; }
        public int CallbackType { get { return 11; } }
        public string CallbackURL {  get; set; }
        public string TransactionId { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string MessageId { get; set; }
        public string Message { get; set; }
        public int ChannelId { get; set; }
        public string ChannelCode { get; set; }
        public int? ContactId { get; set; }
        public string PhoneNumber { get; set; }
    }
}