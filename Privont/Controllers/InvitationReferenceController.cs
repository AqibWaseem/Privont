using Antlr.Runtime;
using Newtonsoft.Json.Linq;
using Privont.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services.Description;
using System.Xml.Linq;
using TrueDialog.Model;
using static Google.Apis.Requests.BatchRequest;
using static Privont.General;

namespace Privont.Controllers
{
    public class InvitationReferenceController : Controller
    {
        // GET: InvitationReference
        RealEstateAgentInfo RealEstateAgentModel =new RealEstateAgentInfo();
        LenderInfo LenderModel = new LenderInfo();
        LeadInfo LeadModel = new LeadInfo();
        VendorInfo VendorModel = new VendorInfo();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Invite(int TypeUser, RealEstateAgentInfo collection)
        {
            try
            {
                List<Dictionary<string, object>> dbrows = new List<Dictionary<string, object>>();
                int UserType = TypeUser;
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                var jr = new JsonResult();
                string Query = "";
                DataTable dt = new DataTable();
                var ID = 0;
                DataTable dtcheckIfUserExistance = dtCheckUserExistance(collection.Contact1, collection.UserID, collection.UserType, TypeUser);
                if (dtcheckIfUserExistance.Rows.Count == 0)
                {
                    //Unique Identifier
                    collection.UniqueIdentifier = GeneralApisController.GenerateUniqueIdentifier(UserType);
                    if (UserType == 2)//Real Estate Agent 
                    {
                        ID = RealEstateAgentModel.InsertRecordsByInviation(collection);
                    }
                    else if (UserType == 3)//Lender
                    {
                        LenderInfo LenderCollection = new LenderInfo();
                        General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, LenderInfo>(collection, LenderCollection);
                        ID = LenderModel.InsertRecordsByInviationLender(LenderCollection);
                    }
                    else if (UserType == 4)//Lead
                    {
                        LeadInfo Target = new LeadInfo();
                        General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, LeadInfo>(collection, Target);
                        ID = LeadModel.InsertRecordsByInviation(Target);
                        //SMS Request To Join Privont
                        var Response = SendSubMessageToLead(ID.ToString(), UserType, collection.UserID, collection.UserType);
                        if (Response == "3")
                        {
                            JSResponse = new Dictionary<string, object>();
                            JSResponse.Add("Status", 0003);
                            JSResponse.Add("Message", "SMS not send because you have not SMS setting!");
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
                        else if (Response == "4")
                        {
                            JSResponse = new Dictionary<string, object>();
                            JSResponse.Add("Status", 0004);
                            JSResponse.Add("Message", "Failed to sent sms!");
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
                        UserInfo newobjUser = new UserInfo();
                        DataTable newDt = newobjUser.GetAllRecordsViaUserIDandUserType(ID, UserType);
                        dbrows = new General().GetAllRowsInDictionary(newDt);
                        JSResponse = new Dictionary<string, object>();
                        JSResponse.Add("Status", HttpStatusCode.OK);
                        JSResponse.Add("Message", "Invitation Send Successfully!");
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
                    else if (UserType == 5)//Vendor
                    {
                        VendorInfo Target = new VendorInfo();
                        General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, VendorInfo>(collection, Target);
                        ID = VendorModel.InsertRecordsByInviationVendor(Target);
                    }
                }
                else if (dtcheckIfUserExistance.Rows.Count > 0)
                {
                    int.TryParse(dtcheckIfUserExistance.Rows[0]["LoginID"].ToString(), out ID);
                    collection.RealEstateAgentId = ID;
                    collection.LenderId = ID;
                    collection.LeadID = ID;
                    collection.VendorID = ID;

                    if (UserType == 2)//Real Estate Agent 
                    {
                        ID = RealEstateAgentModel.UpdateProfileRecord(collection);
                    }
                    else if (UserType == 3)//Lender
                    {
                        LenderInfo LenderCollection = new LenderInfo();
                        General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, LenderInfo>(collection, LenderCollection);
                        ID = LenderModel.UpdateProfileRecord(LenderCollection);
                    }
                    else if (UserType == 4)//Lead
                    {
                        LeadInfo Target = new LeadInfo();
                        General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, LeadInfo>(collection, Target);
                        ID = LeadModel.UpdateProfileRecord(Target);
                        //SMS Request To Join Privont
                        var Response = SendSubMessageToLead(ID.ToString(), UserType, collection.UserID, collection.UserType);
                        if (Response == "3")
                        {
                            JSResponse = new Dictionary<string, object>();
                            JSResponse.Add("Status", 0003);
                            JSResponse.Add("Message", "SMS not send because you have not SMS setting!");
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
                        else if (Response == "4")
                        {
                            JSResponse = new Dictionary<string, object>();
                            JSResponse.Add("Status", 0004);
                            JSResponse.Add("Message", "Failed to sent sms!");
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
                        UserInfo newobjUser = new UserInfo();
                        DataTable newDt = newobjUser.GetAllRecordsViaUserIDandUserType(ID, UserType);
                        dbrows = new General().GetAllRowsInDictionary(newDt);
                        JSResponse = new Dictionary<string, object>();
                        JSResponse.Add("Status", HttpStatusCode.OK);
                        JSResponse.Add("Message", "Invitation Send Successfully!");
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
                    else if (UserType == 5)//Vendor
                    {
                        VendorInfo Target = new VendorInfo();
                        General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, VendorInfo>(collection, Target);
                        ID = VendorModel.UpdateProfileRecord(Target);
                    }
                }
                //else if (UserType == 4 && dtcheckIfUserExistance.Rows.Count > 0)
                //{
                //    //SMS Request To Join Privont
                //    var Response = SendSubMessageToLead(ID.ToString(), UserType, collection.UserID, collection.UserType);
                //    if (Response == "3")
                //    {
                //        JSResponse = new Dictionary<string, object>();
                //        JSResponse.Add("Status", 0003);
                //        JSResponse.Add("Message", "SMS not send because you have not SMS setting!");
                //        JSResponse.Add("Data", DBNull.Value);

                //        jr = new JsonResult()
                //        {
                //            Data = JSResponse,
                //            ContentType = "application/json",
                //            ContentEncoding = System.Text.Encoding.UTF8,
                //            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                //            MaxJsonLength = Int32.MaxValue
                //        };
                //        return jr;
                //    }
                //    else if (Response == "4")
                //    {
                //        JSResponse = new Dictionary<string, object>();
                //        JSResponse.Add("Status", 0004);
                //        JSResponse.Add("Message", "Failed to sent sms!");
                //        JSResponse.Add("Data", DBNull.Value);

                //        jr = new JsonResult()
                //        {
                //            Data = JSResponse,
                //            ContentType = "application/json",
                //            ContentEncoding = System.Text.Encoding.UTF8,
                //            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                //            MaxJsonLength = Int32.MaxValue
                //        };
                //        return jr;
                //    }
                //    UserInfo newobjUser = new UserInfo();
                //    DataTable newDt = newobjUser.GetAllRecordsViaUserIDandUserType(ID, UserType);
                //    dbrows = new General().GetAllRowsInDictionary(newDt);
                //    JSResponse = new Dictionary<string, object>();
                //    JSResponse.Add("Status", HttpStatusCode.OK);
                //    JSResponse.Add("Message", "Invitation Send Successfully!");
                //    JSResponse.Add("Data", dbrows);

                //    jr = new JsonResult()
                //    {
                //        Data = JSResponse,
                //        ContentType = "application/json",
                //        ContentEncoding = System.Text.Encoding.UTF8,
                //        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                //        MaxJsonLength = Int32.MaxValue
                //    };
                //    return jr;
                //}
                string value1 = "0";

                if (ID == 0 && dtcheckIfUserExistance.Rows.Count > 0)
                {
                    int.TryParse(dtcheckIfUserExistance.Rows[0]["LoginID"].ToString(), out ID);
                }
                int EntryUserID = collection.UserID;
                int EntryUserType = collection.UserType;
                value1 = SendSMSandEmail(ID, UserType, EntryUserID, EntryUserType).ToString();

                UserInfo objUser = new UserInfo();
                DataTable dtUserDetauls = objUser.GetAllRecordsViaUserIDandUserType(ID, UserType);
                dbrows = new General().GetAllRowsInDictionary(dtUserDetauls);


                if ((value1) == 0.ToString())
                {
                    //return Json($@"{value1},{"Email Sending Error"},{ID}", JsonRequestBehavior.AllowGet);
                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", 0001);
                    JSResponse.Add("Message", "Email Sending Error!");
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
                else if (value1 == 3.ToString())
                {
                    //return Json($@"{value1},{"Email Successfully Send but SMS not send because you have not SMS setting"},{ID}", JsonRequestBehavior.AllowGet);
                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", 0002);
                    JSResponse.Add("Message", "Email Successfully Send but SMS not send because you have not SMS setting!");
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
                else if (value1 == "false")
                {
                    //return Json($@"{value1},{"Email Successfully Send but SMS not send because you have not SMS setting"},{ID}", JsonRequestBehavior.AllowGet);
                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", 0002);
                    JSResponse.Add("Message", "Email Successfully Send but SMS not send!");
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


                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Invitation Send Successfully!");
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
            catch(Exception ex)
            {
                var JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Error:"+ex.Message);
                JSResponse.Add("Data", DBNull.Value);

                var jr = new JsonResult()
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
        public ActionResult SMSSubscribeMessageToLead()
        {
            return Json("");
        }
        public int SendSubscribeMessage()
        {
            int Response = 0;

            return Response;
        }
        #region  Send SMS and Email with Encrypt Link
        //Encrypt 
        public string SendSMSandEmail(int ID, int IDUserType, int UserID,int UserType)
        {
            var value = Encryption.StringCipher.Encrypt(ID.ToString(), General.key);
            
            string generatedLink = "";
            generatedLink = new GeneralApisController().GenerateLink(ID, IDUserType, UserID, UserType, LinkTypes.Refer);
            Task<string> returnvalue = SendEmailAsyncString(value, IDUserType, generatedLink, UserID, UserType);
            string answer = returnvalue.Result;
            string[] resultArray = answer.Split(',');
            string value1 = resultArray[0];

            return (value1);
        }
        [HttpGet]
        public JsonResult SendSMSClaimedMsg(int LeadID, int UserID, int UserType)
        {
            //Generate Link for Claimed Lead
            string generatedLink = "";
            generatedLink = new GeneralApisController().GenerateLink(LeadID, 4, UserID, UserType, LinkTypes.Refer);

            //Checking SMS Setting Exist or not
            DataTable dt = new SMSSetting().GetSMSDetailsGetSMSDetails(UserID, UserType);// General.FetchData($@"Select SMSDetail from SMSSetting Where UserID={UserID}");
            if (dt.Rows.Count <= 0)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", 0002);
                JSResponse.Add("Message", "SMS not send because you have not SMS setting!");
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
            else if (dt.Rows[0]["SMSDetail"] == DBNull.Value )
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", 0002);
                JSResponse.Add("Message", "SMS not send because you have not SMS setting!");
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
            string SMSDetail = dt.Rows[0]["SMSDetail"].ToString();


            //Get Lead Information
            DataTable LeadDetail = General.FetchData($@"Select * from LeadInfo Where LeadID ={LeadID}");

            //Assigning to Model
            LeadInfo obj = new LeadInfo();
            obj.PhoneNo = LeadDetail.Rows[0]["PhoneNo"]
                .ToString().Trim().Replace("-", "").Replace("_", "").Replace(",", "");
            obj.FirstName = LeadDetail.Rows[0]["FirstName"].ToString();
            obj.LastName = LeadDetail.Rows[0]["LastName"].ToString();

            SMSDetail = SMSDetail.Replace("[Name]", obj.FirstName + " " + obj.LastName);
            SMSDetail = SMSDetail.Replace("[SMSClaimedKey]", generatedLink);

            string PhoneNumber = obj.PhoneNo;
            string Message = SMSDetail;
            var res = SendSMS(PhoneNumber, Message);
            string answer = res.Result;

            if (answer == "true")
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Claim request sent successfully!");
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
            else
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                var jr = new JsonResult();
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Unabled to send SMS! Please try again later");
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
        }
        [HttpPost]
        public async Task<string> SendEmailAsyncString(string value, int UserType, string GeneratedLink, int EntryUserID, int EntryUserType = 0)
        {
            int ID = int.Parse(Encryption.StringCipher.Decrypt(value, General.key));
            string Name = "";
            string Email = "";
            string PhoneNo = "";
            DataTable dt = new DataTable();
            //Get User Details which referred
            dt = new GeneralApisController().GetUserDetails(ID, UserType);

            Name = dt.Rows[0]["Name"].ToString();
            Email = dt.Rows[0]["EmailAddress"].ToString();
            PhoneNo = dt.Rows[0]["Contact1"].ToString();
            if (!PhoneNo.StartsWith("+1"))
            {
                PhoneNo = "+1" + PhoneNo;
            }
            //Sending Email
            var ResponseEmail = SendEmail("Create Account", Name, Email, GeneratedLink, UserType, EntryUserID, EntryUserType);
            if(ResponseEmail == 0)
            {
                //return $@"{ResponseEmail},";
            }
            //Send SMS
            Task<string> returnvalue = SendSMSAsyncString(value, UserType, GeneratedLink, EntryUserID, EntryUserType);
            string answer = returnvalue.Result;
            string[] resultArray = answer.Split(',');
            string value1 = resultArray[0];
            return $@"{value1},";
        }

        [HttpPost]
        public async Task<string> SendSMSAsyncString(string value, int UserType, string GeneratedLink, int EntryUserID, int EntryUserType = 0)
        {
            int ID = int.Parse(Encryption.StringCipher.Decrypt(value, General.key));
            string Name = "";
            string Email = "";
            string PhoneNo = "";

            DataTable dt = new DataTable();
            //Get User Details which referred
            dt = new GeneralApisController().GetUserDetails(ID, UserType);

            Name = dt.Rows[0]["Name"].ToString();
            Email = dt.Rows[0]["EmailAddress"].ToString();
            PhoneNo = dt.Rows[0]["Contact1"].ToString();
            if (!PhoneNo.StartsWith("+1"))
            {
                PhoneNo = "+1" + PhoneNo;
            }
            string InvitedPersonName = new GeneralApisController().GetUserDetails(EntryUserID, EntryUserType).Rows[0]["Name"].ToString();
            int testingValue = 0;
            DataTable dtSMS = General.FetchData($@"Select isnulL(SMSDetailInvite,'')SMSDetailInvite from SMSSetting Where UserID={EntryUserID}  and UserType={EntryUserType}");
            if (dtSMS.Rows.Count <= 0)
            {
                testingValue = 3;
                return $@"{testingValue},";
            }
            string SMSDetailInvite = dtSMS.Rows[0]["SMSDetailInvite"].ToString();
            if (SMSDetailInvite != "")
            {
                PhoneNo = PhoneNo.Trim().Replace("-", "").Replace("_", "").Replace(",", "");
                SMSDetailInvite = SMSDetailInvite.Replace("[Name]", Name);
                SMSDetailInvite = SMSDetailInvite.Replace("[YourName]", InvitedPersonName);
                SMSDetailInvite = SMSDetailInvite.Replace("[Link]", GeneratedLink);
                try
                {
                    var res = SendSMS(PhoneNo, SMSDetailInvite);
                    string answer = res.Result;
                    if(answer == "true")
                    {
                        return $@"true,";
                    }
                    else
                    {
                        return $@"false,";
                    }

                }
                catch
                {
                    return $@"false,";
                }
            }
            else
            {
                testingValue = 3;
                return $@"{testingValue},";
            }

        }
        public  string SendSubMessageToLead(string value, int UserType, int EntryUserID, int EntryUserType = 0)
        {
            int ID = int.Parse(value);
            string Name = "";
            string Email = "";
            string PhoneNo = "";

            DataTable dt = new DataTable();
            //Get User Details which referred
            dt = new GeneralApisController().GetUserDetails(ID, UserType);

            Name = dt.Rows[0]["Name"].ToString();
            Email = dt.Rows[0]["EmailAddress"].ToString();
            PhoneNo = dt.Rows[0]["Contact1"].ToString();
            if (!PhoneNo.StartsWith("+1"))
            {
                PhoneNo = "+1" + PhoneNo;
            }
            string InvitedPersonName = new GeneralApisController().GetUserDetails(EntryUserID, EntryUserType).Rows[0]["Name"].ToString();
            int testingValue = 0;
            DataTable dtSMS = General.FetchData($@"Select isnulL(SMSDetailSub,'')SMSDetailSub,SMSSubKey from SMSSetting Where UserID={EntryUserID} and UserType={EntryUserType}");
            if (dtSMS.Rows.Count <= 0)
            {
                testingValue = 3;
                return $@"{testingValue}";
            }
            string SMSDetailInvite = dtSMS.Rows[0]["SMSDetailSub"].ToString();
            string SMSDetailSubKey = dtSMS.Rows[0]["SMSSubKey"].ToString();
            if (!string.IsNullOrEmpty(SMSDetailInvite) && !string.IsNullOrEmpty(SMSDetailSubKey))
            {
                PhoneNo = PhoneNo.Trim().Replace("-", "").Replace("_", "").Replace(",", "");
                SMSDetailInvite = SMSDetailInvite.Replace("[Name]", Name);
                SMSDetailInvite = SMSDetailInvite.Replace("[YourName]", InvitedPersonName);
                SMSDetailInvite = SMSDetailInvite.Replace("[SubKey]", SMSDetailSubKey);
                try
                {
                    var res = SendSMS(PhoneNo, SMSDetailInvite);
                    string answer = res.Result;
                    if (answer == "true")
                    {
                        return $@"true";
                    }
                    else
                    {
                        return $@"false";
                    }

                }
                catch
                {
                    return $@"false";
                }
            }
            else
            {
                testingValue = 3;
                return $@"{testingValue}";
            }
        }


        public async Task<string> SendSMS(string PhoneNo,string Message)
        {
            var SMSTrueDialog = new SMSTrueDialogController();
            try
            {
                if (SMSTrueDialog.SendPushCampaignAsyncbool(PhoneNo, Message))
                {
                    return $@"true";
                }
                else
                {
                    return $@"false";
                }
            }
            catch
            {
                return $@"false";
            }
        }
        //Sending Email
        [HttpPost]
        public int SendEmail(string Subject, string Name, string Email, string GeneratedLink, int UserType,int EntryUserID,int EntryUserType )
        {
           
            string InvitedPersonName = new GeneralApisController().GetUserDetails(EntryUserID, EntryUserType).Rows[0]["Name"].ToString();

            //Email
            var subject = Subject;// "Create Account";
            var body = new GeneralApisController().LeadBodyHtml($@"{Name}", InvitedPersonName, GeneratedLink, UserType);
            using (var smtpClient = new SmtpClient())
            {
                var smtpSection = System.Configuration.ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                smtpClient.Host = "privont.com";//smtpSection.Network.Host;
                smtpClient.Port = 465;//smtpSection.Network.Port;
                smtpClient.EnableSsl = false;// smtpSection.Network.EnableSsl;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                var fromAddress = new MailAddress(smtpSection.From, "Privont");
                var toAddress = new MailAddress(Email);
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                int testingValue = 0;
                try
                {
                    smtpClient.Send(message);
                    testingValue = 1;
                }
                catch(Exception ex)
                {
                    testingValue = 0;
                }
                return testingValue;
            }
        }
        #endregion

        public ActionResult ReferInvite(int Type, string FirstName, string LastName, string PhoneNo, string EmailAddress, int UserID, int UserType)
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
                    Username = "Abdullah"
                };

                var dataToEncrypt2 = new
                {
                    userId = UserType,
                    Username = "Abdullah"
                };

                // Serialize the data to JSON
                var serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(dataToEncrypt);
                string jsonData2 = serializer.Serialize(dataToEncrypt2);
                // Encrypt the data
                string encryptedData = General.Encrypt(jsonData, secretKey);
                string encryptedData2 = General.Encrypt(jsonData2, secretKey);
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

        [HttpGet]
        public JsonResult GetPriceRange()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = General.FetchData($@"select * from LeadPricePoint");
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Price Range Information");
                JSResponse.Add("Data", dbrows); 

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
            catch
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Price Range Information");
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
        public JsonResult GetReferType()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = General.FetchData($@" select * from ReferTypeInfo where Inactive = 0 ");
                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Refer Type Information");
                JSResponse.Add("Data", dbrows); 

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
            catch(Exception ex)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
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
        [HttpGet]
        public JsonResult GetReferredMembersDetails(int UserID,int UserType,int TypeOfUser)
        {
            try
            {
                string Query = "";
                string Message = "";
                DataTable dt=new DataTable();
                if(TypeOfUser == 2)
                {
                    Message = "Real Estate Information";

                    Query = $@"   Where RealEstateAgentInfo.UserID={UserID} and RealEstateAgentInfo.UserType={UserType}";
                    dt = GeneralApisController.dtFetchAllRealEstateAgentInfo(Query);
                }
                else if (TypeOfUser == 3)
                {
                    Message = "Lender Information";
                    Query = $@"   Where LenderInfo.UserID={UserID} and LenderInfo.UserType={UserType}";
                    dt = GeneralApisController.dtFetchAllLenderInfo(Query);

                }
                else if (TypeOfUser == 4)
                {
                    Message = "Lead Information";
                    Query = $@"

 where LeadInfo.UserID={UserID} and LeadInfo.UserType={UserType}
";
                    dt = GeneralApisController.dtFetchAllLeadInfo(Query);
                }
                else if (TypeOfUser == 5)
                {
                    Message = "Vendor Information";
                    string columnTitle = $@"
(select count(*) from FavouriteVendorInfo where FavouriteVendorInfo.FavouriteVendorID=VendorInfo.VendorID and FavouriteVendorInfo.UserID={UserID} 
                    and FavouriteVendorInfo.UserType={UserType} )";
                    Query = $@"Select VendorInfo.*,ZipCode.ZipCode,OrganizationInfo.OrganizationTitle,{columnTitle} as Favourite
,ISNULL(AverageRating.AverageRating,0)AverageRating,ISNULL(AverageRating.TotalFeedBack,0)TotalFeedBack 

from VendorInfo
left outer join ZipCode on VendorInfo.ZipCodeID = ZipCode.ZipCodeID
left outer join OrganizationInfo on VendorInfo.OrganizationID = OrganizationInfo.OrganizationID
left outer join 
(
SELECT UserID, AVG(Rating) AS AverageRating,count(*) as TotalFeedBack
FROM FeedBackInfo where UserType=5
GROUP BY UserID
)AverageRating on VendorInfo.VendorID=AverageRating.UserID 

where VendorInfo.UserID={UserID} and UserType={UserType}

";
                    dt = General.FetchData(Query);
                }

                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt);
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Referred "+Message);
                JSResponse.Add("Data", dbrows);

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
            catch(Exception ex)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
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
        ///InvitationReference/Refer?q=" + encryptedData + "&d=" + UserType + "&ti=" + value + "&fiy=" + EntryUserID + "&s=" + encryptedData2 + "&tuti=" + tuti + "&v=" + dataToEncrypt3 + "&futy=" + EntryUserType
        [HttpGet]
        public JsonResult Refer(string q,string d,string ti,string fiy, string s,string tuti,string v,string futy)
        {
            //ti = To Invited
            //tuti To Invited User Type
            //fiy = From Invited
            //futy = From Invited User Type
            try
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                Dictionary<string, object> JSChildResponse = new Dictionary<string, object>();
                JsonResult jr = new JsonResult();
                if (!Request.Headers["Accept"].Contains("application/json"))
                {
                    jr = new JsonResult()
                    {
                        Data = "Something went wrong please try again later!",
                        ContentType = "application/json",
                        ContentEncoding = System.Text.Encoding.UTF8,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                    return jr;
                }
                

                ti = Encryption.StringCipher.Decrypt(ti, General.key);// General.Decrypt(ti, General.key); //User ID
                tuti = Encryption.StringCipher.Decrypt(tuti, General.key);//General.Decrypt(tuti, General.key); //User Type

                string EntryUserID = fiy;
                string EntryUserType = futy;

                DataTable dtcheck = UserProfileController.GetUserInformation(int.Parse(ti), int.Parse(tuti));
                if (dtcheck.Rows.Count <= 0)
                {
                    JSResponse.Add("Status", HttpStatusCode.BadRequest);
                    JSResponse.Add("Message", "Link Expire");
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
                if (dtcheck.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtcheck.Rows[0]["Username"].ToString()))
                    {
                        JSResponse.Add("Status", HttpStatusCode.Found);
                        JSResponse.Add("Message", "User already exist. Please Sign In!");
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
                }
                //Invitation from Person Information
                DataTable dtInvitedPersonInformation = UserProfileController.GetUserInformation(int.Parse(EntryUserID), int.Parse(EntryUserType));
                List<Dictionary<string, object>> dbInvitedTo = new General().GetAllRowsInDictionary(dtcheck);
                List<Dictionary<string, object>> dbInvitedFrom = new General().GetAllRowsInDictionary(dtInvitedPersonInformation);

                //Child json Response
                JSChildResponse.Add("InvitedFrom", dbInvitedFrom);
                JSChildResponse.Add("dbInvitedTo", dbInvitedTo);
                //Parent Json Format
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "SignUp User Information");
                JSResponse.Add("Data", JSChildResponse);

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
            catch(Exception ex)
            {
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();

                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Error: " + ex.Message);
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
        public JsonResult PostSignUp(string UserName, string Password, int UserID,int UserType)
        {
            try
            {

                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JsonResult jr = new JsonResult();
                DataTable dtcheck = UserProfileController.GetUserInformation(UserID, UserType);
                if (dtcheck.Rows.Count <= 0)
                {
                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.BadRequest);
                    JSResponse.Add("Message", "Link Expire");
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
                DataTable dtCheckUsernameExistance = new RealEstateAgentInfo().UserExistanceInfo(UserName);
                if (dtCheckUsernameExistance.Rows.Count > 0)
                {
                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.Conflict);
                    JSResponse.Add("Message", "Username already Exist");
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
                try
                {
                    if(UserType == 2)
                    {
                        General.ExecuteNonQuery($@" update  RealEstateAgentInfo set Username='{UserName}', Password='{Password}',IsApproved=1 where RealEstateAgentId={UserID} ");
                    }
                    else if (UserType == 3)
                    {
                        General.ExecuteNonQuery($@" update  LenderInfo set Username='{UserName}', Password='{Password}',IsApproved=1 where LenderId={UserID} ");
                    }
                    else if (UserType == 4)
                    {
                        General.ExecuteNonQuery($@" update  LeadInfo set Username='{UserName}', Password='{Password}',IsApproved=1 where LeadID={UserID} ");
                    }
                    else if (UserType == 5)
                    {
                        General.ExecuteNonQuery($@" update  VendorInfo set Username='{UserName}', Password='{Password}',IsApproved=1 where VendorID={UserID} ");
                    }
                }
                catch
                {
                    throw new Exception("Unabled to set Username and Password");
                }


                DataTable dtData = UserProfileController.GetUserInformation(UserID, UserType);
                List<Dictionary<string, object>> dbInvitedTo = new General().GetAllRowsInDictionary(dtData);

                //Parent Json Format
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Sign Up User Information");
                JSResponse.Add("Data", dbInvitedTo);

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
                JSResponse.Add("Message", "Error: " + ex.Message);
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
        public DataTable dtCheckUserExistance(string PhoneNo,int UserID,int UserType,int TypeOfUser)
        {
            string sql1 = $@"

select * from 
(select RealEstateAgentId as LoginID,UserID,UserType,Contact1,2 TypeOfUser from RealEstateAgentInfo

union all
select LenderId as LoginID,UserID,UserType,Contact1,3 TypeOfUser  from LenderInfo

union all

select LeadID as LoginID,UserID,UserType,Contact1,4 TypeOfUser  from LeadInfo

union all

select VendorID as LoginID,UserID,UserType,Contact1,5 TypeOfUser  from VendorInfo
)UserInfo where Contact1='{PhoneNo}' and UserID={UserID} and UserType={UserType} and TypeOfUser={TypeOfUser}

";
            DataTable dt = General.FetchData(sql1);
            return dt;
        }
    }
}