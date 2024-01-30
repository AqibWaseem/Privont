using Privont.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Web.Services.Description;
using System.Collections;
using TrueDialog.Model;

namespace Privont.Controllers
{
    public class RealEstateAgentInfoController : Controller
    {
        RealEstateAgentInfo Model = new RealEstateAgentInfo();
        // GET: RealEstateAgentInfo
        public ActionResult Index()
        {
            if (General.UserType == 3)
            {
                return RedirectToAction("PageNotFound", "Home");
            }
            List<RealEstateAgentInfo> lst = General.ConvertDataTable<RealEstateAgentInfo>(Model.GetAllRecord());
            return View(lst);
        }


        //[HttpPost]
        //public ActionResult CreateOrganizationInfo(string OrganizationName)
        //{
        //    try
        //    {
        //        string SQL = "";
        //        // TODO: Add insert logic here
        //        string Query = "Insert into OrganizationInfo(OrganizationTitle,Inactive) ";
        //        Query = Query + "Values ('" + OrganizationName + "'," + 0 + ")";
        //        Query = Query + "   Select @@IDENTITY AS OrganizationID";
        //        int OrganizationID = int.Parse(General.FetchData(Query).Rows[0]["OrganizationID"].ToString());
        //        return Json(OrganizationID);
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        public ActionResult Create()
        {
            if (General.UserType == 3)
            {
                return RedirectToAction("PageNotFound", "Home");
            }
            ViewBag.Organization = new DropDown().GetOrganizationList();
            ViewBag.ZipCode = new DropDown().GetZipCode();
            ViewBag.Message = "";
            return View(Model);
        }
        [HttpPost]
        public ActionResult Create(RealEstateAgentInfo collection)
        {
            try
            {

                if (collection.RealEstateAgentId == 0)
                {
                    collection.UserID = 0;
                    collection.UserType = 0;
                    DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.Username}' union Select UserName from LenderInfo Where UserName = '{collection.Username}'union Select UserName from USerInfo Where UserName = '{collection.Username}'");
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
                    DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.Username}' and RealEstateAgentId!={collection.RealEstateAgentId} union Select UserName from LenderInfo Where UserName = '{collection.Username}'union Select UserName from USerInfo Where UserName = '{collection.Username}'");
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
            if (General.UserType == 3)
            {
                return RedirectToAction("PageNotFound", "Home");
            }
            List<RealEstateAgentInfo> lst = General.ConvertDataTable<RealEstateAgentInfo>(Model.GetAllRecord(" and RealEstateAgentID=" + id));
            ViewBag.Organization = new DropDown().GetOrganizationList("", lst[0].OrganizationID);
            ViewBag.ZipCode = new DropDown().GetZipCode("", lst[0].ZipCodeID);
            return View("Create", lst[0]);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (General.UserType == 3)
            {
                return RedirectToAction("PageNotFound", "Home");
            }
            string SQL = $@"Delete from RealEstateAgentInfo where RealEstateAgentID=" + id;
            General.ExecuteNonQuery(SQL);
            return Json("true");
        }

