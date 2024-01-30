using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static Privont.General;

namespace Privont.Controllers
{
    public class GenerateLinkController : Controller
    {
        // GET: GenerateLink
        [HttpGet]
        public JsonResult Link(int UserID, int UserType, int EntryUserID, int EntryUserType)
        {
            string GeneratedLink = new GeneralApisController().GenerateLink(UserID, UserType, EntryUserID, EntryUserType, LinkTypes.Refer);
            Dictionary<string, object> JSResponse = new Dictionary<string, object>();
            JSResponse.Add("Status", 200);
            JSResponse.Add("Message", "Link");
            JSResponse.Add("Data", GeneratedLink);

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