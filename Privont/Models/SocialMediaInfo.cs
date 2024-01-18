using System;
using System.Collections.Generic;
using System.Data;
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

        public DataTable GetAllRecordsInDataTable(int UserID,int UserType)
        {
            DataTable dt = General.FetchData($@"
SELECT        SocialMediaInfo.SocialMediaID, SocialMediaInfo.SocialMediaTitle, isnull(UserSocialMediaInfo.ProfileName,'')ProfileName, 
ISNULL(UserSocialMediaInfo.ProfileLink,'')as ProfileLink, ISNULL(UserSocialMediaInfo.UserID,0)UserID, 
ISNULL(UserSocialMediaInfo.UserTypeID,0)UserTypeID 
                         
FROM            SocialMediaInfo LEFT OUTER JOIN
                         UserSocialMediaInfo ON SocialMediaInfo.SocialMediaID = UserSocialMediaInfo.SocialMediaID and UserSocialMediaInfo.UserTypeID={UserType}
						 and UserSocialMediaInfo.UserID={UserID} ");
            return dt;
        }
        public List<SocialMediaInfo> GetAllRecordsInList(DataTable dt)
        {
            List<SocialMediaInfo> lst = General.ConvertDataTable<SocialMediaInfo>(dt);
            return lst;
        }
    }
}