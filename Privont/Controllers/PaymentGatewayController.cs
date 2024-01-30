using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using Privont.Models;
using Privont.Models.CardAuthentication;
using TrueDialog;


namespace Privont.Controllers
{
    public class PaymentGatewayController : Controller
    {
        //PaymentGateway


        #region Easy Pay Direct Payment
        //var api = new EasyPayDirectAPI();
        //string validateResponse = api.ValidateCard("4111111111111111", "1225", "123");
        //string paymentResponse = api.ProcessPayment("4111111111111111", "1225", "123", "10.00");
        [HttpPost]
        public JsonResult GetCardValidation(CardInfo collection)
        {
            try
            {
                string WhereClause = "";
                JsonResult jr = new JsonResult();
                string Query = $@"";
                var api = new EasyPayPaymentIntegreation();
                var validateResponse = api.ValidateCard(collection.CardNumber, collection.ExpCardDate, collection.CVV.ToString());
                if (!validateResponse.IsSuccess)
                {
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Card are not valid", null);
                    return jr;
                }

                jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Card are Valid!", null);
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
        [HttpPost]
        public JsonResult PostCardPayment(CardInfo collection)
        {
            try
            {
                JsonResult jr = new JsonResult();
                string Query = $@"";
                var api = new EasyPayPaymentIntegreation();
                var validateResponse = api.ValidateCard(collection.CardNumber, collection.ExpiryDate.ToString("yyMM"), collection.CVV.ToString());
                if (!validateResponse.IsSuccess)
                {
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Card are not valid", null);
                    return jr;
                }
                var paymentResponse = api.ProcessPayment(collection.CardNumber, collection.ExpiryDate.ToString("yyMM"), collection.CVV.ToString(), collection.Amount.ToString());
                if (!paymentResponse.IsSuccess)
                {
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Transaction Failed", null);
                    return jr;
                }

                TransactionDetails transactionData = new TransactionDetails {
                    RefNo = "",
                    Amount = collection.Amount,
                    Status = 1,
                    Remarks = "",
                    UserID = collection.UserID,
                    UserType = collection.UserType,
                    TransactionActionID = collection.TransactionActionID,
                    APITransactionID = paymentResponse.TransactionId

                };

                transactionData.TransactionID = transactionData.InsertRecords();

                jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Payment Received!", null);
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
        #endregion

        #region Maverick Payment Gateway Integration

        MaverickPaymentService _maverickService;

        private string _oauthToken=$@"wlXyl.Au2Fc38GlRXBo9YcOR9WQC_77LfbrIuK";

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetAllTransactionHistoryAsync()
        {
            try
            {

                _maverickService = new MaverickPaymentService(_oauthToken);
                List<Transaction> transactions = await _maverickService.GetTransactions();
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Transaction Information");
                JSResponse.Add("Data", transactions);

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
            catch (Exception ex)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", ex.Message);
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
        //[Authorize(Roles = "Admin")]
        public async Task<JsonResult> PostTransaction(CardPayment collection)
        {
            try
            {

                _maverickService = new MaverickPaymentService(_oauthToken);
                List<CardPayment> transactions = await _maverickService.PostTransactions(collection);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Transaction Information");
                JSResponse.Add("Data", transactions);

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
            catch (Exception ex)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", ex.Message);
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
        //[Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetCardAuthentication(TransactionData collection)
        {
            try
            {

                _maverickService = new MaverickPaymentService(_oauthToken);
                bool response = await _maverickService.GetCardAuthentication(collection);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Card Authentication");
                JSResponse.Add("Data", response);

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
            catch (Exception ex)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message",  ex.Message);
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


        #endregion


        #region Squreup Payment Gatway Integration
        //      SquareClient squareClient = new SquareClient.Builder()
        //    .Environment(Square.Environment.Sandbox) // Use Environment.Production for live transactions
        //    .AccessToken(General.AccessTokenForSquareup)
        //    .Build();
        //      // GET: PaymentGateway
        //      public async Task<ActionResult> IndexAsync()
        //      {
        //          try
        //          {
        //              //Create Customer
        //              var Customerbody = new CreateCustomerRequest.Builder()
        //    .IdempotencyKey(GenerateNewGuid().ToString())
        //    .GivenName("Abdullah")
        //    .FamilyName("Bukhari")
        //    .CompanyName("Ali G Essential")
        //    .EmailAddress("info@aligessential.com")
        //    .PhoneNumber("+12533427110")
        //    .Build();

        //              var result = await squareClient.CustomersApi.CreateCustomerAsync(body: Customerbody);
        //              // Customer ID from Squareup API
        //              var customerID = result.Customer.Id;

        //              // Create Address Information
        //              var billingAddress = new Address.Builder()
        //                  .FirstName("Abdullah")
        //                  .LastName("Bukhari")
        //                  .Country("US")
        //.AddressLine1("500 Electric Ave")
        //.AddressLine2("Suite 600")
        //.Locality("New York")
        //.AdministrativeDistrictLevel1("NY")
        //.PostalCode("94103")
        //.Country("US")
        //.Build();

        //              // Create Card Information
        //              var card = new Card.Builder()
        //              .CardholderName("Abdullah Bukhari")
        //                .BillingAddress(billingAddress)
        //                .CustomerId(customerID)
        //                .ReferenceId("Card:" + GenerateNewGuid())
        //                .ExpMonth(4)
        //                .ExpYear(2023).CardBrand("VISA").CardType("")
        //                .Build();

        //              var CreateCardBody = new CreateCardRequest.Builder(
        //                  idempotencyKey: GenerateNewGuid().ToString(),
        //                  sourceId: "cnon:card-nonce-ok",
        //                  card: card)
        //                .Build();

        //              var NewCardResult = await squareClient.CardsApi.CreateCardAsync(body: CreateCardBody);

        //              var NewCardID = NewCardResult.Card.Id;


        //              // Create Amount Money 20$ and New Payment Request
        //              var amountMoney = new Money.Builder()
        //.Amount(20L)
        //.Currency("USD")
        //.Build();

        //              var body = new CreatePaymentRequest.Builder(sourceId: NewCardID, idempotencyKey: GenerateNewGuid().ToString())
        //                .AmountMoney(amountMoney)
        //                .CustomerId(customerID)
        //                .ReferenceId("Payment_Create_" + GenerateNewGuid())
        //                .Build();

        //              return Json("true" + body, JsonRequestBehavior.AllowGet);
        //          }
        //          catch (Exception ex)
        //          {
        //              return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //          }
        //      }

        //      public async Task CreateCardAsync()
        //      {
        //          //Create Customer
        //          var Customerbody = new CreateCustomerRequest.Builder()
        //.IdempotencyKey(GenerateNewGuid().ToString())
        //.GivenName("Abdullah")
        //.FamilyName("Bukhari")
        //.CompanyName("Ali G Essential")
        //.EmailAddress("info@aligessential.com")
        //.PhoneNumber("+12533427110")
        //.Build();

        //          var result = await squareClient.CustomersApi.CreateCustomerAsync(body: Customerbody);


        //          var billingAddress = new Address.Builder()
        // .AddressLine1("500 Electric Ave")
        // .AddressLine2("Suite 600")
        // .Locality("New York")
        // .AdministrativeDistrictLevel1("NY")
        // .PostalCode("94103")
        // .Country("US")
        // .Build();

        //          var card = new Card.Builder()
        //            .CardholderName("John Doe")
        //            .BillingAddress(billingAddress)
        //            .CustomerId("{CUSTOMER_ID}")
        //            .ReferenceId("alternate-id-1")
        //            .Build();

        //          var body = new CreateCardRequest.Builder(
        //              idempotencyKey: GenerateNewGuid().ToString(),
        //              sourceId: "cnon:card-nonce-ok",
        //              card: card)
        //            .Build();

        //          try
        //          {
        //              //var result = await squareClient.CustomersApi.CreateCustomerAsync(body);
        //          }
        //          catch (Exception e)
        //          {
        //              Console.WriteLine("Failed to make the request");
        //              Console.WriteLine($"Exception: {e.Message}");
        //          }
        //      }
        //      public Guid GenerateNewGuid() { return Guid.NewGuid(); }
        #endregion

        public RealEstateAgentInfo GetUserInfo(int UserID,int UserType)
        {
            string WhereClause = $@"where UserID='{UserID}' and UserType='{UserType}' ";
            string sql1 = $@"
select * from 

(
Select (RealEstateAgentId)UserID,UserName,Password,FirstName,LastName,StreetNo,StreetName,EmailAddress,Contact1,Website,Remarks,Inactive,2 as UserType 
from RealEstateAgentInfo 

union all
Select (LenderId)UserID,UserName,Password,FirstName,LastName,StreetNo,StreetName,EmailAddress,Contact1,Website,Remarks,Inactive,3 as UserType 
from LenderInfo  
union all

SELECT        LeadID,Username,Password, FirstName, LastName,'' as  StreetNo, '' as StreetName, EmailAddress, PhoneNo,'' as Website,'' as Remarks,0 as
Inactive, 4 AS UserType
FROM            LeadInfo

union all
SELECT        VendorID, Username, Password, FirstName, LastName, StreetNo, StreetName, EmailAddress, Contact1, Website, Remarks, Inactive
, 5 AS UserType
FROM            VendorInfo

)ProfileLogin

{WhereClause}

";
            DataTable dt = General.FetchData(sql1);
            if(dt.Rows.Count > 0)
            {
                RealEstateAgentInfo obj = General.ConvertDataTable<RealEstateAgentInfo>(dt)[0];
                return obj;
            }
            return new RealEstateAgentInfo();
        }
    }
}