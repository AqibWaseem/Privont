using Privont.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class HomeController : Controller
    {
        UserInfo ModelUserInfo = new UserInfo();
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.Request.Cookies["UserID"] == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.Lead = General.FetchData("select Count(LeadID)LeadID from LeadInfo").Rows[0]["LeadID"].ToString();
            ViewBag.RealEstateAgent = General.FetchData("Select Count(RealEstateAgentID)RealEstateAgentID from RealEstateAgentInfo").Rows[0]["RealEstateAgentID"].ToString();
            ViewBag.Vendor = General.FetchData("Select Count(VendorID)VendorID from VendorInfo").Rows[0]["VendorID"].ToString();
            ViewBag.Lender = General.FetchData("Select Count(LenderID)LenderID from LenderInfo").Rows[0]["LenderID"].ToString();

            if(General.UserType==3)
            {
                return RedirectToAction("Index", "LeadInfo");
            }
            return View();
        }
        public ActionResult Login()
        {
            if (System.Web.HttpContext.Current.Request.Cookies["UserID"] != null)
            {
                return RedirectToAction("Index");
            }
            UserInfo obj = new UserInfo();
            ViewBag.Message = "";
            return View(obj);
        }
        [HttpPost]
        public ActionResult Login(UserInfo obj)
        {

            DataTable dtVerify = ModelUserInfo.GetAllRecord_DataTable($@" where UserName='{obj.UserName}' and Password='{obj.Password}'");
            if (dtVerify.Rows.Count == 0)
            {
                ViewBag.Message = "Invalid Username and Password!";
            }
            else
            {
                bool flag = false;
                bool.TryParse(dtVerify.Rows[0]["Inactive"].ToString(), out flag);
                if (flag == true)
                {
                    ViewBag.Message = "Unabled to login. Please contact to Administration!";
                    return View(obj);
                }
                Response.Cookies["UserID"].Value = dtVerify.Rows[0]["UserID"].ToString(); 
                Response.Cookies["UserType"].Value = dtVerify.Rows[0]["UserType"].ToString();
                return RedirectToAction("index");
            }
            return View(obj);
        }
        public ActionResult Logout()
        {
            Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["UserType"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Login");
        }
        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}