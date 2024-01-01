using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class LenderInfo
    {
        public int LenderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LicenseNo { get; set; }
        public int OrganizationID { get; set; }
        public int ZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string OfficeNo { get; set; }
        public string StreetName { get; set; }
        public string StreetNo { get; set; }
        public string Website { get; set; }
        public string EmailAddress { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public string Remarks { get; set; }
        public string Password { get; set; }
        public string username { get; set; }
        public bool Inactive { get; set; }
        public int UserID { get; set; }
        public int UserType { get; set; }
        public string OrganizationTitle { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedRemarks { get; set; }
        public DataTable GetAllRecord(string WhereClause = "")
        {
            DataTable dataTable = new DataTable();
            dataTable = General.FetchData($@"Select LenderInfo.*,OrganizationInfo.OrganizationTitle,ZipCode.ZipCode from LenderInfo 
left outer join OrganizationInfo on LenderInfo.OrganizationID = OrganizationInfo.OrganizationID
left outer join ZipCode on LenderInfo.ZipCodeID = ZipCode.ZipCodeID

 Where UserName is not null " + WhereClause);
            return dataTable;
        }
        public DataTable GetAllRecordforSignup(string WhereClause = "")
        {
            DataTable dataTable = new DataTable();
            dataTable = General.FetchData($@"Select LenderInfo.* from LenderInfo 
  " + WhereClause);
            return dataTable;
        }
        public int InsertRecord(LenderInfo obj)
        {
            int LenderId = 0;
            string Query = $@"INSERT INTO [dbo].[LenderInfo]
           ([FirstName]
           ,[LastName]
           ,[LicenseNo]
           ,[OrganizationID]
,[ZipCodeID]
,[OfficeNo]
,[StreetNo]
,[StreetName]
,[Website]
,[EmailAddress]
,[Contact1]
,[Contact2]
,[Remarks]
,[Inactive]
,[Password]
,[UserName]
,[UserID]
,[UserType]
)
     VALUES
           ('{obj.FirstName}'
           ,'{obj.LastName}'
           ,'{obj.LicenseNo}'
           ,{obj.OrganizationID}
,{obj.ZipCodeID}
,'{obj.OfficeNo}'
,'{obj.StreetNo}'
,'{obj.StreetName}'
,'{obj.Website}'
,'{obj.EmailAddress}'
,'{obj.Contact1}'
,'{obj.Contact2}'
,'{obj.Remarks}'
,{(obj.Inactive == true ? 1 : 0)}
,'{obj.Password}'
,'{obj.username}'
,{obj.UserID}
,{obj.UserType}
)";
            try
            {
                Query = Query + $@" SELECT SCOPE_IDENTITY() as LenderId";
                DataTable dt = General.FetchData(Query);
                obj.LenderId = int.Parse(dt.Rows[0]["LenderId"].ToString());
                return obj.LenderId;
            }
            catch (Exception ex)
            {
                return LenderId;
            }
        }
        public int UpdateRecord(LenderInfo obj)
        {
            string Query = $@"UPDATE [dbo].[LenderInfo]
   SET [FirstName] = '{obj.FirstName}'
      ,[LastName] = '{obj.LastName}'
      ,[LicenseNo] = '{obj.LicenseNo}'
      ,[OrganizationID] = {obj.OrganizationID}
      ,[ZipCodeID] = {obj.ZipCodeID}
      ,[OfficeNo] = '{obj.OfficeNo}'
      ,[StreetName] = '{obj.StreetName}'
      ,[StreetNo] = '{obj.StreetNo}'
      ,[Website] = '{obj.Website}'
      ,[EmailAddress] = '{obj.EmailAddress}'
      ,[Contact1] = '{obj.Contact1}'
      ,[Contact2] = '{obj.Contact2}'
      ,[Remarks] = '{obj.Remarks}'
      ,[Inactive] = {(obj.Inactive == true ? 1 : 0)}
      ,[Password] = '{obj.Password}'
      ,[UserName] = '{obj.username}'
 WHERE LenderId=" + obj.LenderId;
            try
            {
                General.ExecuteNonQuery(Query);
                return obj.LenderId;
            }
            catch (Exception ex)
            {
                return obj.LenderId;
            }
        }

        public int UpdateProfileRecord(LenderInfo obj)
        {
            string Query = $@"UPDATE [dbo].[LenderInfo]
   SET [FirstName] = '{obj.FirstName}'
      ,[LastName] = '{obj.LastName}'
      ,[StreetName] = '{obj.StreetName}'
      ,[StreetNo] = '{obj.StreetNo}'
      ,[Website] = '{obj.Website}'
      ,[EmailAddress] = '{obj.EmailAddress}'
      ,[Remarks] = '{obj.Remarks}'
 WHERE LenderId=" + obj.LenderId;
            try
            {
                General.ExecuteNonQuery(Query);
                return obj.LenderId;
            }
            catch (Exception ex)
            {
                return obj.LenderId;
            }
        }

    }
}