using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class RealEstateAgentInfo : LenderInfo
    {
        public int RealEstateAgentId { get; set; }
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
        public string OrganizationTitle { get; set; }
        public int UserID { get; set; }
        public int UserType { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedRemarks { get; set; }
        public DataTable GetAllRecord(string WhereClause = "")
        {
            DataTable dataTable = new DataTable();
            dataTable = General.FetchData($@"Select RealEstateAgentInfo.*,OrganizationInfo.OrganizationTitle,ZipCode.ZipCode
 from RealEstateAgentInfo
inner join OrganizationInfo on RealEstateAgentInfo.OrganizationID = OrganizationInfo.OrganizationID
 inner join ZipCode on RealEstateAgentInfo.ZipCodeID = ZipCode.ZipCodeID
 Where UserName is not null
  " + WhereClause);
            return dataTable;
        }
        public DataTable GetAllRecordforSignup(string WhereClause = "")
        {
            DataTable dataTable = new DataTable();
            dataTable = General.FetchData($@"Select RealEstateAgentInfo.*
 from RealEstateAgentInfo 
  " + WhereClause);
            return dataTable;
        }
        public int InsertRecord(RealEstateAgentInfo obj)
        {
            int RealEstateAgentID = 0;
            string Query = $@"INSERT INTO [dbo].[RealEstateAgentInfo]
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
                Query = Query + $@"SELECT SCOPE_IDENTITY() as RealEstateAgentID";
                DataTable dt = General.FetchData(Query);
                obj.RealEstateAgentId = int.Parse(dt.Rows[0]["RealEstateAgentID"].ToString());
                return obj.RealEstateAgentId;
            }
            catch (Exception ex)
            {
                return RealEstateAgentID;
            }
        }
        public int UpdateRecord(RealEstateAgentInfo obj)
        {
            string Query = $@"UPDATE [dbo].[RealEstateAgentInfo]
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
 WHERE RealEstateAgentID=" + obj.RealEstateAgentId;
            try
            {
                General.ExecuteNonQuery(Query);
                return obj.RealEstateAgentId;
            }
            catch (Exception ex)
            {
                return obj.RealEstateAgentId;
            }
        }

        public DataTable UserExistanceInfo(string UserName)
        {
            DataTable dt = General.FetchData($@"
SELECT        UserID, username, Password, Inactive, UserType
FROM            (SELECT        RealEstateAgentId AS UserID, username, Password, FirstName, LastName, StreetNo, StreetName, EmailAddress, Contact1, Website, Remarks, Inactive, 2 AS UserType
                          FROM            RealEstateAgentInfo
                          UNION ALL
                          SELECT        LenderId AS UserID, username, Password, FirstName, LastName, StreetNo, StreetName, EmailAddress, Contact1, Website, Remarks, Inactive, 3 AS UserType
                          FROM            LenderInfo
                          UNION ALL
                          SELECT        LeadID, Username, Password, FirstName, LastName, '' AS StreetNo, '' AS StreetName, EmailAddress, PhoneNo, '' AS Website, '' AS Remarks, 0 AS Inactive, 4 AS UserType
                          FROM            LeadInfo
                          UNION ALL
                          SELECT        VendorID, Username, Password, FirstName, LastName, StreetNo, StreetName, EmailAddress, Contact1, Website, Remarks, Inactive, 5 AS UserType
                          FROM            VendorInfo) AS ProfileLogin


where username is not null and username='{UserName}'");
            return dt;
        }
        public int UpdateProfileRecord(RealEstateAgentInfo obj)
        {
            string Query = $@"UPDATE [dbo].[RealEstateAgentInfo]
   SET [FirstName] = '{obj.FirstName}'
      ,[LastName] = '{obj.LastName}'
      ,[StreetName] = '{obj.StreetName}'
      ,[StreetNo] = '{obj.StreetNo}'
      ,[Website] = '{obj.Website}'
      ,[EmailAddress] = '{obj.EmailAddress}'
      ,[Remarks] = '{obj.Remarks}'
 WHERE RealEstateAgentID=" + obj.RealEstateAgentId;
            try
            {
                General.ExecuteNonQuery(Query);
                return obj.RealEstateAgentId;
            }
            catch (Exception ex)
            {
                return obj.RealEstateAgentId;
            }
        }
    }
}