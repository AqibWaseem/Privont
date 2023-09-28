using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class APIConfigInfo
    {
        public int APIConfigID { get; set; }
        public int RealEstateID { get; set; }
        public int TypeID { get; set; } //Follow up Boss=1, Zillow=2
        public string APITypeTitle { get; set; }
        public string APIConfig { get; set; }

        public void InsertRecords()
        { 
            string Query = $@"INSERT INTO APIConfigInfo
           (RealEstateID
           ,TypeID
           ,APIConfig)
     VALUES
           ({General.UserID}
           ,{TypeID}
           ,'{APIConfig}')";
            General.ExecuteNonQuery(Query);
        }
        public void DeleteRecords()
        {
            General.ExecuteNonQuery($@" delete from APIConfigInfo where RealEstateID=" + General.UserID);
        }
    }
}