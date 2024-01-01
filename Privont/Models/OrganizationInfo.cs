using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class OrganizationInfo
    {
        #region Model
        public int OrganizationID { get; set; }
        public string OrganizationTitle { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string RegistrationNo { get; set; }
        public int ZIPCodeID { get; set; }
        public string OfficeNo { get; set; }
        public string StreetNo { get; set; }
        public string StreetName { get; set; }
        public string Website { get; set; }
        public string EmailAddress { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public string Remarks { get; set; }
        public bool Inactive { get; set; }
        public int OrganizationType { get; set; }
        #endregion


    }
}