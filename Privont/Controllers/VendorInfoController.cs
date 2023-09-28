using Privont.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}