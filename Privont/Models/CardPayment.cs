using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class CardPayment
    {
        public Terminal terminal { get; set; }
        public string amount { get; set; }
        public string source { get; set; }
        public int level { get; set; }
        public Card card { get; set; }
        public Contact contact { get; set; }
        public string sendReceipt { get; set; }
        public CardValidate _CardValidate { get; set; }

    }
   
    public class Terminal
    {
        public int id { get; set; }
    }

    public class Address
    {
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string zip { get; set; }
    }

    public class Card
    {
        public string name { get; set; }
        public long number { get; set; }
        public string exp { get; set; }
        public int cvv { get; set; }
        public Address address { get; set; }
    }

    public class Contact
    {
        public string email { get; set; }
    }
    public class CardValidate
    {
        public string total { get; set; } // You can replace 'object' with the specific data type you're expecting
        public string sequence { get; set; } // You can replace 'object' with the specific data type you're expecting
    }
}