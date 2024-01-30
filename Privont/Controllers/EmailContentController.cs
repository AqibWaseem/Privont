using Privont.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using static Privont.General;
using TrueDialog.Model;

namespace Privont.Controllers
{
    public class EmailContentController : Controller
    {
        // GET: EmailContent
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(string ToEmail,string Subject,string Body="")
        {
            string generatedLink = "";
            generatedLink = new GeneralApisController().GenerateLink(3, 2, 1, 2, LinkTypes.Refer);
            var res = new InvitationReferenceController().SendEmail(Subject, "Abdullah", ToEmail, generatedLink, 2, 1, 2);
            return View();
        }
    }
}