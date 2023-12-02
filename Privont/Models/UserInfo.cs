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
        public int UserType { get; set; }//UserInfo Master User = 1 , Realeastate Agent User = 2 , Lender Agent User = 3
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
    }
}