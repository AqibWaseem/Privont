using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models.CardAuthentication
{
    public class Card
    {
        public string Name { get; set; }
        public long Number { get; set; }
        public string Exp { get; set; }
        public string Cvv { get; set; }
        public Address Address { get; set; }
    }
    public class Address
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
    }
    public class TransactionData
    {
        public int Id { get; set; }
        public string Amount { get; set; }
        public string Source { get; set; }
        public int Level { get; set; }
        public Card Card { get; set; }
    }
}