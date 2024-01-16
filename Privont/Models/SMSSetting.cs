using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class SMSSetting
    {
        public string SMSDetail { get; set; }
        public string SMSDetailInvite { get; set; }
        public string SMSSubKey { get; set; }
        public string SMSDetailKey { get; set; }
        public string SMSDetailSub { get; set; }
        public int UserID { get; set; }
        public int UserType { get; set; }
        public DataTable GetSMSDetailsGetSMSDetails(int UserID,int UserType)
        {
            string Query = $@"SELECT        *
FROM            SMSSetting
WHERE        (UserID = {UserID}) AND (UserType = {UserType})";
            return General.FetchData(Query);
        }
    }
}