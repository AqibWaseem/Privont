using Privont.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class UserInfoController : Controller
    {
        UserInfo Model=new UserInfo();
        // GET: UserInfo
        public ActionResult Index()
        {
            List<UserInfo> list = new List<UserInfo>();
            list=General.ConvertDataTable<UserInfo>(Model.GetUserAllRecord_DataTable());
            return View(list);
        }

        // GET: UserInfo/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserInfo/Create
        public ActionResult Create()
        {
            UserInfo obj=new UserInfo();
            return View(obj);
        }
        public ActionResult SignUp()
        {
            UserInfo obj = new UserInfo();
            return View(obj);
        }
        // POST: UserInfo/Create
        [HttpPost]
        public ActionResult SaveRecord(UserInfo collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (collection.UserID == 0)
                {
                    DataTable dt = General.FetchData($@"Select * from UserInfo Where UserName ='{collection.UserName}'");
                    if (dt.Rows.Count>0)
                    {
                        return Json("1,");
                    }
                    int id = Model.InsertRecord(collection);
                }
                else
                {
                   int id = Model.UpdateRecord(collection);
                }
                return Json("true");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserInfo/Edit/5
        public ActionResult Edit(int id)
        {
            List<UserInfo> list = new List<UserInfo>();
            list = General.ConvertDataTable<UserInfo>(Model.GetUserAllRecord_DataTable(" where UserID="+id));
            return View("SignUp", list[0]);
        }

        // POST: UserInfo/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserInfo/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string Query = $@"delete from UserInfo where UserID="+id;
            General.ExecuteNonQuery(Query);
            return Json("true");
        }

        // POST: UserInfo/Delete/5
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