        public ActionResult GetZipCode(string ProductTitle)
        {
            if (ProductTitle is null)
            {
                ProductTitle = "";
            }
            DataTable dtEmployee = General.FetchData($@"Select Top(20) ZipCodeID,ZipCode from ZipCode Where ZipCode like '%{ProductTitle}%'");
            List<Dictionary<string, object>> dbrows = new List<Dictionary<string, object>>();//GetTableRows(dtEmployee);

            Dictionary<string, object> dbitem = new Dictionary<string, object>();
            foreach (DataRow dr in dtEmployee.Rows)
            {
                dbitem = new Dictionary<string, object>();
                dbitem.Add("value", dr["ZipCodeID"].ToString());
                dbitem.Add("label", dr["ZipCode"].ToString());
                dbrows.Add(dbitem);
            }
            Dictionary<string, object> products = new Dictionary<string, object>();
            products.Add("ZipCode", dbrows);
            return Json(dbrows, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetZipCodeValue(int ZipCodeID)
        //{
        //    string ZipCode = General.FetchData($@"Select ZipCode from ZipCode Where ZipCodeID={ZipCodeID}").Rows[0]["ZipCode"].ToString();
        //    return Json(ZipCode + ",");
        //}

        public ActionResult Signup(string q, int d, int i, string y, string s)
        {
            y = General.Decrypt(y, General.key);
            List<RealEstateAgentInfo> lst = General.ConvertDataTable<RealEstateAgentInfo>(Model.GetAllRecordforSignup(" where RealEstateAgentID=" + 1086));
            if (lst.Count <= 0)
            {
                return Redirect("LinkExpire");
            }
            ViewBag.Organization = new DropDown().GetOrganizationList();
            ViewBag.UserInformation = "You Have Been Invited by " + General.FetchData($@"Select (FirstName+' '+LastName)Name From RealEstateAgentInfo Where RealEstateAgentID={i}").Rows[0]["Name"].ToString();
            return View(lst[0]);

        }
        [HttpPost]
        public ActionResult SignUp(RealEstateAgentInfo collection)
        {
            DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.Username}' union Select UserName from LenderInfo Where UserName = '{collection.Username}'union Select UserName from USerInfo Where UserName = '{collection.Username}'");
            if (dt.Rows.Count > 0)
            {
                ViewBag.Organization = new DropDown().GetOrganizationList("", collection.OrganizationID);
                ViewBag.ZipCode = new DropDown().GetZipCode("", collection.ZipCodeID);
                ViewBag.UserInformation = "You Have Been Invited by " + General.FetchData($@"Select (FirstName+' '+LastName)Name From RealEstateAgentInfo Where RealEstateAgentID={collection.UserID}").Rows[0]["Name"].ToString();
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
        public ActionResult ApprovalStatus(int RealEstateAgentID, int Approved, string ApprovalRemarks)
        {
            string sql = $@"Update RealEstateAgentInfo Set IsApproved={Approved} , ApprovedRemarks='{ApprovalRemarks}' Where RealEstateAgentID={RealEstateAgentID}";
            General.ExecuteNonQuery(sql);
            return Json("true,");
        }
        //  RealEstateAgentInfo/APIConfiguration
        public ActionResult APIConfiguration()
        {
            ViewBag.Alert = "";
            List<APIConfigInfo> lst = new List<APIConfigInfo>();
            lst = General.ConvertDataTable<APIConfigInfo>(General.FetchData($@" select APITypeInfo.APITypeTitle,APIConfigInfo.* from APITypeInfo
	left outer join APIConfigInfo on APIConfigInfo.TypeID=APITypeInfo.APITypeID  and RealEstateID=" + General.UserID));
            return View(lst);
        }
        [HttpPost]
        public ActionResult APIConfiguration(List<APIConfigInfo> collection)
        {
            //if (collection.Where(a => a.APIConfig == string.Empty).Count()>1)
            //{
            //    ViewBag.Alert = "";
            //    return View(collection);
            //}
            collection[0].DeleteRecords();
            foreach (APIConfigInfo item in collection)
            {
                if (!string.IsNullOrEmpty(item.APIConfig))
                    item.InsertRecords();
            }
            return Json("true");
        }

        // APIS
        [HttpGet]
        public JsonResult GetRealEstateInfo()
        {
            List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(Model.GetAllRecord());
            Dictionary<string, object> JSResponse = new Dictionary<string, object>();
            if (JSResponse == null)
            {
                JSResponse.Add("Status", false);
            }
            else
            {
                JSResponse.Add("Status", true);
            }
            JSResponse.Add("Message", "Real Estate Information");
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
        [HttpPost]
        public JsonResult PostRealEstateAgentInfo(RealEstateAgentInfo collection)
        {
            DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.Username}' union Select UserName from LenderInfo Where UserName = '{collection.Username}'union Select UserName from USerInfo Where UserName = '{collection.Username}'");
            if (dt.Rows.Count > 0)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                if (JSResponse == null)
                {
                    JSResponse.Add("Status", false);
                }
                else
                {
                    JSResponse.Add("Status", true);
                }
                JSResponse.Add("Message", "UserName Already Exist Please Use different UserName");
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
            int RealEstateAgentId = Model.InsertRecord(collection);
            if (RealEstateAgentId == 0)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                if (JSResponse == null)
                {
                    JSResponse.Add("Status", false);
                }
                else
                {
                    JSResponse.Add("Status", true);
                }
                JSResponse.Add("Message", "Unabled to Insert Records... Please contact to the administration!");
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
            else
            {
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(Model.GetAllRecord(" where RealEstateAgentId=" + RealEstateAgentId));
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                if (JSResponse == null)
                {
                    JSResponse.Add("Status", false);
                }
                else
                {
                    JSResponse.Add("Status", true);
                }
                JSResponse.Add("Message", "Data Saved Successfully!");
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

        }
        [HttpPost]
        public JsonResult UpdateRealEstateAgentInfo(RealEstateAgentInfo collection)
        {
            DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.Username}' and RealEstateAgentId!={collection.RealEstateAgentId} union Select UserName from LenderInfo Where UserName = '{collection.Username}'union Select UserName from USerInfo Where UserName = '{collection.Username}'");
            if (dt.Rows.Count > 0)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                if (JSResponse == null)
                {
                    JSResponse.Add("Status", false);
                }
                else
                {
                    JSResponse.Add("Status", true);
                }
                JSResponse.Add("Message", "UserName Already Exist Please Use different UserName");
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
            int RealEstateAgentId = Model.UpdateRecord(collection);
            if (RealEstateAgentId == 0)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                if (JSResponse == null)
                {
                    JSResponse.Add("Status", false);
                }
                else
                {
                    JSResponse.Add("Status", true);
                }
                JSResponse.Add("Message", "Unabled to Update Records... Please contact to the administration!");
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
            else
            {
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(Model.GetAllRecord(" where RealEstateAgentId=" + RealEstateAgentId));
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                if (JSResponse == null)
                {
                    JSResponse.Add("Status", false);
                }
                else
                {
                    JSResponse.Add("Status", true);
                }
                JSResponse.Add("Message", "Data Updated Successfully!");
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

        }

    }
}