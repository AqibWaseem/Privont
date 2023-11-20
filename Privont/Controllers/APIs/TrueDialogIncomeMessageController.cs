using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Privont.Models;
using System.Web.Mvc;

namespace Privont.Controllers.APIs
{
    public class TrueDialogIncomeMessageController : ApiController
    {
        [HttpPost]
        [Route("api/TrueDialog/IncomingMessageCallback")]
        public HttpResponseMessage IncomingMessageCallback(JObject jObject)
        {
            try
            {
                TrueDialogIncomeMessage value = new TrueDialogIncomeMessage();
                //JArray userArray = (JArray)jObject["value"];
                value = jObject["value"].ToObject<TrueDialogIncomeMessage>();
                string QueryInsert = $@"INSERT INTO TrueDialogIncomeMessage
           (CallbackTimestamp
           ,CallbackToken
           ,CallbackType
           ,CallbackURL
           ,TransactionId
           ,AccountId
           ,AccountName
           ,MessageId
           ,Message
           ,ChannelId
           ,ChannelCode
           ,ContactId
           ,PhoneNumber)
     VALUES
           ('{value.CallbackTimestamp}'
           ,'{value.CallbackToken}'
           ,'{value.CallbackType}'
           ,'{value.CallbackURL}'
           ,'{value.TransactionId}'
           ,'{value.AccountId}'
           ,'{value.AccountName}'
           ,'{value.MessageId}'
           ,'{value.Message}'
           ,'{value.ChannelId}'
           ,'{value.ChannelCode}'
           ,'{value.ContactId}'
           ,'{value.PhoneNumber}')";
                General.ExecuteNonQuery(QueryInsert);
                JObject newjson = new JObject(
                          new JProperty("Status", Convert.ToInt32(HttpStatusCode.OK)),
                          new JProperty("Message", "Income Message Received!"),
                          new JProperty("Data", DBNull.Value)
                       );
                return Request.CreateResponse(HttpStatusCode.OK, newjson, "application/json");
            }
            catch
            {
                JObject usermsg = new JObject(
                                  new JProperty("Status", Convert.ToInt32(HttpStatusCode.BadRequest)),
                                  new JProperty("Message", "Request format not correct"),
                                  new JProperty("Data", DBNull.Value)
                                  );
                return Request.CreateResponse(HttpStatusCode.OK, usermsg, "application/json");
            }
        }
    }
}