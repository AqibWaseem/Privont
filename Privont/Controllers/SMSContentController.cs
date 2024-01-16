using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class SMSContentController : Controller
    {
        // GET: SMSContent

        private const string BaseUrl = "https://api.truedialog.com/api/v2.1/";
        private const string ApiKey = "42e7b9be42864ab9a417ac09310c61bb"; // Replace with your API key
        private const string ApiSecret = "Cs6!a5+XEr9?"; // Replace with your API secret   
        private const int AccountID = 22965;
        public ActionResult Index()
        {
            return View();
        }

        // GET: SMSContent/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SMSContent/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SMSContent/Create
        [HttpPost]
        public ActionResult Create(string PhoneNo, string Message)
        {
            try
            {
                var Response = SendPushCampaignAsyncbool(PhoneNo, Message);
                var flag = Response.Result;
                return Json(flag);
            }
            catch
            {
                return Json(false);
            }
        }

        // GET: SMSContent/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SMSContent/Edit/5
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

        // GET: SMSContent/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SMSContent/Delete/5
        [HttpPost]
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
    }
}
