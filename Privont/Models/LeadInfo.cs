using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class LeadInfo :BaseEntity
    {
        #region Model
        public int LeadID { get; set; }
        public string PhoneNo { get; set; }
        public bool ReadytoOptin { get; set; }
        public DateTime EntryDateTime { get; set; }
        public int OptInSMSStatus { get; set; }
        public int PricePointID { get; set; }
        public string PricePointName { get; set; }
        public int isClaimLead { get; set; }
        public int APITypeID { get; set; }
        public int ApiLeadID { get; set; }
        public string ApiSource { get; set; }
        public bool SMSSent { get; set; }
        public string Claimed { get; set; }
        public List<LenderInfo> LstLenderInfo = new List<LenderInfo>();
        #endregion

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
Select * from (select LeadID,LeadInfo.FirstName,LeadInfo.LastName,isnull(OptInSMSStatus,0)OptInSMSStatus,PhoneNo,LeadInfo.EmailAddress,EntryDateTime,isNull(ReadytoOptin,0)ReadytoOptin,LeadInfo.UserID,EntrySource as UserType , ZipCode.ZipCode , case when DATEDIFF(minute, EntryDateTime, GetDATE())<=@EntryTime then 1 else 0 end IsBelowTime from leadinfo inner join RealEstateAgentInfo on RealEstateAgentInfo.RealEstateAgentID = LeadInfo.UserID inner join ZipCode on RealEstateAgentInfo.ZipCodeID = ZipCode.ZipCodeID Where LeadInfo.EntrySource = 2 and LeadInfo.isClaimLead = 1 )A where 1=case when isbelowtime=1 and (select Count(*) from favouritelender where favouritelender.UserID=A.Userid and favouritelender.UserID=@lenderid)>1 then 1 When  isbelowtime=0 then 1  else 0 end order by LeadID desc";
            }
            else
            {
                sql = $@"SELECT
    LI.LeadID,
    LI.FirstName,
    LI.LastName,
    ISNULL(LI.OptInSMSStatus, 0) AS OptInSMSStatus,
    LI.PhoneNo,
    LI.EmailAddress,
    LI.EntryDateTime,
    ISNULL(LI.ReadytoOptin, 0) AS ReadytoOptin,
    LI.UserID,
    LI.EntrySource AS UserType,
    LI.PricePointID,
LI.SMSSent,
    LPP.PricePoint AS PricePointName,
    LI.isClaimLead ";
                sql = sql + $@" FROM
    LeadInfo LI
LEFT OUTER JOIN
    LeadPricePoint LPP ON LI.PricePointID = LPP.PricePointID ";
                sql = sql + $@" {WhereClause} ORDER BY
    LI.LeadID desc";
            }
            dataTable = General.FetchData(sql);
            return dataTable;
        }
        public int InsertRecord(LeadInfo obj)
        {
            if(obj.UserID==0)
            {
                obj.UserID = General.UserID;
            }
            if (obj.UserType == 0)
            {
                obj.UserType = General.UserType;
            }
            int LeadID = 0;
            string Query = $@"INSERT INTO [dbo].[LeadInfo]
           ([FirstName]
           ,[LastName]
           ,[PhoneNo]
           ,[EmailAddress],
[UserID],
[EntryDateTime],
[EntrySource],[ApiSource]
           ,[ApiLeadID]
           ,[APITypeID])
     VALUES
           ('{obj.FirstName}'
           ,'{obj.LastName}'
           ,'{obj.PhoneNo}'
           ,'{obj.EmailAddress}'
,{obj.UserID},
GetDate(),
{obj.UserType},'{obj.ApiSource}'
           ,{obj.ApiLeadID}
           ,{obj.APITypeID})";
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

        public int UpdateProfileRecord(LeadInfo obj)
        {
            string Query = $@"UPDATE [dbo].[LeadInfo]
   SET [FirstName] = '{obj.FirstName}'
      ,[LastName] = '{obj.LastName}'
    
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

        public int InsertRecordsByInviation(LeadInfo obj)
        {
            int LeadID = 0;
            string Query = $@"INSERT INTO [dbo].[LeadInfo]
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
           ,[PricePointID]
           ,[State]
           ,[FirstTimeBuyer]
           ,[IsMilitary]
           ,[TypeID]
           ,[BestTimeToCall],[EntryDateTime],
[EntrySource],[ApiSource]
           ,[ApiLeadID]
           ,[APITypeID])
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
           ,'{obj.PriceRangeID}'
           ,'{obj.State}'
           ,'{obj.FirstTimeBuyer}'
           ,'{obj.IsMilitary}'
           ,'{obj.TypeID}'
           ,'{obj.BestTimeToCall}',GETDATE(),
{obj.UserType},'{obj.ApiSource}'
           ,{obj.ApiLeadID}
           ,{obj.APITypeID})";
            try
            {
                Query = Query + $@" SELECT SCOPE_IDENTITY() as LeadID";
                DataTable dt = General.FetchData(Query);
                obj.LeadID = int.Parse(dt.Rows[0]["LeadID"].ToString());
                return obj.LeadID;
            }
            catch (Exception ex)
            {
                return LeadID;
            }
        }
        public DataTable GetAllRecordsVIAPhoneNo(string PhoneNo)
        {
            string Query = $@"select * from LeadInfo where Contact1='{PhoneNo}'";
            DataTable dataTable = General.FetchData(Query);
            return dataTable;
        }
    }
}