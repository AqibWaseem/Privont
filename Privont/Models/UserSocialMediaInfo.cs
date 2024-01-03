using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class UserSocialMediaInfo
    {
        public int UserSocialID { get; set; }
        public int SocialMediaID { get; set; }
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public string ProfileName { get; set; }
        public string ProfileLink { get; set; }

    }
}