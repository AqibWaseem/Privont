using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class LeadInfo
    {
        public int LeadID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNo { get; set; }
        public string EmailAddress { get; set; }
        public bool ReadytoOptin { get; set; }
        public int UserID { get; set; }
        public int UserType { get; set; }
        public DateTime EntryDateTime { get; set; }
        public string ZipCode { get; set; }
        public int OptInSMSStatus { get; set; }
        public int PricePointID { get; set; }
        public string PricePointName { get; set; }
        public int isClaimLead { get; set; }
        public DataTable GetAllRecord(string WhereClause = "")
        {
            DataTable dataTable = new DataTable();
            dataTable = General.FetchData($@"select LeadInfo.*,(LeadPricePoint.PricePoint)PricePointName from LeadInfo  left outer join LeadPricePoint on LeadInfo.PricePointID = LeadPricePoint.PricePointID " + WhereClause);
            return dataTable;
        }
        public DataTable GetIndexAllRecord(string WhereClause = "")
        {
            DataTable dataTable = new DataTable();
            string sql = ""; 
            if (General.UserType ==3)
            {
                sql = $@"Declare @LenderID int set @LenderID={General.UserID}
Declare @EntryTime int
Select @EntryTime=ExpiryTime from LeadExpiryTime
Select * from (select LeadID,LeadInfo.FirstName,LeadInfo.LastName,isnull(OptInSMSStatus,0)OptInSMSStatus,PhoneNo,LeadInfo.EmailAddress,EntryDateTime,isNull(ReadytoOptin,0)ReadytoOptin,LeadInfo.UserID,EntrySource as UserType , ZipCode.ZipCode , case when DATEDIFF(minute, EntryDateTime, GetDATE())<=10 then 1 else 0 end IsBelowTime from leadinfo inner join RealEstateAgentInfo on RealEstateAgentInfo.RealEstateAgentID = LeadInfo.UserID inner join ZipCode on RealEstateAgentInfo.ZipCodeID = ZipCode.ZipCodeID Where LeadInfo.EntrySource = 2 and LeadInfo.isClaimLead = 1 )A where 1=case when isbelowtime=1 and (select Count(*) from favouritelender where favouritelender.UserID=A.Userid and favouritelender.UserID=@lenderid)>1 then 1 When  isbelowtime=0 then 1  else 0 end ";
            }
            else
            {
                sql = "Select LeadID,LeadInfo.FirstName,LeadInfo.LastName,isnull(OptInSMSStatus,0)OptInSMSStatus,PhoneNo,LeadInfo.EmailAddress,EntryDateTime,isNull(ReadytoOptin,0)ReadytoOptin,LeadInfo.UserID,EntrySource as UserType,LeadInfo.PricePointID,(LeadPricePoint.PricePoint)PricePointName,LeadInfo.isClaimLead";
                sql = sql + " from LeadInfo  left outer join LeadPricePoint on LeadInfo.PricePointID = LeadPricePoint.PricePointID ";
                sql = sql + $@" {WhereClause} Order by LeadID";
            }
            dataTable = General.FetchData(sql);
            return dataTable;
        }
        public int InsertRecord(LeadInfo obj)
        {
            int LeadID = 0;
            string Query = $@"INSERT INTO [dbo].[LeadInfo]
           ([FirstName]
           ,[LastName]
           ,[PhoneNo]
           ,[EmailAddress],
[UserID],
[EntryDateTime],
[EntrySource],)
     VALUES
           ('{obj.FirstName}'
           ,'{obj.LastName}'
           ,'{obj.PhoneNo}'
           ,'{obj.EmailAddress}'
,{General.UserID},
GetDate(),
{General.UserType})";
            try
            {
                Query = Query + $@"SELECT SCOPE_IDENTITY() as LeadID";
                DataTable dt = General.FetchData(Query);
                obj.LeadID = int.Parse(dt.Rows[0]["LeadID"].ToString());
                return obj.LeadID;
            }
            catch (Exception ex)
            {
                return LeadID;
            }
        }
        public int UpdateRecord(LeadInfo obj)
        {
            string Query = $@"UPDATE [dbo].[LeadInfo]
   SET [FirstName] = '{obj.FirstName}'
      ,[LastName] = '{obj.LastName}'
      ,[PhoneNo] = '{obj.PhoneNo}'
      ,[EmailAddress] = '{obj.EmailAddress}'
 WHERE LeadID=" + obj.LeadID;
            try
            {
                General.ExecuteNonQuery(Query);
                return obj.LeadID;
            }
            catch (Exception ex)
            {
                return obj.LeadID;
            }
        }
    }
}