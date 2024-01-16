using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Privont.Models;
using Privont.Models.CardAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrueDialog;

namespace Privont
{
    public class MaverickPaymentService
    {
        private readonly HttpClient _httpClient;
        public string url = "https://sandbox-gateway.dashboard.maverickpayments.com/";//"https://sandbox-dashboard.maverickpayments.com/";
        public string ApiBaseUrl { get { return "https://gateway.maverickpayments.com/"; } }//"https://gateway.maverickpayments.com/";

        public MaverickPaymentService(string apiKey)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            _httpClient = new HttpClient(clientHandler);
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(ApiBaseUrl + "payments");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    transactions = JsonConvert.DeserializeObject<List<Transaction>>(jsonResponse);
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Specific handling for HTTP-related exceptions
                throw new Exception("HTTP Request Error: " + httpEx.Message);
            }
            catch (TaskCanceledException cancelEx)
            {
                // Specific handling for canceled tasks
                throw new Exception("Task Canceled: " + cancelEx.Message);
            }
            catch (JsonException jsonEx)
            {
                // Handling JSON serialization/deserialization errors
                throw new Exception("JSON Error: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                // Catch-all for any other unexpected exceptions
                throw new Exception("Unexpected Error: " + ex.Message);
            }

            return transactions;
        }
        public async Task<List<CardPayment>> PostTransactions(CardPayment collection)
        {
            List<CardPayment> transactions = new List<CardPayment>();
            try
            {

                _httpClient.BaseAddress = new Uri(ApiBaseUrl);
                _httpClient.MaxResponseContentBufferSize = 256000;
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.Timeout = TimeSpan.FromMilliseconds(60000);

                //var request = new HttpRequestMessage(HttpMethod.Post, ApiBaseUrl + "payment/sale");
                var content = new StringContent(JsonConvert.SerializeObject(collection), Encoding.UTF8, "application/json");
                //request.Content = content;
                HttpResponseMessage response = _httpClient.PostAsync("payment/sale", content).Result;
                //var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    transactions = JsonConvert.DeserializeObject<List<CardPayment>>(jsonResponse);
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Specific handling for HTTP-related exceptions
                throw new Exception("HTTP Request Error: " + httpEx.Message);
            }
            catch (TaskCanceledException cancelEx)
            {
                // Specific handling for canceled tasks
                throw new Exception("Task Canceled: " + cancelEx.Message);
            }
            catch (JsonException jsonEx)
            {
                // Handling JSON serialization/deserialization errors
                throw new Exception("JSON Error: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                // Catch-all for any other unexpected exceptions
                throw new Exception("Unexpected Error: " + ex.Message);
            }
            return transactions;
        }
        public async Task<bool> GetCardAuthentication(TransactionData collection)
        {
            bool flag = false;
            try
            {
                List<CardPayment> CardPayment = new List<CardPayment>();
                _httpClient.BaseAddress = new Uri(ApiBaseUrl);
                _httpClient.MaxResponseContentBufferSize = 256000;
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.Timeout = TimeSpan.FromMilliseconds(60000);

                //var request = new HttpRequestMessage(HttpMethod.Post, ApiBaseUrl + "payment/sale");
                var content = new StringContent(JsonConvert.SerializeObject(collection), Encoding.UTF8, "application/json");
                //request.Content = content;
                HttpResponseMessage response = _httpClient.PostAsync("payment/sale", content).Result;
                //var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    CardPayment = JsonConvert.DeserializeObject<List<CardPayment>>(jsonResponse);
                    if (!string.IsNullOrEmpty(CardPayment[0]._CardValidate.sequence))
                    {
                        flag = true;
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Specific handling for HTTP-related exceptions
                throw new Exception("HTTP Request Error: " + httpEx.Message);
            }
            catch (TaskCanceledException cancelEx)
            {
                // Specific handling for canceled tasks
                throw new Exception("Task Canceled: " + cancelEx.Message);
            }
            catch (JsonException jsonEx)
            {
                // Handling JSON serialization/deserialization errors
                throw new Exception("JSON Error: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                // Catch-all for any other unexpected exceptions
                throw new Exception("Unexpected Error: " + ex.Message);
            }

            return flag;
        }
    }
}