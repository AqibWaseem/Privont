using Privont.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace Privont
{
    public class EasyPayPaymentIntegreation
    {
        private const string ApiUrl = "https://secure.easypaydirectgateway.com/api/transact.php";
        private const string security_key = "92q4QsQACuh3N4N355w5ScdeHx8nFveA";

        public EasyPayDirectResponse ValidateCard(string cardNumber, string cardExp, string cvv)
        {
            using (var webClient = new WebClient())
            {
                var postData = new NameValueCollection
                    {
                        { "type", "validate" },
                        { "security_key", security_key },
                        { "ccnumber", cardNumber },
                        { "ccexp", cardExp },
                        { "cvv", cvv }
                    };
                return SendRequest(postData);
            }
        }

        public EasyPayDirectResponse ProcessPayment(string cardNumber, string cardExp, string cvv, string amount)
        {
            using (var webClient = new WebClient())
            {
                var postData = new NameValueCollection
                {
                    { "type", "sale" },
                    { "security_key", security_key },
                    { "ccnumber", cardNumber },
                    { "ccexp", cardExp },
                    { "cvv", cvv },
                    { "amount", amount }
                };

                return SendRequest(postData);
            }
        }

        private EasyPayDirectResponse SendRequest(NameValueCollection postData)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var response = webClient.UploadValues(ApiUrl, "POST", postData);
                    var responseString = Encoding.UTF8.GetString(response);
                    return EasyPayDirectResponse.ParseApiResponse(responseString);
                }
            }
            catch (WebException ex)
            {
                return new EasyPayDirectResponse($"Network error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new EasyPayDirectResponse($"Error: {ex.Message}");
            }
        }

    }
}