using PrivontAdmin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrivontAdmin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.Request.Cookies["UserID"] == null)
            {
                return RedirectToAction("Login");
            }

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
        [HttpGet]
        public ActionResult Login()
        {
            if (System.Web.HttpContext.Current.Request.Cookies["UserID"] != null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = "";
            UserInfo obj = new UserInfo();
            return View(obj);
        }
        [HttpPost]
        public ActionResult Login(UserInfo obj)
        {
            string Query = $@"
                select * from UserInfo where UserName like '{obj.UserName}' and Password = '{obj.Password}'";
            DataTable dtVerify = General.FetchData(Query);
            ViewBag.Message = "";
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
                Response.Cookies["Name"].Value = dtVerify.Rows[0]["Name"].ToString();
                return RedirectToAction("index");
            }
            return View(obj);
        }
        public ActionResult Logout()
        {
            Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["Name"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Login");
        }
        public ActionResult PageNotFound()
        {
            return View();
        }
    }
}