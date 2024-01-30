using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class EasyPayDirectResponse
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string ErrorMessage { get; set; }

        // Constructor for successful response
        public EasyPayDirectResponse(string transactionId, string responseCode, string responseMessage)
        {
            IsSuccess = true;
            TransactionId = transactionId;
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
        }

        // Constructor for error response
        public EasyPayDirectResponse(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }

        // Method to parse API response (you might need to adjust the parsing logic based on the actual API response format)
        public static EasyPayDirectResponse ParseApiResponse(string apiResponse)
        {
            try
            {
                // Assuming the API returns a JSON response. Adjust the parsing logic as needed.
                var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(apiResponse);

                if (jsonObject.ContainsKey("error"))
                {
                    return new EasyPayDirectResponse(jsonObject["error"].ToString());
                }
                else
                {
                    return new EasyPayDirectResponse(
                        jsonObject["transactionId"].ToString(),
                        jsonObject["responseCode"].ToString(),
                        jsonObject["responseMessage"].ToString());
                }
            }
            catch (Exception ex)
            {
                return new EasyPayDirectResponse("Error parsing API response: " + ex.Message);
            }
        }
    }
}