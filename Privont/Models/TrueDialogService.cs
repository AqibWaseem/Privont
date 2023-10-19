using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Privont.Models
{
    public class TrueDialogService
    {
        private readonly string apiKey = "2a3026c04aef4826ab6bd59af941d29f";
        private readonly string apiSecret = "2a3026c04aef4826ab6bd59af941d29f";
        private readonly string apiUrl = "https://api.truedialog.com/api/v1/";

        public TrueDialogService(string apiKey, string apiSecret)
        {
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiSecret}"));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {authHeader}");
            return client;
        }

        public async Task<string> SendMessage(string phoneNumber, string message)
        {
            using (var client = GetHttpClient())
            {
                var data = new
                {
                    PhoneNumber = phoneNumber,
                    Message = message
                };

                var response = await client.PostAsJsonAsync($"{apiUrl}some-endpoint", data);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Handle error
                    return null;
                }
            }
        }
    }
}