using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Privont.Models
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
        public int UserType { get; set; }//UserInfo Master User = 1 , Realeastate Agent User = 2 , Lender Agent User = 3 , Lead = 4 , Vendor = 5
        public DataTable GetAllRecord_DataTable(string WhereClause = "")
        {
            DataTable dt = General.FetchData($@" select UserID,UserName,Inactive,1 as UserType from UserInfo {WhereClause} 
union
Select (RealEstateAgentId)UserID,UserName,Inactive,2 as UserType from RealEstateAgentInfo {WhereClause} union 
Select (LenderId)UserID,UserName,Inactive,3 as UserType from LenderInfo {WhereClause}   ");
            return dt;
        }
        public DataTable GetUserAllRecord_DataTable(string WhereClause = "")
        {
            DataTable dt = General.FetchData($@" Select * from UserInfo {WhereClause}   ");
            return dt;
        }
        public int InsertRecord(UserInfo obj)
        {
            int UserID = 0;
            string Query = $@"Insert into UserInfo values('{obj.UserName}','{obj.Password}','{obj.Name}',{obj.UserType},{(obj.Inactive ? 1 : 0)})";
            Query = Query + $@"SELECT SCOPE_IDENTITY() as UserID";
            try
            {
                DataTable dt = General.FetchData(Query);
                int.TryParse(dt.Rows[0]["UserID"].ToString(), out UserID);
            }
            catch (Exception ex)
            {

            }
            return UserID;
        }
        public int UpdateRecord(UserInfo obj)
        {
            int UserID = 0;
            string Query = $@"UPDATE UserInfo
   SET [UserName] = '{obj.UserName}'
      ,[Password] = '{obj.Password}'
      ,[Inactive] = {(obj.Inactive ? 1 : 0)}
      ,[Name] = '{obj.Name}'
      ,[UserType] = {obj.UserType}
 WHERE UserID=" + obj.UserID;
            try
            {
                General.ExecuteNonQuery(Query);
                UserID = obj.UserID;
            }
            catch (Exception ex)
            {
            }
            return UserID;
        }
        public DataTable GetAllRecordsViaUserIDandUserType(int UserID,int UserType)
        {
            string Query = $@"SELECT        UserDetail.UserID, UserDetail.FirstName, UserDetail.LastName, UserDetail.EmailAddress, UserDetail.Contact1, UserDetail.IsApproved, UserDetail.IsEmailVerified, UserDetail.UniqueIdentifier, UserDetail.SourceID, 
                         UserDetail.PriceRangeID, UserDetail.State, UserDetail.FirstTimeBuyer, UserDetail.IsMilitary, UserDetail.TypeID, UserDetail.BestTimeToCall, UserDetail.UserType, LeadPricePoint.PricePoint,  ReferTypeInfo.TypeTitle
FROM            

(SELECT        RealEstateAgentId AS UserID, FirstName, LastName, EmailAddress, Contact1, IsApproved, IsEmailVerified, UniqueIdentifier, SourceID, PriceRangeID, State, FirstTimeBuyer, IsMilitary, TypeID, BestTimeToCall, 
                                                    2 AS UserType
                          FROM            RealEstateAgentInfo
                          UNION ALL
                          SELECT        LenderId AS UserID, FirstName, LastName, EmailAddress, Contact1, IsApproved, IsEmailVerified, UniqueIdentifier, SourceID, PriceRangeID, State, FirstTimeBuyer, IsMilitary, TypeID, BestTimeToCall, 
                                                   3 AS UserType
                          FROM            LenderInfo
                          UNION ALL
                          SELECT        LeadID AS UserID, FirstName, LastName, EmailAddress, Contact1, IsApproved, IsEmailVerified, UniqueIdentifier, SourceID, PriceRangeID, State, FirstTimeBuyer, IsMilitary, TypeID, BestTimeToCall, 
                                                   4 AS UserType
                          FROM            LeadInfo
                          UNION ALL
                          SELECT        VendorID AS UserID, FirstName, LastName, EmailAddress, Contact1, IsApproved, IsEmailVerified, UniqueIdentifier, SourceID, PriceRangeID, State, FirstTimeBuyer, IsMilitary, TypeID, BestTimeToCall, 
                                                   5 AS UserType
                          FROM            VendorInfo) AS UserDetail LEFT OUTER JOIN
                         LeadPricePoint ON LeadPricePoint.PricePointID = UserDetail.PriceRangeID LEFT OUTER JOIN
                         ReferTypeInfo ON ReferTypeInfo.TypeID = UserDetail.TypeID
WHERE        (UserDetail.UserID = {UserID}) AND (UserDetail.UserType = {UserType})";
            DataTable dt = General.FetchData(Query);
            return dt;
        }
    }
}