using Newtonsoft.Json.Linq;
using Privont.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Privont.Controllers
{
    public class SMSTrueDialogController : Controller
    {
        // GET: TrueDialog
        //public readonly TrueDialogService trueDialogService;

        //public SMSTrueDialogController(TrueDialogService trueDialogService)
        //{
        //    this.trueDialogService = trueDialogService;
        //}
        //[HttpPost]
        //public ActionResult Index(string PhoneNo, string message)
        //{
        //    var result = trueDialogService.SendMessage("" + PhoneNo + "", "" + message + "");
        //    // Handle the result
        //    return Json("true");
        //}
        //[HttpPost]
        //public async Task<JsonResult> SMSAPI(string PhoneNo, string message)
        //{
        //    var result = await new General().trueDialogService.SendMessage("" + PhoneNo + "", "" + message + "");
        //    return Json("true");
        //}

        //public async Task<ActionResult> SMSAPIAsync(string PhoneNo, string message)
        //{
        //    var baseAddress = new Uri("https://api.truedialog.com/api/v2.1/");

        //    using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        //    {
        //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", "Basic ABC123");
        //        using (var content = new StringContent("{  \"Channels\": [    22  ],  \"Targets\": [    \"Phone Number\"  ],  \"Message\": \"Hello from Apiary!\",  \"Execute\": true}", System.Text.Encoding.Default, "application/json"))
        //        {
        //            using (var response = await httpClient.PostAsync("account/{AccountId}/action-pushcampaign", content))
        //            {
        //                string responseData = await response.Content.ReadAsStringAsync();
        //            }
        //        }
        //    }
        //    return Json("true");
        //}
        
        private const string BaseUrl = "https://api.truedialog.com/api/v2.1/";
        private const string ApiKey = "42e7b9be42864ab9a417ac09310c61bb"; // Replace with your API key
        private const string ApiSecret = "Cs6!a5+XEr9?"; // Replace with your API secret   
        private const int AccountID = 22965;
        [HttpPost]
        public async Task<ActionResult> SendPushCampaignAsync(string PhoneNo,string message)
        {
            try
            {
                if(!PhoneNo.StartsWith("+1"))
                {
                    PhoneNo = "+1" + PhoneNo;
                }
                using (var httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) })
                {
                    // Set up authorization header
                    var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ApiKey}:{ApiSecret}"));
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

                    // Define your request payload
                    var requestPayload = new
                    {
                        Channels = new int[] { 22 },
                        Targets = new string[] { $"{PhoneNo}" },
                        Message = $"{message}",
                        Execute = true
                    };

                    // Serialize the request payload to JSON
                    var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload);

                    // Create StringContent with JSON payload
                    using (var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
                    {
                        // Replace {AccountId} in the URL with the actual account ID
                        var requestUrl = $"account/{AccountID}/action-pushcampaign";

                        // Send the POST request
                        using (var response = await httpClient.PostAsync(requestUrl, content))
                        {
                            response.EnsureSuccessStatusCode();
                            if (response.IsSuccessStatusCode)
                            {
                                string responseData = await response.Content.ReadAsStringAsync();
                            }
                            else if(response.StatusCode== System.Net.HttpStatusCode.BadRequest)
                            {
                                return Json("1,");
                            }
                            return Json("true");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        [HttpPost]
        public async Task<bool> SendPushCampaignAsyncbool(string PhoneNo, string message)
        {
            try
            {
                if (!PhoneNo.StartsWith("+1"))
                {
                    PhoneNo = "+1" + PhoneNo;
                }
                using (var httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) })
                {
                    // Set up authorization header
                    var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ApiKey}:{ApiSecret}"));
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

                    // Define your request payload
                    var requestPayload = new
                    {
                        Channels = new int[] { 22 },
                        Targets = new string[] { $"{PhoneNo}" },
                        Message = $"{message}",
                        Execute = true
                    };

                    // Serialize the request payload to JSON
                    var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload);

                    // Create StringContent with JSON payload
                    using (var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
                    {
                        // Replace {AccountId} in the URL with the actual account ID
                        var requestUrl = $"account/{AccountID}/action-pushcampaign";

                        // Send the POST request
                        using (var response = await httpClient.PostAsync(requestUrl, content))
                        {
                            response.EnsureSuccessStatusCode();
                            if (response.IsSuccessStatusCode)
                            {
                                string responseData = await response.Content.ReadAsStringAsync();
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public async Task<string> FetchReceivedMessages()
        {
            string apiUrl = BaseUrl+ $@"account/{AccountID}/callback"; 
            string apiKey = ApiKey;
            using (var httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) })
            {
                // Set up authorization header
                var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ApiKey}:{ApiSecret}"));
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);



                // Create StringContent with JSON payload
                var requestUrl = apiUrl;// $"account/{AccountID}/action-pushcampaign";

                // Send the POST request
                using (var response = await httpClient.GetAsync(requestUrl))
                {
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        return responseData;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        return "";
                    }
                    return "";
                }
            }
        }
        [HttpPost]
     
        public ActionResult ReceiveIncomingMessage(TrueDialogIncomeMessage value)
        {
            //TrueDialogIncomeMessage value = new TrueDialogIncomeMessage();
            ////JArray userArray = (JArray)jObject["value"];
            //value = jObject["value"].ToObject<TrueDialogIncomeMessage>();
            string QueryInsert = $@"INSERT INTO TrueDialogIncomeMessage
           (Message
           ,PhoneNumber)
     VALUES
           ('{value.Message}'
           ,'{value.PhoneNumber}')";
            General.ExecuteNonQuery(QueryInsert);
            return Json("true",JsonRequestBehavior.AllowGet);
        }
    }
}