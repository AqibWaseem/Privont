using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Web;

namespace Privont.Models
{
    public class CustomerSquareup
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string email_address { get; set; }
        public string phone_number { get; set; }
        public string company_name { get; set; }
        public Preferences preferences { get; set; }
        public string creation_source { get; set; }
        public int version { get; set; }
    }
    public class Preferences
    {
        public bool email_unsubscribed { get; set; }
    }
}