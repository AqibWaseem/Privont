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
    public class CardInfoController : Controller
    {
        // GET: CardInfo
        [HttpGet]
        public JsonResult GetCardInfo(int UserID,int UserType)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = General.FetchData($@"     SELECT        CardTypeInfo.CardIcon, CardTypeInfo.CardTypeTitle, CardInfo.CardID, CardInfo.CardNumber, CardInfo.CardHolderName, CardInfo.ExpiryDate, CardInfo.CVV, CardInfo.CardTypeID, CardInfo.UserID, CardInfo.UserType
FROM            CardInfo INNER JOIN
                         CardTypeInfo ON CardInfo.CardTypeID = CardTypeInfo.CardTypeID

						 where CardInfo.UserID={UserID} and CardInfo.UserType={UserType} ");
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Card Information");
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
            catch
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Card Information");
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
        [HttpGet]
        public JsonResult GetCardTypeInfo()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = General.FetchData($@" select * from CardtypeINfo");
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Card Type Information");
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
            catch
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Card Type Information");
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
        [HttpPost]
        public JsonResult PostCardInfo(CardInfo collection)
        {
            try
            {
                JsonResult jr = new JsonResult();
                //Get Card Validation Information
                var api = new EasyPayPaymentIntegreation();
                var validateResponse = api.ValidateCard(collection.CardNumber, collection.ExpiryDate.ToString("yyMM"), collection.CVV.ToString());
                if (!validateResponse.IsSuccess)
                {
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.NotFound, "Card are not valid", null);
                    return jr;
                }
                General.ExecuteNonQuery($@"Delete from CardInfo where CardInfo.UserID={collection.UserID} and CardInfo.UserType={collection.UserType} ");
                General.ExecuteNonQuery($@"INSERT INTO [dbo].[CardInfo]
           ([CardNumber]
           ,[CardHolderName]
           ,[ExpiryDate]
           ,[CVV]
           ,[CardTypeID]
           ,[UserID]
           ,[UserType])
     VALUES
           ('{collection.CardNumber}'
           ,'{collection.CardHolderName}'
           ,'{collection.ExpiryDate}'
           ,'{collection.CVV}'
           ,'{collection.CardTypeID}'
           ,'{collection.UserID}'
           ,'{collection.UserType}')");
                DataTable dt = new DataTable();
                dt = General.FetchData($@"     SELECT        CardTypeInfo.CardIcon, CardTypeInfo.CardTypeTitle, CardInfo.CardID, CardInfo.CardNumber, CardInfo.CardHolderName, CardInfo.ExpiryDate, CardInfo.CVV, CardInfo.CardTypeID, CardInfo.UserID, CardInfo.UserType
FROM            CardInfo INNER JOIN
                         CardTypeInfo ON CardInfo.CardTypeID = CardTypeInfo.CardTypeID

						 where CardInfo.UserID={collection.UserID} and CardInfo.UserType={collection.UserType} ");
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Card Information");
                JSResponse.Add("Data", dbrows);

                jr = new JsonResult()
                {
                    Data = JSResponse,
                    ContentType = "application/json",
                    ContentEncoding = System.Text.Encoding.UTF8,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
                return jr;
            }
            catch
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Card Information");
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
    }
}