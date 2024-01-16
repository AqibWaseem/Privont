using Privont.Models;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using static Privont.General;

namespace Privont.Controllers
{
    public class SMSSettingController : Controller
    {
        SMSSetting ModelSettingSMS=new SMSSetting();
        // GET: SMSSetting
        public ActionResult Index()
        {
            SMSSetting obj = new SMSSetting();
            DataTable dt = General.FetchData($@"Select * from SMSSetting Where UserID={General.UserID}");
            obj = General.ConvertDataTable<SMSSetting>(dt)[0];
            
            return View(obj);
        }
        [HttpPost]
        public ActionResult Index(SMSSetting collection)
        {
            string sql = $@"Delete from SMSSetting Where UserID = {General.UserID} and UserType={General.UserType}";
             sql = sql+ $@"  Insert into SMSSetting (SMSDetail,SMSDetailInvite,UserID,SMSSubKey,SMSDetailSub,UserType) values ('{collection.SMSDetail}','{collection.SMSDetailInvite}',{General.UserID}'{collection.SMSSubKey}','{collection.SMSDetailSub}',{General.UserType})";
            General.ExecuteNonQuery(sql);
            return View("Index");
        }
        public ActionResult SendSms(int LeadID,string Link)
        {

            DataTable dt= General.FetchData($@"Select SMSDetail from SMSSetting Where UserID={General.UserID}");
            if(dt.Rows.Count<=0)
            {
                return Json("1,");
            }
            string SMSDetail = dt.Rows[0]["SMSDetail"].ToString();
            DataTable LeadDetail = General.FetchData($@"Select * from LeadInfo Where LeadID ={LeadID}");
            LeadInfo obj = new LeadInfo();
            obj.PhoneNo = LeadDetail.Rows[0]["PhoneNo"].ToString().Trim().Replace("-", "").Replace("_", "").Replace(",", "");
            obj.FirstName = LeadDetail.Rows[0]["FirstName"].ToString();
            obj.LastName = LeadDetail.Rows[0]["LastName"].ToString();
            SMSDetail = SMSDetail.Replace("[Name]", obj.FirstName+" "+obj.LastName);
            SMSDetail = SMSDetail.Replace("[Link]", Link);
            return Json("true," + SMSDetail + "," + obj.PhoneNo);
        }
       
        public string SendSmsString(int LeadID, string Link,int UserID)
        {
            DataTable dt = General.FetchData($@"Select SMSDetail from SMSSetting Where UserID={UserID}");
            if (dt.Rows.Count <= 0)
            {
                return "1,";
            }
            string SMSDetail = dt.Rows[0]["SMSDetail"].ToString();
            DataTable LeadDetail = General.FetchData($@"Select * from LeadInfo Where LeadID ={LeadID}");
            LeadInfo obj = new LeadInfo();
            obj.PhoneNo = LeadDetail.Rows[0]["PhoneNo"].ToString().Trim().Replace("-", "").Replace("_", "").Replace(",", "");
            obj.FirstName = LeadDetail.Rows[0]["FirstName"].ToString();
            obj.LastName = LeadDetail.Rows[0]["LastName"].ToString();
            SMSDetail = SMSDetail.Replace("[Name]", obj.FirstName + " " + obj.LastName);
            SMSDetail = SMSDetail.Replace("[Link]", Link);
            return $@"2,{SMSDetail},{obj.PhoneNo}";
        }
        [HttpGet]
        public ActionResult GetSMSSettings(int UserID,int UserType)
        {
            try
            {
                DataTable dt = ModelSettingSMS.GetSMSDetailsGetSMSDetails(UserID, UserType);
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "SMS Settings Information!");
                JSResponse.Add("Data", dbrows);

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
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Error: " + ex.Message);
                JSResponse.Add("Data", DBNull.Value);

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
        }
        public bool GetKeyValidationResult(string Key, int UserID, int UserType)
        {
            string Query = $@"select count(*) Exist from SMSSetting where (SMSSubKey = '{Key.Trim()}' or SMSDetailKey='{Key.Trim()}') and UserID != {UserID} and UserType = {UserType} ";
         
            bool flag = false;
            DataTable dt = General.FetchData(Query);
            if (dt.Rows.Count > 0)
            {
                if (int.Parse(dt.Rows[0][0].ToString())>0)
                    flag = true;
                else
                    flag = false;
            }
            return flag;
        }
        [HttpGet]
        public ActionResult GetKeyValidation(string Key, int UserID,int UserType)
        {
            try
            {
               
                bool flag = GetKeyValidationResult( Key,  UserID,  UserType);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "SMS Settings Information!");
                JSResponse.Add("Data", flag);

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
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Error: " + ex.Message);
                JSResponse.Add("Data", DBNull.Value);

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
        }
        [HttpGet]
        public JsonResult GetSMSSettingsInfo(int UserID, int UserType)
        {
            try
            {

                string sql = $@"select * from SMSSetting where UserID={UserID} and UserTYpe={UserType}";
                DataTable dt = General.FetchData(sql);
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "SMS Settings Information!");
                JSResponse.Add("Data", dbrows);

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
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Error: "+ex.Message);
                JSResponse.Add("Data", DBNull.Value);

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
        }
        [HttpPost]
        public JsonResult SaveRecord(SMSSetting collection)
        {
            try
            {
                string sql = $@"Delete from SMSSetting Where UserID = {collection.UserID} and UserType={collection.UserType}";
                sql = sql + $@"  Insert into SMSSetting (SMSDetail,SMSDetailInvite,UserID,SMSSubKey,SMSDetailSub,UserType,SMSDetailKey)

values (
'{collection.SMSDetail}',
'{collection.SMSDetailInvite}',
 {collection.UserID},
'{collection.SMSSubKey}',
'{collection.SMSDetailSub}',
{collection.UserType},
'{collection.SMSDetailKey}')";
                General.ExecuteNonQuery(sql);
                DataTable dt = ModelSettingSMS.GetSMSDetailsGetSMSDetails(collection.UserID, collection.UserType);
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Saved Successfully!");
                JSResponse.Add("Data", dbrows);

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
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Error: "+ex.Message);
                JSResponse.Add("Data", DBNull.Value);

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
        }
    }
}