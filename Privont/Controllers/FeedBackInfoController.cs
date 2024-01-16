using Privont.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrueDialog.Model;

namespace Privont.Controllers
{
    public class FeedBackInfoController : Controller
    {
        // GET: FeedBackInfo
        [HttpGet]
        public JsonResult GetFeedBackInfo(int UserID,int UserTypeID,int EntryUserID,int EntryUserTypeID)
        {
            try
            {
                //Reting Details
                DataTable dtFeedBackRating = new FeedBackInfo().GetFeedbackRatingVIAUserID(UserID, UserTypeID);
                //Feedback Details
                DataTable dt = new DataTable();
                string WhereClause = "";
                if(UserTypeID == 3)
                {
                    WhereClause = $@" select FeedBackInfo.*,LenderInfo.LenderId,LenderInfo.FirstName,LenderInfo.LastName 
                            from  (select * from FeedBackInfo where Inactive=0)FeedBackInfo  
                            inner join LenderInfo on FeedBackInfo.UserID=LenderInfo.LenderId 
                                    where FeedBackInfo.UserID=" +UserID;
                    dt = General.FetchData(WhereClause);
                }
                else if (UserTypeID == 5)
                {
                    WhereClause = $@" select FeedBackInfo.*,VendorInfo.VendorID,VendorInfo.FirstName,VendorInfo.LastName 
                            from (select * from FeedBackInfo where Inactive=0)FeedBackInfo
	                        inner join VendorInfo on FeedBackInfo.UserID=VendorInfo.VendorID 
                                    where FeedBackInfo.UserID=" + UserID;
                    dt = General.FetchData(WhereClause);
                }
                bool flagResponse = false;
                if(General.FetchData($@"select count(*)Exist from FeedBackInfo where EntryUserID={EntryUserID} and EntryUserTypeID={EntryUserTypeID} and UserID={UserID} and UserType={UserTypeID}").Rows.Count > 0 )
                { 
                    flagResponse = true; 
                }

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                List<Dictionary<string, object>> dbdtFeedBackRatingrows = new General().GetAllRowsInDictionary(dtFeedBackRating);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Feedback Information");
                JSResponse.Add("AlreadyReviewed", flagResponse);
                JSResponse.Add("Data", dbrows);  
                JSResponse.Add("RatingDetails", dbdtFeedBackRatingrows);

                JsonResult jr = new JsonResult()
                {
                    Data = JSResponse,
                    ContentType = "application/json",
                    ContentEncoding = System.Text.Encoding.UTF8,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
                return jr;
            }
            catch(Exception ex)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Error: "+ ex.Message);
                JSResponse.Add("AlreadyReviewed", false);
                JSResponse.Add("Data", DBNull.Value);
                JSResponse.Add("RatingDetails", DBNull.Value);

                JsonResult jr = new JsonResult()
                {
                    Data = JSResponse,
                    ContentType = "application/json",
                    ContentEncoding = System.Text.Encoding.UTF8,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
                return jr;
            }
        }
        [HttpPost]
        public JsonResult PostFeedBackInfo(FeedBackInfo collection)
        {
            try
            {
                var responseID = collection.InsertRecords();

                int UserID=collection.UserID;
                int UserTypeID = collection.UserType;
                int EntryUserID = collection.EntryUserID;
                int EntryUserTypeID = collection.EntryUserTypeID;

                //Reting Details
                DataTable dtFeedBackRating = new FeedBackInfo().GetFeedbackRatingVIAUserID(UserID, UserTypeID);
                //Feedback Details
                DataTable dt = new DataTable();
                string WhereClause = "";
                if (UserTypeID == 3)
                {
                    WhereClause = $@" select FeedBackInfo.*,LenderInfo.LenderId,LenderInfo.FirstName,LenderInfo.LastName 
                            from  (select * from FeedBackInfo where Inactive=0)FeedBackInfo  
                            inner join LenderInfo on FeedBackInfo.UserID=LenderInfo.LenderId 
                                    where FeedBackInfo.UserID=" + UserID;
                    dt = General.FetchData(WhereClause);
                }
                else if (UserTypeID == 5)
                {
                    WhereClause = $@" select FeedBackInfo.*,VendorInfo.VendorID,VendorInfo.FirstName,VendorInfo.LastName 
                            from (select * from FeedBackInfo where Inactive=0)FeedBackInfo
	                        inner join VendorInfo on FeedBackInfo.UserID=VendorInfo.VendorID 
                                    where FeedBackInfo.UserID=" + UserID;
                    dt = General.FetchData(WhereClause);
                }
                bool flagResponse = false;
                if (General.FetchData($@"select count(*)Exist from FeedBackInfo where EntryUserID={EntryUserID} and EntryUserTypeID={EntryUserTypeID} and UserID={UserID} and UserType={UserTypeID}").Rows.Count > 0)
                {
                    flagResponse = true;
                }

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);

                List<Dictionary<string, object>> dbdtFeedBackRatingrows = new General().GetAllRowsInDictionary(dtFeedBackRating);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Feedback Information");
                JSResponse.Add("AlreadyReviewed", flagResponse);
                JSResponse.Add("Data", dbrows);
                JSResponse.Add("RatingDetails", dbdtFeedBackRatingrows);
                JsonResult jr = new JsonResult()
                {
                    Data = JSResponse,
                    ContentType = "application/json",
                    ContentEncoding = System.Text.Encoding.UTF8,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
                return jr;
            }
            catch(Exception ex)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Error: " + ex.Message);
                JSResponse.Add("AlreadyReviewed", false);
                JSResponse.Add("Data", DBNull.Value);
                JSResponse.Add("RatingDetails", DBNull.Value);

                JsonResult jr = new JsonResult()
                {
                    Data = JSResponse,
                    ContentType = "application/json",
                    ContentEncoding = System.Text.Encoding.UTF8,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
                return jr;
            }
        }
    }
}