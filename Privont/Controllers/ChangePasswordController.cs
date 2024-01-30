using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class ChangePasswordController : Controller
    {
        // GET: ChangePassword
        [HttpGet]
        public JsonResult RequestToChangePassword(int RequestBy,string Request,int UserID,int UserType)
        {
            try
            {
                // if RequestBy = 1 then Phone no else if RequestBy = 2 then email
                // Request = Email / Phone no

                DataTable dt = new DataTable();
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JsonResult jr = new JsonResult();
                if (UserID == 0 )
                {
                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.NotFound);
                    JSResponse.Add("Message", "User ID cannot be null or zero!");
                    JSResponse.Add("Data", DBNull.Value);

                    jr = new JsonResult()
                    {
                        Data = JSResponse,
                        ContentType = "application/json",
                        ContentEncoding = System.Text.Encoding.UTF8,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                    return jr;
                }
                string WhereClause = $@"where UserID='{UserID}' and UserType='{UserType}'  and (Contact1 = '{Request}' or EmailAddress='{Request}')";
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
                dt = General.FetchData(sql1);
                if(dt.Rows.Count==0)
                {

                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.NotFound);
                    JSResponse.Add("Message", (RequestBy == 2 ? "Email" : "Phone no") + " is not valid! Please try again later");
                    JSResponse.Add("Data", DBNull.Value);

                    jr = new JsonResult()
                    {
                        Data = JSResponse,
                        ContentType = "application/json",
                        ContentEncoding = System.Text.Encoding.UTF8,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                    return jr;
                }
                if(RequestBy == 1)
                {
                    // Sent Otp Code On Phone No to Verify

                }
                else if (RequestBy == 2)
                {
                    // Sent Otp Code On Email to Verify


                }

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Members Profile Information");
                JSResponse.Add("Data", dbrows);

                jr = new JsonResult()
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
                JSResponse.Add("Message", "Error" + ex.Message);
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

        [HttpGet]
        public JsonResult ChangePassword(string Password, int UserID, int UserType)
        {
            try
            {
                // if RequestBy = 1 then Phone no else if RequestBy = 2 then email
                // Request = Email / Phone no

                DataTable dt = new DataTable();
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JsonResult jr = new JsonResult();
                if (UserID == 0)
                {
                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.NotFound);
                    JSResponse.Add("Message", "User ID cannot be null or zero!");
                    JSResponse.Add("Data", DBNull.Value);

                    jr = new JsonResult()
                    {
                        Data = JSResponse,
                        ContentType = "application/json",
                        ContentEncoding = System.Text.Encoding.UTF8,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                    return jr;
                }
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
                dt = General.FetchData(sql1);
                if (dt.Rows.Count == 0)
                {

                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.NotFound);
                    JSResponse.Add("Message", "User is not valid! Please try again later");
                    JSResponse.Add("Data", DBNull.Value);

                    jr = new JsonResult()
                    {
                        Data = JSResponse,
                        ContentType = "application/json",
                        ContentEncoding = System.Text.Encoding.UTF8,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                    return jr;
                }
                if(UserType == 2)
                {
                    General.ExecuteNonQuery($@"update  RealEstateAgentInfo set Password='{Password}' where RealEstateAgentId={UserID}");
                }
                else if(UserType == 3)
                {
                    General.ExecuteNonQuery($@"update  LenderInfo set Password='{Password}' where LenderId={UserID}");
                }
                else if (UserType == 4)
                {
                    General.ExecuteNonQuery($@"update  LeadInfo set Password='{Password}' where LeadID={UserID}");
                }
                else if (UserType == 5)
                {
                    General.ExecuteNonQuery($@"update  VendorInfo set Password='{Password}' where VendorID={UserID}");
                }
                

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Change Password Successfully!");
                JSResponse.Add("Data", dbrows);

                jr = new JsonResult()
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
                JSResponse.Add("Message", "Error" + ex.Message);
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