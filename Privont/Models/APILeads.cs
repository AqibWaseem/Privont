using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class APILeads
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        

    }
    public class Collaborator
    {
        public int id { get; set; }
        public string name { get; set; }
        //public bool assigned { get; set; }
        public string role { get; set; }
    }

    public class Email
    {
        public string value { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public int isPrimary { get; set; }
    }

    public class Metadata
    {
        public string collection { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
        public string next { get; set; }
        public string nextLink { get; set; }
        public string notice { get; set; }
    }

    public class Person
    {
        public int id { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public string createdVia { get; set; }
        public DateTime lastActivity { get; set; }
        public string name { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string stage { get; set; }
        public int stageId { get; set; }
        public string source { get; set; }
        public int sourceId { get; set; }
        public string sourceUrl { get; set; }
        //public object leadFlowId { get; set; }
        //public bool delayed { get; set; }
        public int contacted { get; set; }
        public int price { get; set; }
        //public object assignedLenderId { get; set; }
        //public object assignedLenderName { get; set; }
        public int assignedUserId { get; set; }
        //public object assignedPondId { get; set; }
        public string assignedTo { get; set; }
        public List<string> tags { get; set; }
        public List<Email> emails { get; set; }
        public List<Phone> phones { get; set; }
        //public List<object> addresses { get; set; }
        //public object picture { get; set; }
        //public List<object> socialData { get; set; }
        public int websiteVisits { get; set; }
        //public bool claimed { get; set; }
        //public object dealStatus { get; set; }
        //public object dealStage { get; set; }
        //public object dealName { get; set; }
        //public object dealCloseDate { get; set; }
        //public object dealPrice { get; set; }
        //public bool firstToClaimOffer { get; set; }
        public List<Collaborator> collaborators { get; set; }
        //public List<object> teamLeaders { get; set; }
        //public List<object> pondMembers { get; set; }
    }

    public class Phone
    {
        public string value { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public int isPrimary { get; set; }
        public string normalized { get; set; }
        //public bool isLandline { get; set; }
        //public bool isOnboardingNumber { get; set; }
    }

    public class Root
    {
        public Metadata _metadata { get; set; }
        public List<Person> people { get; set; }
    }
}