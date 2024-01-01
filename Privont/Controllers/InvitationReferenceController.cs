using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Privont.Controllers
{
    public class InvitationReferenceController : Controller
    {
        // GET: InvitationReference
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Invite(int Type, string FirstName, string LastName, string PhoneNo, string EmailAddress, int UserID, int UserType)
        {
            try
            {
                string InsertQuery = "";
                int ID = 0;
                string value = "";
                if (Type == 2) // Type = 2 : Real Estate Agent
                {
                    InsertQuery = "";
                    InsertQuery = $@"Insert into RealEstateAgentInfo (FirstName,LastName,Contact1,EmailAddress,UserID,UserType) values ('{FirstName}','{LastName}','{PhoneNo}','{EmailAddress}',{UserID},{UserType})";
                    InsertQuery = InsertQuery + $@"SELECT SCOPE_IDENTITY() as RealEstateAgentID";
                    DataTable dt = General.FetchData(InsertQuery);
                    value = dt.Rows[0]["RealEstateAgentID"].ToString();
                    ID = int.Parse(dt.Rows[0]["RealEstateAgentID"].ToString());
                }
                else if (Type == 3) // Type = 3 : Lender Information
                {
                    InsertQuery = "";
                    InsertQuery = $@"Insert into LenderInfo (FirstName,LastName,Contact1,EmailAddress,UserID,UserType) values ('{FirstName}','{LastName}','{PhoneNo}','{EmailAddress}',{UserID},{UserType})";
                    InsertQuery = InsertQuery + $@"SELECT SCOPE_IDENTITY() as LenderID";
                    DataTable dt = General.FetchData(InsertQuery);
                    value = dt.Rows[0]["LenderID"].ToString();
                    ID = int.Parse(dt.Rows[0]["LenderID"].ToString());
                }
                else if (Type == 4) // Type = 4 : Lead Information
                {

                }
                else if (Type == 5) // Type = 5 : Vendor Information
                {

                }
                value = General.Encrypt(value, General.key);




                string secretKey = "Privont@Privont";
                // Construct the data to be encrypted
                var dataToEncrypt = new
                {
                    userId = UserID,
                    username = "Abdullah"
                };

                var dataToEncrypt2 = new
                {
                    userId = UserType,
                    username = "Abdullah"
                };

                // Serialize the data to JSON
                var serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(dataToEncrypt);
                string jsonData2 = serializer.Serialize(dataToEncrypt2);
                // Encrypt the data
                string encryptedData = GeneralApisController.Encrypt(jsonData, secretKey);
                string encryptedData2 = GeneralApisController.Encrypt(jsonData2, secretKey);
                // Construct the URL
                string domainUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string userType = UserType.ToString();
                string userId = UserID.ToString();
                string generatedLink = "";

                generatedLink = domainUrl + "/InvitationReference/Refer?q=" + encryptedData + "&d=" + UserType + "&i=" + UserID + "&y=" + value + "&s=" + encryptedData2 + "&uT=" + Type;

                Task<string> returnvalue = new GeneralApisController().SendEmailAsyncString(value, Type, generatedLink, UserID);

                string answer = returnvalue.Result;
                string[] resultArray = answer.Split(',');
                string value1 = resultArray[0];
                if (int.Parse(value1) == 0)
                {
                    return Json($@"{value1},{"Email Sending Error"},{ID}", JsonRequestBehavior.AllowGet);
                }
                else if (int.Parse(value1) == 3)
                {
                    return Json($@"{value1},{"Email Successfully Send but SMS not send because you have not SMS setting"},{ID}", JsonRequestBehavior.AllowGet);
                }
                return Json($@"true,{ID}", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", false);
                JSResponse.Add("Message", "Error: "+ex.Message);
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
    }
}