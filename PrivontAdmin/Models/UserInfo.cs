using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace PrivontAdmin.Models
{
    public class UserInfo
    {
        public int UserID { get; set; }
        [Required]

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }
        public bool Inactive { get; set; }
        public int UserType { get; set; }//UserInfo Master User = 1 , Realeastate Agent User = 2 , Lender Agent User = 3

    }
}