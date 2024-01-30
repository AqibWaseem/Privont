using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Privont.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using TrueDialog.Model;

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
                Console.WriteLine($"Unabled to send sms. Please try again later");
                throw;
            }
        }

        [HttpPost]
        public bool SendPushCampaignAsyncbool(string PhoneNo, string message)
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
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

                    // Define your request payload
                    var requestPayload = new
                    {
                        Channels = new int[] { 22 },
                        Targets = new string[] { PhoneNo },
                        Message = message,
                        Execute = true
                    };

                    // Serialize the request payload to JSON
                    var jsonPayload = JsonConvert.SerializeObject(requestPayload);

                    // Create StringContent with JSON payload
                    using (var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
                    {
                        // Replace {AccountId} in the URL with the actual account ID
                        var requestUrl = $"account/{AccountID}/action-pushcampaign";

                        // Set a timeout for the HTTP request (e.g., 30 seconds)
                        httpClient.Timeout = TimeSpan.FromSeconds(30);

                        // Send the POST request synchronously
                        using (var response = httpClient.PostAsync(requestUrl, content).Result)
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                // Read and log the response data
                                string responseData = response.Content.ReadAsStringAsync().Result;
                                Console.WriteLine($"Response: {responseData}");
                                return true;
                            }
                            else if (response.StatusCode == HttpStatusCode.BadRequest)
                            {
                                // Log the error and return false
                                string errorData = response.Content.ReadAsStringAsync().Result;
                                Console.WriteLine($"Bad Request Error: {errorData}");
                                return false;
                            }
                            else
                            {
                                // Log other errors and return false
                                Console.WriteLine($"Error: {response.StatusCode}");
                                return false;
                            }
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
            int EntryUserID = 0;
            int EntryUserType = 0;

            #region Subscription message received
            General.GetUserIDandUserTypeFromSubKey(value.Message, ref EntryUserID, ref EntryUserType);
            if (EntryUserID > 0 && EntryUserType > 0)
            {
                LeadInfo leadInfo = new LeadInfo();
                leadInfo = General.ConvertDataTable<LeadInfo>(leadInfo.GetAllRecordsVIAPhoneNo(value.PhoneNumber, EntryUserID, EntryUserType))[0];
                General.ExecuteNonQuery($@"update LeadInfo set IsPrivontFamily=1 where LeadID=" + leadInfo.LeadID);
                var res = new InvitationReferenceController().SendSMSandEmail(leadInfo.LeadID, 4, EntryUserID, EntryUserType);
                if(res == "true")
                {
                    General.ExecuteNonQuery($@"Update LeadInfo set SMSSent=1 Where LeadID={leadInfo.LeadID}");
                }
               
            }
            #endregion

            #region Claimed message received
            General.GetUserIDandUserTypeFromClaimedKey(value.Message, ref EntryUserID, ref EntryUserType);
            if (EntryUserID > 0 && EntryUserType > 0)
            {
                LeadInfo leadInfo = new LeadInfo();
                leadInfo = General.ConvertDataTable<LeadInfo>(leadInfo.GetAllRecordsVIAPhoneNo(value.PhoneNumber, EntryUserID, EntryUserType))[0];

                General.FetchData($@"Update LeadInfo Set isClaimLead=1,OptInSMSStatus=1  Where LeadID = {leadInfo.LeadID}");

            }
            #endregion






            string QueryInsert = $@"INSERT INTO TrueDialogIncomeMessage
           (CallbackTimestamp,
            Message
           ,PhoneNumber)
     VALUES
           ('{value.CallbackTimestamp}'
           ,'{value.Message}'
           ,'{value.PhoneNumber}')";
            General.ExecuteNonQuery(QueryInsert);
            return Json("true",JsonRequestBehavior.AllowGet);
        }
    }
}