using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class FeedBackInfo
    {
        #region Model
        public int FeedbackID { get; set; }
        public string FeedbackTitle { get; set; }
        public bool Inactive { get; set; }
        public string Remarks { get; set; }
        public int UserID { get; set; }
        public int UserType { get; set; }
        public int EntryUserID { get; set; }
        public int EntryUserTypeID { get; set; }
        public DateTime EntryDate { get; set; }
        public int Rating { get; set; }
        #endregion

        public DataTable GetAllRecords(string WhereClause="")
        {
            DataTable dataTable = General.FetchData(" select * from FeedBackInfo " + WhereClause);
            return dataTable;
        }
        public int InsertRecords()
        {
            try
            {
                int FeedbackID = 0;
                string Query = $@"INSERT INTO FeedBackInfo
           (FeedbackTitle
           ,Inactive
           ,Remarks
           ,UserID
           ,UserType
           ,EntryUserID
           ,EntryUserTypeID
           ,EntryDate,Rating)
     VALUES
           ('{FeedbackTitle}'
           ,{(Inactive ? 1 : 0)}
           ,'{Remarks}'
           ,{UserID}
           ,{UserType}
           ,{EntryUserID}
           ,{EntryUserTypeID}
           ,'{DateTime.Now}',{Rating})";
                Query = Query + "  select @@identity as FeedbackID";
                DataTable dt = General.FetchData(Query);
                int.TryParse(dt.Rows[0]["FeedbackID"].ToString(), out FeedbackID);
                return FeedbackID;
            }
            catch
            {
                return 0;
            }
        }
        public DataTable GetFeedbackRatingVIAUserID(int UserID,int UserType)
        {
            string whereClause = $"  UserID={UserID} and UserType="+ UserType;
            DataTable dtFeedBack = General.FetchData($@"
declare @TotalFeedbacks int

select  @TotalFeedbacks = count(*) from FeedBackInfo where {whereClause}

select  

(
select (cast((COUNT(*)*100) / @TotalFeedbacks as decimal(18,2))) as Rating from FeedBackInfo where {whereClause} and Rating=1
) as Rating1,

(
select (cast((COUNT(*)*100) / @TotalFeedbacks as decimal(18,2))) as Rating from FeedBackInfo where {whereClause} and Rating=2
) as Rating2,
(
select (cast((COUNT(*)*100) / @TotalFeedbacks as decimal(18,2))) as Rating from FeedBackInfo where {whereClause} and Rating=3
) as Rating3,
(
select (cast((COUNT(*)*100) / @TotalFeedbacks as decimal(18,2))) as Rating from FeedBackInfo where {whereClause} and Rating=4
) as Rating4,

(
select (cast((COUNT(*)*100) / @TotalFeedbacks as decimal(18,2))) as Rating from FeedBackInfo where {whereClause} and Rating=5
) as Rating5");
            return dtFeedBack;
        }
    }
}