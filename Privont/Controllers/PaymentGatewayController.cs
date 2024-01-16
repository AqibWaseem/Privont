using System;
using System.Collections.Generic;
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


        #region Payment

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
    }
}