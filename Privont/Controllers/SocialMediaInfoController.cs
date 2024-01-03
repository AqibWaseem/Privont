using Privont.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class SocialMediaInfoController : Controller
    {
        //SocialMediaInfo
        [HttpGet]
        public JsonResult GetSocialMediaInfo()
        {
            DataTable dt = General.FetchData($@"select * from SocialMediaInfo");
            List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
            Dictionary<string, object> JSResponse = new Dictionary<string, object>();
           
            JSResponse.Add("Status", HttpStatusCode.OK);
            JSResponse.Add("Message", "Social Media Information");
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
        public bool InsertSocialMediaVIAUserIDandUserType(List<UserSocialMediaInfo> collection)
        {
            try
            {
                string Query = $@" delete from UserSocialMediaInfo where UserTypeID=" + collection[0].UserTypeID + " and UserID=" + collection[0].UserID;
                foreach (UserSocialMediaInfo item in collection)
                {
                    Query = Query + Environment.NewLine;
                    Query = Query + $@"      INSERT INTO [dbo].[UserSocialMediaInfo]
           ([SocialMediaID]
           ,[ProfileName]
           ,[ProfileLink]
           ,[UserID]
           ,[UserTypeID])
     VALUES
           ({item.SocialMediaID}
           ,'{item.ProfileName}'
           ,'{item.ProfileLink}'
           ,{item.UserID}
           ,{item.UserTypeID})";
                    Query = Query + Environment.NewLine;
                }
                General.ExecuteNonQuery(Query);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        } 
    }
}
