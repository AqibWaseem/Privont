using Privont.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrueDialog.Model;

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
        [HttpPost]
        public JsonResult POSTSocialMediaInfo(List<SocialMediaInfo> collection)
        {
            DataTable dt = new DataTable();

            bool response = InsertSocialMediaVIAUserIDandUserType(collection);
            if (response)
            {
                dt = General.FetchData($@"SELECT        SocialMediaInfo.SocialMediaID, SocialMediaInfo.SocialMediaTitle, UserSocialMediaInfo.ProfileName, 
UserSocialMediaInfo.ProfileLink, UserSocialMediaInfo.UserID, 
UserSocialMediaInfo.UserTypeID 
                         
FROM            SocialMediaInfo LEFT OUTER JOIN
                         UserSocialMediaInfo ON SocialMediaInfo.SocialMediaID = UserSocialMediaInfo.SocialMediaID
						 where UserSocialMediaInfo.UserID={collection[0].UserID} and UserTypeID={collection[0].UserTypeID}");
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "User Social Media Information");
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
            else
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Unabled to insert social media records.");
                JSResponse.Add("Data", DBNull.Value);

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
        public bool InsertSocialMediaVIAUserIDandUserType(List<SocialMediaInfo> collection,int UserID=0,int UserTypeID=0)
        {
            try
            {
                string Query = $@" delete from UserSocialMediaInfo where UserTypeID=" + UserTypeID + " and UserID=" + UserID;
                foreach (SocialMediaInfo item in collection)
                {
                    if(!string.IsNullOrEmpty(item.ProfileLink))
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
           ,{UserID}
           ,{UserTypeID})";
                        Query = Query + Environment.NewLine;
                    }
                   
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
