using Microsoft.SqlServer.Server;
using Privont.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class LenderInfoController : Controller
    {
        LenderInfo Model = new LenderInfo();
        // GET: LenderInfo
        public ActionResult Index()
        {
            if (General.UserType == 2)
            {
                return RedirectToAction("PageNotFound", "Home");
            }
            List<LenderInfo> lst = General.ConvertDataTable<LenderInfo>(Model.GetAllRecord());
            return View(lst);
        }

        [HttpPost]
        public ActionResult CreateOrganizationInfo(string OrganizationName)
        {
            try
            {
                DataTable dt = General.FetchData($@"Select * from OrganizationInfo Where rtrim(ltrim(OrganizationTitle)) like '%{OrganizationName}%' and rtrim(ltrim(OrganizationTitle)) not like ''");
                // TODO: Add insert logic here
                if (dt.Rows.Count > 0)
                {
                    return Json("1,");
                }
                else
                {
                    string Query = "Insert into OrganizationInfo(OrganizationTitle,Inactive) ";
                    Query = Query + "Values ('" + OrganizationName + "'," + 0 + ")";
                    Query = Query + "   Select @@IDENTITY AS OrganizationID";
                    int OrganizationID = int.Parse(General.FetchData(Query).Rows[0]["OrganizationID"].ToString());
                    return Json("2," + OrganizationID);
                }

            }
            catch
            {
                return View();
            }
        }

        public ActionResult Create()
        {
            if (General.UserType == 2)
            {
                return RedirectToAction("PageNotFound", "Home");
            }
            ViewBag.Organization = new DropDown().GetOrganizationList();
            ViewBag.ZipCode = new DropDown().GetZipCode();
            ViewBag.Message = "";
            return View(Model);
        }
        [HttpPost]
        public ActionResult Create(LenderInfo collection)
        {
            try
            {

                if (collection.LenderId == 0)
                {
                    collection.UserID = 0;
                    DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.username}' union Select UserName from LenderInfo Where UserName = '{collection.username}'union Select UserName from USerInfo Where UserName = '{collection.username}'");
                    if (dt.Rows.Count > 0)
                    {
                        ViewBag.Organization = new DropDown().GetOrganizationList("", collection.OrganizationID);
                        ViewBag.ZipCode = new DropDown().GetZipCode("", collection.ZipCodeID);
                        ViewBag.Message = "UserName Already Exist Please Use different UserName";
                        return View(collection);
                    }
                    Model.InsertRecord(collection);
                }
                else
                {
                    DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.username}'  union Select UserName from LenderInfo Where UserName = '{collection.username}' and LenderID != {collection.LenderId}union Select UserName from USerInfo Where UserName = '{collection.username}'");
                    if (dt.Rows.Count > 0)
                    {
                        ViewBag.Organization = new DropDown().GetOrganizationList("", collection.OrganizationID);
                        ViewBag.ZipCode = new DropDown().GetZipCode("", collection.ZipCodeID);
                        ViewBag.Message = "UserName Already Exist Please Use different UserName";
                        return View(collection);
                    }
                    Model.UpdateRecord(collection);
                }
                if (General.UserType == 3)
                {
                    return RedirectToAction("Index", "Home");
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
            if (General.UserType == 2)
            {
                return RedirectToAction("PageNotFound", "Home");
            }
            List<LenderInfo> lst = General.ConvertDataTable<LenderInfo>(Model.GetAllRecord(" and LenderId=" + id));
            ViewBag.Organization = new DropDown().GetOrganizationList("", lst[0].OrganizationID);
            ViewBag.ZipCode = new DropDown().GetZipCode("", lst[0].ZipCodeID);
            return View("Create", lst[0]);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (General.UserType == 2)
            {
                return RedirectToAction("PageNotFound", "Home");
            }
            string SQL = $@"Delete from LenderInfo where LenderId=" + id;
            General.ExecuteNonQuery(SQL);
            return Json("true");
        }

        public ActionResult FavouriteLender()
        {
            DataTable dt = General.FetchData($@"Select (LenderInfo.FirstName+' '+LenderInfo.LastName)LenderName,LenderInfo.Contact1,LenderInfo.OfficeNo from favouriteLender inner join LenderInfo
on FavouriteLender.LenderID = LenderInfo.LenderId 
inner join RealEstateAgentInfo on RealEstateAgentId = FavouriteLender.UserID 

Where RealEstateAgentId = {General.UserID}");
            ViewBag.Lender = new DropDown().GetLenderInfo();
            return View(dt);
        }
        public ActionResult SaveFavouritLender(int LenderID)
        {
            DataTable dt = General.FetchData($@"Select * from FavouriteLender Where LenderID={LenderID} and UserID={General.UserID}");
            if (dt.Rows.Count > 0)
            {
                return Json("1,This Lender Is already Added");
            }
            General.ExecuteNonQuery($@"Insert Into FavouriteLender Values({LenderID},{General.UserID})");
            DataTable dt2 = General.FetchData($@"Select (LenderInfo.FirstName+' '+LenderInfo.LastName)LenderName,LenderInfo.Contact1,LenderInfo.OfficeNo from favouriteLender inner join LenderInfo
on FavouriteLender.LenderID = LenderInfo.LenderId Where LenderInfo.LenderId=  {LenderID}");
            string Name = dt2.Rows[0]["LenderName"].ToString();
            string Contact1 = dt2.Rows[0]["Contact1"].ToString();
            string OfficeNo = dt2.Rows[0]["OfficeNo"].ToString();
            return Json("true," + Name + "," + Contact1 + "," + OfficeNo);
        }

        public ActionResult Signup(string q, int d, int i, string y, string s)
        {
            y = General.Decrypt(y, General.key);
            LenderInfo obj = new LenderInfo();
            obj.UserID = i;
            obj.UserType = d;
            List<LenderInfo> lst = General.ConvertDataTable<LenderInfo>(Model.GetAllRecordforSignup(" where LenderId=" + y));
            if (lst.Count <= 0)
            {
                return RedirectToAction("LinkExpire", "RealEstateAgentInfo");
            }
            ViewBag.UserInformation = "You Have Been Invited by " + General.FetchData($@"Select (FirstName+' '+LastName)Name From LenderInfo Where LenderId={i}").Rows[0]["Name"].ToString();
            ViewBag.Organization = new DropDown().GetOrganizationList();
            return View(lst[0]);

        }
        [HttpPost]
        public ActionResult SignUp(LenderInfo collection)
        {
            DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.username}' union Select UserName from LenderInfo Where UserName = '{collection.username}'union Select UserName from USerInfo Where UserName = '{collection.username}'");
            if (dt.Rows.Count > 0)
            {
                ViewBag.Organization = new DropDown().GetOrganizationList("", collection.OrganizationID);
                ViewBag.ZipCode = new DropDown().GetZipCode("", collection.ZipCodeID);
                ViewBag.UserInformation = "You Have Been Invited by " + General.FetchData($@"Select (FirstName+' '+LastName)Name From LenderInfo Where LenderId={collection.UserID}").Rows[0]["Name"].ToString();
                ViewBag.Message = "UserName Already Exist Please Use different UserName";
                return View(collection);
            }
            Model.UpdateRecord(collection);
            return RedirectToAction("Login", "Home");
        }
        public ActionResult LinkExpire()
        {
            return View();
        }
        public ActionResult ApprovalStatus(int LenderID, int Approved, string ApprovalRemarks)
        {
            string sql = $@"Update LenderInfo Set IsApproved={Approved} , ApprovedRemarks='{ApprovalRemarks}' Where LenderID={LenderID}";
            General.ExecuteNonQuery(sql);
            return Json("true,");
        }
        // 
        #region APIs Records
        [HttpGet]
        public JsonResult GetLenderInfo(int RealEstateAgentID=0,int Status = 0)
        {
            try
            {
                string WhereClause = "";
                if (Status == 1)//For Pending Lender
                {
                    WhereClause = "   and isnull(IsApproved,0)=0   ";
                }
                else if (Status == 2)// For Approved Lenders
                {
                    WhereClause = "   and isnull(IsApproved,0)=1   ";
                }
                if (RealEstateAgentID > 0)
                {
                    //WhereClause = WhereClause + $@"  and LenderInfo.LenderId not in (select LenderID from FavouriteLender where UserID={RealEstateAgentID})  ";
                    WhereClause = WhereClause + $@"  and LenderInfo.LenderId ={RealEstateAgentID}  ";
                }
                string Query = $@"select LenderInfo.*
,ISNULL(AverageRating.AverageRating,0)AverageRating,ISNULL(AverageRating.TotalFeedBack,0)TotalFeedBack  from LenderInfo
left outer join OrganizationInfo on LenderInfo.OrganizationID = OrganizationInfo.OrganizationID
left outer join ZipCode on LenderInfo.ZipCodeID = ZipCode.ZipCodeID
left outer join 
(
SELECT UserID, AVG(Rating) AS AverageRating,count(*) as TotalFeedBack
FROM FeedBackInfo where UserType=3
GROUP BY UserID
)AverageRating on LenderInfo.LenderId=AverageRating.UserID   {WhereClause}";

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(General.FetchData(Query));

                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Lender Information!", dbrows);
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: "+ex.Message, null);
                return jr;
            }
        }
        [HttpPost]
        public JsonResult PostLenderInfo(LenderInfo collection)
        {
            DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.username}' union Select UserName from LenderInfo Where UserName = '{collection.username}'union Select UserName from USerInfo Where UserName = '{collection.username}'");
            if (dt.Rows.Count > 0)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.Conflict, "UserName Already Exist Please Use different UserName!", null);
                return jr;
            }
            collection.LenderId = Model.InsertRecord(collection);
            if (collection.LenderId == 0)
            {

                JsonResult jr = GeneralApisController.ResponseMessage(
                                                                HttpStatusCode.BadGateway,
                                                                "Unabled to Insert Records... Please contact to the administration!", null);
                return jr;
            }
            else
            {
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(Model.GetAllRecord(" where LenderId=" + collection.LenderId));

                JsonResult jr = GeneralApisController.ResponseMessage(
                                                               HttpStatusCode.OK,
                                                               "Data Saved Successfully!", dbrows);
                return jr;
            }

        }
        [HttpPost]
        public JsonResult UpdateLenderInfo(LenderInfo collection)
        {
            DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.username}'  union Select UserName from LenderInfo Where UserName = '{collection.username}' and LenderID != {collection.LenderId}union Select UserName from USerInfo Where UserName = '{collection.username}'");
            if (dt.Rows.Count > 0)
            {

                JsonResult jr = GeneralApisController.ResponseMessage(
                                                       HttpStatusCode.Conflict,
                                                       "UserName Already Exist Please Use different UserName!", null);
                return jr;
            }
            collection.LenderId = Model.UpdateRecord(collection);
            if (collection.LenderId == 0)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(
                                                       HttpStatusCode.BadGateway,
                                                       "Unabled to Update Records... Please contact to the administration!", null);
                return jr;
            }
            else
            {
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(Model.GetAllRecord(" where LenderId=" + collection.LenderId));
                JsonResult jr = GeneralApisController.ResponseMessage(
                                                    HttpStatusCode.OK,
                                                    "Data Updated Successfully!", dbrows);
                return jr;
            }

        }

        #region Favourite Lenders APIs
        //
        [HttpGet]
        public JsonResult GetFavouriteLenderVIARealEstateAgentId(int RealEstateAgentId)
        {
            try
            {
                string Query = $@"
Select LenderInfo.*,OrganizationInfo.OrganizationTitle,ZipCode.ZipCode
,ISNULL(AverageRating.AverageRating,0)AverageRating,ISNULL(AverageRating.TotalFeedBack,0)TotalFeedBack
from LenderInfo 
left outer join OrganizationInfo on LenderInfo.OrganizationID = OrganizationInfo.OrganizationID
left outer join ZipCode on LenderInfo.ZipCodeID = ZipCode.ZipCodeID
left outer join 
(
SELECT UserID, AVG(Rating) AS AverageRating,count(*) as TotalFeedBack
FROM FeedBackInfo where UserType=3
GROUP BY UserID
)AverageRating on LenderInfo.LenderId=AverageRating.UserID
 
inner join FavouriteLender
on FavouriteLender.LenderID = LenderInfo.LenderId 
inner join RealEstateAgentInfo on RealEstateAgentId = FavouriteLender.UserID 

Where RealEstateAgentId ={RealEstateAgentId}";
                DataTable dt = General.FetchData(Query);
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);

                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Favourite Lender Information!", dbrows);
                return jr;

            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
        //Create New Favourite Lender
        [HttpGet]
        public JsonResult PostFavouriteLender(int LenderID, int RealEstateAgentID)
        {

            try
            {
                DataTable dt = General.FetchData($@"Select * from FavouriteLender Where LenderID={LenderID} and UserID={RealEstateAgentID}");
                if (dt.Rows.Count > 0)
                {
                    JsonResult res = GeneralApisController.ResponseMessage(HttpStatusCode.Conflict, "This Lender Is already Added!", null);
                    return res;
                }
                General.ExecuteNonQuery($@"Insert Into FavouriteLender Values({LenderID},{RealEstateAgentID})");

                DataTable dt2 = General.FetchData($@"

select LenderInfo.*,OrganizationInfo.OrganizationTitle,ZipCode.ZipCode ,
,ISNULL(AverageRating.AverageRating,0)AverageRating,ISNULL(AverageRating.TotalFeedBack,0)TotalFeedBack  from LenderInfo
left outer join OrganizationInfo on LenderInfo.OrganizationID = OrganizationInfo.OrganizationID
left outer join ZipCode on LenderInfo.ZipCodeID = ZipCode.ZipCodeID
left outer join 
(
SELECT UserID, AVG(Rating) AS AverageRating,count(*) as TotalFeedBack
FROM FeedBackInfo where UserType=3
GROUP BY UserID
)AverageRating on LenderInfo.LenderId=AverageRating.UserID

where LenderInfo.LenderId in (select LenderID from FavouriteLender where UserID={RealEstateAgentID})");
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt2);

                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Favourite Lender Information!", dbrows);
                return jr;

            }
            catch (Exception ex)
            {

                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
        [HttpGet]
        public JsonResult GetAllLendersExceptFavourite(int RealEstateAgentID)
        {

            try
            {

                DataTable dt2 = General.FetchData($@"
select LenderInfo.*
,ISNULL(AverageRating.AverageRating,0)AverageRating,ISNULL(AverageRating.TotalFeedBack,0)TotalFeedBack  from LenderInfo
left outer join OrganizationInfo on LenderInfo.OrganizationID = OrganizationInfo.OrganizationID
left outer join ZipCode on LenderInfo.ZipCodeID = ZipCode.ZipCodeID
left outer join 
(
SELECT UserID, AVG(Rating) AS AverageRating,count(*) as TotalFeedBack
FROM FeedBackInfo where UserType=3
GROUP BY UserID
)AverageRating on LenderInfo.LenderId=AverageRating.UserID

where LenderInfo.LenderId not in (select LenderID from FavouriteLender where UserID={RealEstateAgentID})");


                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt2);

                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Favourite Lender Information!", dbrows);
                return jr;

            }
            catch (Exception ex)
            {

                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
        #endregion
        #endregion

    }
}