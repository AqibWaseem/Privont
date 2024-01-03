using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class SocialMediaInfo:UserSocialMediaInfo
    {
        public int SocialMediaID { get; set; }
        public string SocialMediaTitle { get; set; }
        public bool Inactive { get; set; }
        public string Remarks { get; set; }

    }
}