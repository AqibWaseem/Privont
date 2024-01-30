using Square.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class CardInfo : TransactionDetails
    {
        public int CardID { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ExpCardDate { get; set; }
        public int CVV { get; set; }
        public int CardTypeID { get; set; }
        //public int UserID { get; set; }
        //public int UserType { get; set; }
        //public decimal Amount { get; set; }

    }
}