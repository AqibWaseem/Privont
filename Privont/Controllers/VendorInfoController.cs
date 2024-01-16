using Privont.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Google.Apis.Requests.BatchRequest;

namespace Privont.Controllers
{
    public class VendorInfoController : Controller
    {
        VendorInfo Model = new VendorInfo();
        // GET: VendorInfo
        public ActionResult Index()
        {
            List<VendorInfo> lst = General.ConvertDataTable<VendorInfo>(Model.GetAllRecord());
            return View(lst);
        }

        public ActionResult Create()
        {
            ViewBag.ZipCode = new DropDown().GetZipCode();
            return View(Model);
        }
        [HttpPost]
        public ActionResult Create(VendorInfo collection)
        {
            try
            {
                if (collection.VendorID == 0)
                {
                    Model.InsertRecord(collection);
                }
                else
                {
                    Model.UpdateRecord(collection);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View(collection);
            }
        }
        public ActionResult Edit(int id)
        {
            List<VendorInfo> lst = General.ConvertDataTable<VendorInfo>(Model.GetAllRecord(" where VendorID=" + id));
            ViewBag.ZipCode = new DropDown().GetZipCode("", lst[0].ZipCodeID);
            return View("Create", lst[0]);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string SQL = $@"Delete from VendorInfo where VendorID=" + id;
            General.ExecuteNonQuery(SQL);
            return Json("true");
        }

        #region APIs VendorInfo
        [HttpGet]
        public JsonResult GetVendorInfo(int VendorID = 0,int UserID = 0,int UserTypeID = 0)
        {
            try
            {
                string WhereClause = "";
                string columnTitle = "0";
                if (VendorID > 0)
                {
                   
                    WhereClause = "   where VendorInfo.VendorID = " + VendorID;
                }
                columnTitle = $@"
(select count(*) from FavouriteVendorInfo where FavouriteVendorInfo.FavouriteVendorID=VendorInfo.VendorID and FavouriteVendorInfo.UserID={UserID} 
                    and FavouriteVendorInfo.UserType={UserTypeID} )";
                DataTable dt = General.FetchData($@"Select VendorInfo.*,ZipCode.ZipCode,OrganizationInfo.OrganizationTitle,{columnTitle} as Favourite 
,ISNULL(AverageRating.AverageRating,0)AverageRating,ISNULL(AverageRating.TotalFeedBack,0)TotalFeedBack 


from  (select * from VendorInfo where VendorID!={UserID})VendorInfo

left outer join ZipCode on VendorInfo.ZipCodeID = ZipCode.ZipCodeID
left outer join OrganizationInfo on VendorInfo.OrganizationID = OrganizationInfo.OrganizationID  
left outer join 
(
SELECT UserID, AVG(Rating) AS AverageRating,count(*) as TotalFeedBack
FROM FeedBackInfo where UserType=5
GROUP BY UserID
)AverageRating on VendorInfo.VendorID=AverageRating.UserID

" + WhereClause);
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                if (VendorID == 0)
                {
                    JSResponse.Add("Message", "Vendors Information");
                }
                else
                {
                    JSResponse.Add("Message", "Specific Vendors Information");
                }
                //JSResponse.Add("Message", "Vendors Information");

                JSResponse.Add("Data", dbrows);
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
                JSResponse.Add("Data", DBNull.Value);

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
        public JsonResult GetFavouriteVendorInfo(int UserID,int UserType)
        {
            try
            {
                string columnTitle = $@"
(select count(*) from FavouriteVendorInfo where FavouriteVendorInfo.FavouriteVendorID=VendorInfo.VendorID and FavouriteVendorInfo.UserID={UserID} 
                    and FavouriteVendorInfo.UserType={UserType} )";
                DataTable dt = General.FetchData($@"Select VendorInfo.*,ZipCode.ZipCode,OrganizationInfo.OrganizationTitle,{columnTitle} as Favourite
,ISNULL(AverageRating.AverageRating,0)AverageRating,ISNULL(AverageRating.TotalFeedBack,0)TotalFeedBack 

from VendorInfo
left outer join ZipCode on VendorInfo.ZipCodeID = ZipCode.ZipCodeID
left outer join OrganizationInfo on VendorInfo.OrganizationID = OrganizationInfo.OrganizationID
left outer join 
(
SELECT UserID, AVG(Rating) AS AverageRating,count(*) as TotalFeedBack
FROM FeedBackInfo where UserType=5
GROUP BY UserID
)AverageRating on VendorInfo.VendorID=AverageRating.UserID


                                                        where 
                                                            VendorInfo.VendorID in (select FavouriteVendorID from FavouriteVendorInfo where UserID={UserID} and UserType={UserType})");
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Favourite Vendors Information");
                JSResponse.Add("Data", dbrows);

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
                JSResponse.Add("Data", DBNull.Value);

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
        public JsonResult PostFavouriteVendorInfo(int VendorID,int UserID,int UserType,bool IsFavourite)
        {
            try
            {
                if (IsFavourite)
                {
                    General.ExecuteNonQuery($@"INSERT INTO [dbo].[FavouriteVendorInfo]
           ([FavouriteVendorID]
           ,[UserID]
           ,[UserType])
     VALUES
           ({VendorID}
           ,{UserID}
           ,{UserType})");
                }
                else
                {
                    General.ExecuteNonQuery($@"delete from FavouriteVendorInfo where FavouriteVendorID={VendorID}  and UserID={UserID} and UserType= {UserType}");
                }
              
              



                string WhereClause = "";
                string columnTitle = "0";
                if (VendorID > 0)
                {
                    columnTitle = $@"
(select count(*) from FavouriteVendorInfo where FavouriteVendorInfo.FavouriteVendorID=VendorInfo.VendorID and FavouriteVendorInfo.UserID={UserID} 
                    and FavouriteVendorInfo.UserType={UserType} )";
                    WhereClause = "   where VendorInfo.VendorID = " + VendorID;
                }

                DataTable dt = General.FetchData($@"Select VendorInfo.*,ZipCode.ZipCode,OrganizationInfo.OrganizationTitle,{columnTitle} as Favourite,ISNULL(AverageRating.AverageRating,0)AverageRating,ISNULL(AverageRating.TotalFeedBack,0)TotalFeedBack  from VendorInfo
left outer join ZipCode on VendorInfo.ZipCodeID = ZipCode.ZipCodeID
left outer join OrganizationInfo on VendorInfo.OrganizationID = OrganizationInfo.OrganizationID  
left outer join 
(
SELECT UserID, AVG(Rating) AS AverageRating,count(*) as TotalFeedBack
FROM FeedBackInfo where UserType=5
GROUP BY UserID
)AverageRating on VendorInfo.VendorID=AverageRating.UserID

" + WhereClause);


                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                List<Dictionary<string, object>> lstAverageRating = new General().GetAllRowsInDictionary(GetFeedBackAverageDetail(UserID,UserType));
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Favourite Vendors Information");
                JSResponse.Add("Data", dbrows);

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
                JSResponse.Add("Data", DBNull.Value);

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
        public DataTable GetFeedBackAverageDetail(int UserID,int UserType)
        {
            string Query = $@" SELECT AVG(Rating) AS AverageRating,count(*) as TotalFeedBack
FROM FeedBackInfo where UserID = {UserID} and UserType={UserType} ";
            DataTable dt = General.FetchData(Query);
            return dt;
        }
        #endregion
    }
}