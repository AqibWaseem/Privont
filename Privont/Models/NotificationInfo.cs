using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class NotificationInfo
    {
        public int NotificationID { get; set; }
        public DateTime NotificationDate { get; set; }
        public string NotificationTitle { get; set; }
        public bool Inactive { get; set; }
        public bool MarkAsRead { get; set; }
        public int UserID { get; set; }
        public int UserType { get; set; }
        public int EntryUserID { get; set; }
        public int EntryUserType { get; set; }

        public int InsertRecords()
        {
            int NotificationID = 0;
            try
            {
                string Query = $@"
                        insert into NotificationInfo() 
                        values (GetDate(),'{NotificationTitle}','{(Inactive?1:0)}',0,{UserID},{UserType},{EntryUserID},{EntryUserType}) ";
                General.ExecuteNonQuery(Query);
                return NotificationID;
            }
            catch
            {
                return NotificationID;
            }
        }
        public bool UpdateMarkAsRead(int NotificationID)
        {
            string Query = $@"Update NotificationInfo set MarkAsRead=1 where NotificationID=" +NotificationID;
            try
            {
                General.ExecuteNonQuery(Query);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
             
    }
}