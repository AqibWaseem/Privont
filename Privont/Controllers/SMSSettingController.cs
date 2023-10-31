using Privont.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class SMSSettingController : Controller
    {
        // GET: SMSSetting
        public ActionResult Index()
        {
            SMSSetting obj = new SMSSetting();
            DataTable dt = General.FetchData($@"Select SMSDetail,SMSDetailInvite from SMSSetting Where UserID={General.UserID}");
            if(dt.Rows.Count>0)
            {
                obj.SMSDetail = dt.Rows[0]["SMSDetail"].ToString();
                obj.SMSDetailInvite = dt.Rows[0]["SMSDetailInvite"].ToString();
            }
            return View(obj);
        }
        [HttpPost]
        public ActionResult Index(SMSSetting collection)
        {
            string sql = $@"Delete from SMSSetting Where UserID = {General.UserID}";
             sql = sql+ $@"  Insert into SMSSetting (SMSDetail,SMSDetailInvite,UserID) values ('{collection.SMSDetail}','{collection.SMSDetailInvite}',{General.UserID})";
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
    }
}