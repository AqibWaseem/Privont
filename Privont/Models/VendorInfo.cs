using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class VendorInfo:BaseEntity
    {
        public DataTable GetAllRecord(string WhereClause = "")
        {
            DataTable dataTable = new DataTable();
            dataTable = General.FetchData($@"
Select VendorInfo.*,ZipCode.ZipCode,OrganizationInfo.OrganizationTitle from VendorInfo
left outer join ZipCode on VendorInfo.ZipCodeID = ZipCode.ZipCodeID
left outer join OrganizationInfo on VendorInfo.OrganizationID = OrganizationInfo.OrganizationID

" + WhereClause);
            return dataTable;
        }
        public int InsertRecord(VendorInfo obj)
        {
            int VendorID = 0;
            string Query = $@"INSERT INTO [dbo].[VendorInfo]
           ([VendorName]
           ,[PhoneNo]
           ,[RegistrationNo]
           ,[ZIPCodeID]
,[OfficeNo]
,[StreetNo]
,[StreetName]
,[Website]
,[EmailAddress]
,[Contact1]
,[Contact2]
,[Remarks]
,[Inactive]
)
     VALUES
           ('{obj.VendorName}'
           ,'{obj.PhoneNo}'
           ,'{obj.RegistrationNo}'
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
)";
            try
            {
                Query = Query + $@"SELECT SCOPE_IDENTITY() as VendorID";
                DataTable dt = General.FetchData(Query);
                obj.VendorID = int.Parse(dt.Rows[0]["VendorID"].ToString());
                return obj.VendorID;
            }
            catch (Exception ex)
            {
                return VendorID;
            }
        }
        public int UpdateRecord(VendorInfo obj)
        {
            string Query = $@"UPDATE [dbo].[VendorInfo]
   SET [VendorName] = '{obj.VendorName}'
      ,[RegistrationNo] = '{obj.RegistrationNo}'
      ,[ZIPCodeID] = {obj.ZipCodeID}
      ,[OfficeNo] = '{obj.OfficeNo}'
      ,[StreetName] = '{obj.StreetName}'
      ,[StreetNo] = '{obj.StreetNo}'
      ,[Website] = '{obj.Website}'
      ,[EmailAddress] = '{obj.EmailAddress}'
      ,[Contact1] = '{obj.Contact1}'
      ,[Contact2] = '{obj.Contact2}'
      ,[Remarks] = '{obj.Remarks}'
      ,[Inactive] = {(obj.Inactive == true ? 1 : 0)}
 WHERE VendorID=" + obj.VendorID;
            try
            {
                General.ExecuteNonQuery(Query);
                return obj.VendorID;
            }
            catch (Exception ex)
            {
                return obj.VendorID;
            }
        }


        public int UpdateProfileRecord(VendorInfo obj)
        {
            string Query = $@"UPDATE [dbo].[VendorInfo]
   SET [FirstName] = '{obj.FirstName}'
      ,[LastName] = '{obj.LastName}'
      ,[LicenseNo] = '{obj.LicenseNo}'
      ,[OfficeNo] = '{obj.OfficeNo}'
      ,[Website] = '{obj.Website}'
      ,[EmailAddress] = '{obj.EmailAddress}'
      ,[Contact1] = '{obj.Contact1}'
      ,[Contact2] = '{obj.Contact2}'
      ,[OrganizationID] = '{obj.OrganizationID}'
      ,[ZipCodeID] = '{obj.ZipCodeID}'
      ,[StreetNo] = '{obj.StreetNo}'
      ,[StreetName] = '{obj.StreetName}'
      ,[Remarks] = '{obj.Remarks}'
      ,[CompanyName] = '{obj.CompanyName}'
      ,[Longitude] = '{obj.Longitude}'
      ,[Latitude] = '{obj.Latitude}'
 WHERE VendorID=" + obj.VendorID;
            try
            {
                General.ExecuteNonQuery(Query);
                return obj.VendorID;
            }
            catch (Exception ex)
            {
                return obj.VendorID;
            }
        }

        public int InsertRecordsByInviationVendor(VendorInfo obj)
        {
            int VendorID = 0;
            string Query = $@"INSERT INTO [dbo].[VendorInfo]
           ([FirstName]
           ,[LastName]
           ,[EmailAddress]
           ,[Contact1]
           ,[UserID]
           ,[UserType]
           ,[IsApproved]
           ,[IsEmailVerified]
           ,[UniqueIdentifier]
           ,[SourceID]
           ,[PriceRangeID]
           ,[State]
           ,[FirstTimeBuyer]
           ,[IsMilitary]
           ,[TypeID]
           ,[BestTimeToCall])
     VALUES
           ('{obj.FirstName}'
           ,'{obj.LastName}'
           ,'{obj.EmailAddress}'
           ,'{obj.Contact1}'
           ,'{obj.UserID}'
           ,'{obj.UserType}'
           ,'{obj.IsApproved}'
           ,'{obj.IsEmailVerified}'
           ,'{obj.UniqueIdentifier}'
           ,'{obj.SourceID}'
           ,'{obj.PriceRangeID}'
           ,'{obj.State}'
           ,'{obj.FirstTimeBuyer}'
           ,'{obj.IsMilitary}'
           ,'{obj.TypeID}'
           ,'{obj.BestTimeToCall}')";
            try
            {
                Query = Query + $@" SELECT SCOPE_IDENTITY() as VendorID";
                DataTable dt = General.FetchData(Query);
                obj.VendorID = int.Parse(dt.Rows[0]["VendorID"].ToString());
                return obj.VendorID;
            }
            catch (Exception ex)
            {
                return VendorID;
            }
        }
        public List<VendorInfo> GetVendorInfoRecordsInList(DataTable dt, int UserType)
        {
            List<VendorInfo> lst = new List<VendorInfo>();
            lst = General.ConvertDataTable<VendorInfo>(dt);
            foreach (var dr in lst)
            {
                DataTable dtSocialMediaAccount = new SocialMediaInfo().GetAllRecordsInDataTable(dr.VendorID, UserType);
                List<SocialMediaInfo> lstSocialMedia = new SocialMediaInfo().GetAllRecordsInList(dtSocialMediaAccount);
                dr.lstSocialMediaInfo = lstSocialMedia;
            }
            return lst;
        }
    }
}