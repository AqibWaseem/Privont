using Microsoft.AspNetCore.Http;
using Privont.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Privont.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        [HttpGet]
        public JsonResult GraphReports(int UserID,int UserType,int Month,int Year)
        {
            try
            {
                string WhereClause = "";
                DataTable dataTable = new DataTable();
                JsonResult jr = new JsonResult();
                string Query = $@"declare @UserID as int = {UserID}
declare @UserType as int = {UserType}
select 
(select Count(*) as MyCircle from FavouriteLender where UserID=@UserID ) as MyCircle,
(select Count(*) as ApprovedMembers from LeadInfo where UserID=@UserID and UserType=@UserType  and IsApproved=1) as ApprovedMembers,
(select Count(*) as PendingMembers from LeadInfo where UserID=@UserID  and UserType=@UserType and IsApproved=0) as PendingMembers,
(select Count(*) as ReferredVendors from VendorInfo where UserID=@UserID and UserType=@UserType) as ReferredVendors
";
                DataTable dtBottomDetails = General.FetchData(Query);

                string QueryMemberInfoByDateWise = $@"SELECT        EntryDateTime, IsApproved
FROM            LeadInfo
WHERE        (UserID = {UserID}) AND (UserType = {UserType})  and MONTH(EntryDateTime) ={Month} and YEAR(EntryDateTime) = {Year}";
                DataTable dtMemberInfoByDateWise = General.FetchData(QueryMemberInfoByDateWise);


                List<Dictionary<string, object>> dbdtBottomDetails = new General().GetAllRowsInDictionary(dtBottomDetails);
                List<Dictionary<string, object>> dbtMemberInfoByDateWise = new General().GetAllRowsInDictionary(dtMemberInfoByDateWise);

                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Reports");
                JSResponse.Add("GraphDetails", dbtMemberInfoByDateWise);
                JSResponse.Add("BottomDetails", dbdtBottomDetails);

                jr = new JsonResult()
                {
                    Data = JSResponse,
                    ContentType = "application/json",
                    ContentEncoding = System.Text.Encoding.UTF8,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
                return jr;
            }
            catch (Exception ex)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Reports");
                JSResponse.Add("GraphDetails", null);
                JSResponse.Add("BottomDetails", null);

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
        [HttpGet]
        public JsonResult GetTransactionDetais(int UserID,int UserType)
        {
            try
            {
                string WhereClause = "";
                DataTable dataTable = new DataTable();
                JsonResult jr = new JsonResult();
                string Query = $@"select * from TransactionDetails where UserID={UserID} and UserType={UserType}";
                DataTable dtBottomDetails = General.FetchData(Query);


                List<Dictionary<string, object>> bdRows = new General().GetAllRowsInDictionary(dtBottomDetails);
                
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Transaction Details");
                JSResponse.Add("Data", bdRows);

                jr = new JsonResult()
                {
                    Data = JSResponse,
                    ContentType = "application/json",
                    ContentEncoding = System.Text.Encoding.UTF8,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
    }
}
