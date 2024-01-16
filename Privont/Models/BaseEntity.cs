using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LicenseNo { get; set; }
        public int OrganizationID { get; set; }
        public int ZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string OfficeNo { get; set; }
        public string StreetName { get; set; }
        public string StreetNo { get; set; }
        public string Website { get; set; }
        public string EmailAddress { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public string Remarks { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public bool Inactive { get; set; }
        public int UserID { get; set; }
        public int UserType { get; set; }
        public string OrganizationTitle { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedRemarks { get; set; }
        public int IsEmailVerified { get; set; }
        public int UniqueIdentifier { get; set; }
        public int SourceID { get; set; }
        public int PriceRangeID { get; set; }
        public string State { get; set; }
        public int FirstTimeBuyer { get; set; }
        public int IsMilitary { get; set; }
        public int TypeID { get; set; }
        public int BestTimeToCall { get; set; }
        
    }
}