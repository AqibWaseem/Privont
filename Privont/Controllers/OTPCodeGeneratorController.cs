using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Privont.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using TrueDialog.Model;

namespace Privont.Controllers
{
    public class OTPCodeGeneratorController : Controller
    {
        OTPCodeGenerator ModelOTP = new OTPCodeGenerator();
        // GET: OTPCodeGenerator
        [HttpPost]
        public JsonResult VerifiedOTP(string Username, int SMSOTP, int EmailOTP)
        {
            try
            {
                JsonResult jr = new JsonResult();
                DataTable dt = ModelOTP.VerifyOTP(Username, SMSOTP, EmailOTP);
                if(dt.Rows.Count == 0)
                {
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Invalid OTP!, Please try again", null);
                    return jr;
                }
                DateTime sentdate;
                sentdate = Convert.ToDateTime(dt.Rows[0]["SentDate"]);
                DateTime serverdate = Convert.ToDateTime(General.FetchData("select Getdate()").Rows[0][0]);
                TimeSpan totaltime = serverdate - sentdate;
                int mins = totaltime.Minutes;
                if (mins <= 10)
                {
                    // Return User Information
                    string WhereClause = $@" where Username='{Username}'";
                    dt = OTPCodeGenerator.GetUserDetails(WhereClause);
                    List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "OTP!", dbrows);
                    return jr;
                }
                else
                {
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Your OTP has expired!, Please try again.", null);
                }
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
        [HttpPost]
        public JsonResult SendOTP(string Username,int RequestedBy)
        {
            try
            {
                //RequestedBy == 1 ? PhoneNo : Email 
                JsonResult jr = new JsonResult();
                string WhereClause = $@" where Username='{Username}'";
                DataTable dt = OTPCodeGenerator.GetUserDetails(WhereClause);
                if (dt.Rows.Count == 0)
                {
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "User not exist!", null);
                    return jr;
                }
                int UserID = int.Parse(dt.Rows[0]["UserID"].ToString());
                int UserType = int.Parse(dt.Rows[0]["UserType"].ToString());
                dt = ModelOTP.SendOTPCode( UserID,  UserType, RequestedBy);
                if(dt.Rows.Count > 0) 
                {
                    List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "OTP Information!", dbrows);
                }
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