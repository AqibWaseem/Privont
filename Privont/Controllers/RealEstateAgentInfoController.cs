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
                    DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.username}' and RealEstateAgentId!={collection.RealEstateAgentId} union Select UserName from LenderInfo Where UserName = '{collection.username}'union Select UserName from USerInfo Where UserName = '{collection.username}'");
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
            List<RealEstateAgentInfo> lst = General.ConvertDataTable<RealEstateAgentInfo>(Model.GetAllRecordforSignup(" where RealEstateAgentID=" + y));
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
            DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.username}' union Select UserName from LenderInfo Where UserName = '{collection.username}'union Select UserName from USerInfo Where UserName = '{collection.username}'");
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
            DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.username}' union Select UserName from LenderInfo Where UserName = '{collection.username}'union Select UserName from USerInfo Where UserName = '{collection.username}'");
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
            DataTable dt = General.FetchData($@"Select USerName from RealEstateAgentInfo Where UserName = '{collection.username}' and RealEstateAgentId!={collection.RealEstateAgentId} union Select UserName from LenderInfo Where UserName = '{collection.username}'union Select UserName from USerInfo Where UserName = '{collection.username}'");
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

        //Update User Profile Information
        [HttpPost]
        public JsonResult UpdateUserProfile(RealEstateAgentInfo collection)
        {
            DataTable dt = new RealEstateAgentInfo().UserExistanceInfo(collection.username);
            if (dt.Rows.Count > 0)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", 401);
                JSResponse.Add("Message", "Username Already Exist Please Use different Username!");
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
            int RealEstateAgentId = 0;
            if (collection.UserType == 2)
            {
                RealEstateAgentId = Model.UpdateProfileRecord(collection);
            }
            else if (collection.UserType == 3)
            {
                LenderInfo LenderCollection = new LenderInfo();
                General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, LenderInfo>(collection, LenderCollection);
                RealEstateAgentId = new LenderInfo().UpdateProfileRecord(LenderCollection);
            }
            else if (collection.UserType == 4)
            {
                LeadInfo LeadInfoCollection = new LeadInfo();
                General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, LeadInfo>(collection, LeadInfoCollection);
                RealEstateAgentId = new LeadInfo().UpdateProfileRecord(LeadInfoCollection);
            }
            else if (collection.UserType == 5)
            {
                VendorInfo VendorInfoCollection = new VendorInfo();
                General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, VendorInfo>(collection, VendorInfoCollection);
                RealEstateAgentId = new VendorInfo().UpdateProfileRecord(VendorInfoCollection);
            }
            if (RealEstateAgentId == 0)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.BadRequest);
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
                List<Dictionary<string, object>> dbrows = new List<Dictionary<string, object>>();
                if (collection.UserType == 2)
                {
                    dbrows = new General().GetAllRowsInDictionary(Model.GetAllRecord(" and RealEstateAgentId=" + RealEstateAgentId));
                }
                else if (collection.UserType == 3)
                {
                    dbrows = new General().GetAllRowsInDictionary(new LenderInfo().GetAllRecord(" and LenderId=" + RealEstateAgentId));
                }
                else if (collection.UserType == 4)
                {
                    dbrows = new General().GetAllRowsInDictionary(new LeadInfo().GetAllRecord(" and LeadID=" + RealEstateAgentId));
                }
                else if (collection.UserType == 5)
                {
                    dbrows = new General().GetAllRowsInDictionary(new VendorInfo().GetAllRecord(" and VendorID=" + RealEstateAgentId));
                }

                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
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
        public JsonResult UserProfileInfo(int UserID, int UserType)
        {
            try
            {
                JsonResult jr = new JsonResult();
                string Query = "";
                DataTable dt = new DataTable();
                DataTable dtSocialMedia = new DataTable();
                List<Dictionary<string, object>> dbrows = new List<Dictionary<string, object>>();
                List<Dictionary<string, object>> dbSocialMedia = new List<Dictionary<string, object>>();
                if (UserType == 1 || UserType > 5)
                {
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "This User not exist!", null);
                }
                else if (UserType == 2)//
                {
                    Query = $@"select RealEstateAgentInfo.*,OrganizationInfo.OrganizationTitle,ZipCode.ZipCode
 from RealEstateAgentInfo
inner join OrganizationInfo on RealEstateAgentInfo.OrganizationID = OrganizationInfo.OrganizationID
 inner join ZipCode on RealEstateAgentInfo.ZipCodeID = ZipCode.ZipCodeID
 Where UserName is not null and RealEstateAgentId=" + UserID;
                    dt = General.FetchData(Query);

                     dtSocialMedia=General.FetchData($@"SELECT        SocialMediaInfo.SocialMediaID, SocialMediaInfo.SocialMediaTitle, UserSocialMediaInfo.ProfileName, 
UserSocialMediaInfo.ProfileLink, UserSocialMediaInfo.UserID, 
UserSocialMediaInfo.UserTypeID 
                         
FROM            SocialMediaInfo LEFT OUTER JOIN
                         UserSocialMediaInfo ON SocialMediaInfo.SocialMediaID = UserSocialMediaInfo.SocialMediaID
						 where UserSocialMediaInfo.UserID={UserID} and UserTypeID={UserType}");
                    dbSocialMedia = new General().GetAllRowsInDictionary(dtSocialMedia);


                    dbrows = new General().GetAllRowsInDictionary(dt);

                    //jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "User Profile Information!", dbrows);
                    Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "User Profile Information!");
                    JSResponse.Add("Data", dbrows);
                    JSResponse.Add("SocialMedia", dbSocialMedia);

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
                else if (UserType == 3)//
                {
                    Query = $@"Select LenderInfo.*,OrganizationInfo.OrganizationTitle,ZipCode.ZipCode from LenderInfo 
left outer join OrganizationInfo on LenderInfo.OrganizationID = OrganizationInfo.OrganizationID
left outer join ZipCode on LenderInfo.ZipCodeID = ZipCode.ZipCodeID

 Where UserName is not null and LenderId=" + UserID;
                    dt = General.FetchData(Query);
                    dtSocialMedia = General.FetchData($@"SELECT        SocialMediaInfo.SocialMediaID, SocialMediaInfo.SocialMediaTitle, UserSocialMediaInfo.ProfileName, 
UserSocialMediaInfo.ProfileLink, UserSocialMediaInfo.UserID, 
UserSocialMediaInfo.UserTypeID 
                         
FROM            SocialMediaInfo LEFT OUTER JOIN
                         UserSocialMediaInfo ON SocialMediaInfo.SocialMediaID = UserSocialMediaInfo.SocialMediaID
						 where UserSocialMediaInfo.UserID={UserID} and UserTypeID={UserType}");
                    dbSocialMedia = new General().GetAllRowsInDictionary(dtSocialMedia);


                    dbrows = new General().GetAllRowsInDictionary(dt);

                    //jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "User Profile Information!", dbrows);
                    Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "User Profile Information!");
                    JSResponse.Add("Data", dbrows);
                    JSResponse.Add("SocialMedia", dbSocialMedia);

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
                else if (UserType == 4)
                {
                    Query = $@"select * from LeadInfo where LeadID=" + UserID;
                    dt = General.FetchData(Query);
                    dtSocialMedia = General.FetchData($@"SELECT        SocialMediaInfo.SocialMediaID, SocialMediaInfo.SocialMediaTitle, UserSocialMediaInfo.ProfileName, 
UserSocialMediaInfo.ProfileLink, UserSocialMediaInfo.UserID, 
UserSocialMediaInfo.UserTypeID 
                         
FROM            SocialMediaInfo LEFT OUTER JOIN
                         UserSocialMediaInfo ON SocialMediaInfo.SocialMediaID = UserSocialMediaInfo.SocialMediaID
						 where UserSocialMediaInfo.UserID={UserID} and UserTypeID={UserType}");
                    dbSocialMedia = new General().GetAllRowsInDictionary(dtSocialMedia);


                    dbrows = new General().GetAllRowsInDictionary(dt);

                    //jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "User Profile Information!", dbrows);
                    Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "User Profile Information!");
                    JSResponse.Add("Data", dbrows);
                    JSResponse.Add("SocialMedia", dbSocialMedia);

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
                else if (UserType == 5)
                {
                    Query = $@"select * from VendorInfo where VendorInfo.VendorID=" + UserID;
                    dt = General.FetchData(Query);
                    dtSocialMedia = General.FetchData($@"SELECT        SocialMediaInfo.SocialMediaID, SocialMediaInfo.SocialMediaTitle, UserSocialMediaInfo.ProfileName, 
UserSocialMediaInfo.ProfileLink, UserSocialMediaInfo.UserID, 
UserSocialMediaInfo.UserTypeID 
                         
FROM            SocialMediaInfo LEFT OUTER JOIN
                         UserSocialMediaInfo ON SocialMediaInfo.SocialMediaID = UserSocialMediaInfo.SocialMediaID
						 where UserSocialMediaInfo.UserID={UserID} and UserTypeID={UserType}");
                    dbSocialMedia = new General().GetAllRowsInDictionary(dtSocialMedia);


                    dbrows = new General().GetAllRowsInDictionary(dt);

                    //jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "User Profile Information!", dbrows);
                    Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "User Profile Information!");
                    JSResponse.Add("Data", dbrows);
                    JSResponse.Add("SocialMedia", dbSocialMedia);

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
                return jr;

            }
            catch (Exception ex)
            {

                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Error: "+ex.Message);
                JSResponse.Add("Data", DBNull.Value);
                JSResponse.Add("SocialMedia", DBNull.Value);

                var jr = new JsonResult()
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