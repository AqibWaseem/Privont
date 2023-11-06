using Privont.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using static Privont.General;
using Newtonsoft.Json.Linq;
using TrueDialog.Model;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Helpers;
using System.Web.DynamicData;

namespace Privont.Controllers
{
    public class GeneralApisController : Controller
    {
        // GET: GeneralApis
        LeadInfo LeadModel = new LeadInfo();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveRecord(int LinkType, string FirstName, string LastName, string PhoneNo, string EmailAddress)
        {
            try
            {
                string value = "";
                if (LinkType == 1)
                {
                    string sql = $@"Insert into RealEstateAgentInfo (FirstName,LastName,Contact1,EmailAddress,UserID,UserType) values ('{FirstName}','{LastName}','{PhoneNo}','{EmailAddress}',{General.UserID},{General.UserType})";
                    sql = sql + $@"SELECT SCOPE_IDENTITY() as RealEstateAgentID";
                    DataTable dt = General.FetchData(sql);
                    value = dt.Rows[0]["RealEstateAgentID"].ToString();
                }
                else
                {
                    string sql = $@"Insert into LenderInfo (FirstName,LastName,Contact1,EmailAddress,UserID,UserType) values ('{FirstName}','{LastName}','{PhoneNo}','{EmailAddress}',{General.UserID},{General.UserType})";
                    sql = sql + $@"SELECT SCOPE_IDENTITY() as LenderID";
                    DataTable dt = General.FetchData(sql);
                    value = dt.Rows[0]["LenderID"].ToString();
                }
                value = General.Encrypt(value, General.key);
                return Json("true," + value);
            }
            catch
            {
                return Json("false,");
            }
        }
        public ActionResult Encrypt(int obj)
        {
            string value = General.Encrypt(obj.ToString(), General.key);
            return Json("true," + value);
        }
        public string EncryptLead(int obj)
        {
            string value = General.Encrypt(obj.ToString(), General.key);
            return value;
        }
        public ActionResult Lead(string q, string d, string i, string y, string s)
        {
            int value = int.Parse(General.Decrypt(y, General.key));
            List<LeadInfo> lst = General.ConvertDataTable<LeadInfo>(LeadModel.GetAllRecord(" where LeadID=" + value));
            if (lst.Count <= 0)
            {
                return RedirectToAction("LinkExpire", "RealEstateAgentInfo");
            }
            return View(lst[0]);
        }
        [HttpPost]
        public ActionResult SaveRecordLead(int LeadID, int obj)
        {
            General.FetchData($@"Update LeadInfo Set isClaimLead={obj} Where LeadID = {LeadID}");
            return Json("true");
        }
        public ActionResult Thankyou()
        {
            return View();
        }
        public ActionResult Username(string Username)
        {
            DataTable dt = General.FetchData($@" select UserID,UserName,Inactive,1 as UserType from UserInfo Where userName= {Username} 
union
Select (RealEstateAgentId)UserID,UserName,Inactive,2 as UserType from RealEstateAgentInfo Where userName= {Username} union 
Select (LenderId)UserID,UserName,Inactive,3 as UserType from LenderInfo Where userName= {Username}   ");
            if (dt.Rows.Count > 0)
            {
                return Json("1,");
            }
            else
            {
                return Json("2,");
            }

        }
        public ActionResult VerifyEmail(string Email, string Firstname, string Lastname, int Type, int userID)
        {
            string sql = "";
            int UserID = 0;
            if (Type == 1)
            {
                if (userID == 0)
                {
                    sql = sql + $@"Insert into RealEstateAgentInfo (FirstName,LastName,EmailAddress) values ('{Firstname}','{Lastname}','{Email}')";
                    sql = sql + " SELECT SCOPE_IDENTITY() as Id";
                }
                else
                {
                    sql = sql + $@" Update RealEstateAgentInfo Set FirstName='{Firstname}',Lastname='{Lastname}',EmailAddress='{Email}' Where RealEstateAgentID={userID}";
                }
            }
            else
            {
                if (userID == 0)
                {
                    sql = sql + $@" Insert into LenderInfo (FirstName,LastName,EmailAddress) values ('{Firstname}','{Lastname}','{Email}')";
                    sql = sql + " SELECT SCOPE_IDENTITY() as Id";
                }
                else
                {
                    sql = sql + $@" Update LenderInfo Set Set FirstName='{Firstname}',Lastname='{Lastname}',EmailAddress='{Email}' Where LenderID={userID}";
                }
            }
            string ID = "0";
            if (userID == 0)
            {
                ID = (General.FetchData(sql).Rows[0]["Id"].ToString());
            }
            else
            {
                ID = userID.ToString();
            }
            UserID = int.Parse(ID);
            // Generate a verification token (e.g., a GUID)
            var verificationToken = Guid.NewGuid().ToString();

            // Store the token and user's email address in your database
            // Replace this with your database code
            string Ec = General.Encrypt(ID, General.key);
            // Send the verification email
            var verificationLink = Url.Action("Verify", "GeneralApis", new { token = verificationToken, value = Ec }, Request.Url.Scheme);
            var subject = "Verify Your Email Address";
            var body = BodyHtml(UserID, $@"{Firstname} {Lastname}", verificationToken, Ec, Type);
            //$"Please click the following link to verify your email address: <a href='{verificationLink}'>Verify Email</a>";

            using (var smtpClient = new SmtpClient())
            {
                var smtpSection = System.Configuration.ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                smtpClient.Host = smtpSection.Network.Host;
                smtpClient.Port = smtpSection.Network.Port;
                smtpClient.EnableSsl = smtpSection.Network.EnableSsl;
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
                smtpClient.Send(message);
                ViewBag.Message = "Registration successful. Please check your email to verify your account.";
                return Json("true," + userID);
            }
        }
        public string BodyHtml(int UserID, string Username, string token, string value, int Type)
        {
            var verificationLink = Url.Action("Verify", "GeneralApis", new { token = token, value = value, Type = Type }, Request.Url.Scheme);
            string MailBody = @"<!DOCTYPE HTML PUBLIC '-//W3C//DTD XHTML 1.0 Transitional //EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>
<head>
    <!--[if gte mso 9]>
    <xml>
      <o:OfficeDocumentSettings>
        <o:AllowPNG/>
        <o:PixelsPerInch>96</o:PixelsPerInch>
      </o:OfficeDocumentSettings>
    </xml>
    <![endif]-->
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <meta name='x-apple-disable-message-reformatting'>
    <!--[if !mso]><!-->
    <meta http-equiv='X-UA-Compatible' content='IE=edge'><!--<![endif]-->
    <title></title>

    <style type='text/css'>
      @media only screen and (min-width: 620px) {
  .u-row {
    width: 600px !important;
  }
  .u-row .u-col {
    vertical-align: top;
  }

  .u-row .u-col-50 {
    width: 300px !important;
  }

  .u-row .u-col-100 {
    width: 600px !important;
  }

}

@media (max-width: 620px) {
  .u-row-container {
    max-width: 100% !important;
    padding-left: 0px !important;
    padding-right: 0px !important;
  }
  .u-row .u-col {
    min-width: 320px !important;
    max-width: 100% !important;
    display: block !important;
  }
  .u-row {
    width: calc(100% - 40px) !important;
  }
  .u-col {
    width: 100% !important;
  }
  .u-col > div {
    margin: 0 auto;
  }
}
body {
  margin: 0;
  padding: 0;
}

table,
tr,
td {
  vertical-align: top;
  border-collapse: collapse;
}

p {
  margin: 0;
}

.ie-container table,
.mso-container table {
  table-layout: fixed;
}

* {
  line-height: inherit;
}

a[x-apple-data-detectors='true'] {
  color: inherit !important;
  text-decoration: none !important;
}

table, td { color: #000000; } a { color: #075e55; text-decoration: underline; }
    </style>



    <!--[if !mso]><!-->
    <link href='https://fonts.googleapis.com/css?family=Lato:400,700&display=swap' rel='stylesheet' type='text/css'><!--<![endif]-->

</head>

<body class='clean-body u_body' style='margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #f9f9f9;color: #000000'>
    <!--[if IE]><div class='ie-container'><![endif]-->
    <!--[if mso]><div class='mso-container'><![endif]-->
    <table style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #f9f9f9;width:100%' cellpadding='0' cellspacing='0'>
        <tbody>
            <tr style='vertical-align: top'>
                <td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>
                    <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td align='center' style='background-color: #f9f9f9;'><![endif]-->


                    <div class='u-row-container' style='padding: 0px;background-color: #f9f9f9'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #f9f9f9;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: #f9f9f9;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #f9f9f9;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:15px;font-family: Lato,sans-serif;' align='left'>

                                                            <table height='0px' align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 1px solid #f9f9f9;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>
                                                                <tbody>
                                                                    <tr style='vertical-align: top'>
                                                                        <td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>
                                                                            <span>&#160;</span>
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: transparent'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #ffffff;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:25px 10px;font-family: Lato,sans-serif;' align='left'>

                                                            <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                                                                <tr>
                                                                    <td style='padding-right: 0px;padding-left: 0px;' align='center'>

                                                                        <!-- <img align='center' border='0' src='https://privont.softinn.pk/images/Privont-Logo.png' alt='Image' title='Image' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 29%;max-width: 168.2px;' width='168.2'/> -->
                                                                        <h1 style='color:#075e55'>PRIVONT</h1>
                                                                    </td>
                                                                </tr>
                                                            </table>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: transparent'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #075e55;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #075e55;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:35px 10px 10px;font-family: Lato,sans-serif;' align='left'>

                                                            <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                                                                <tr>
                                                                    <td style='padding-right: 0px;padding-left: 0px;' align='center'>

                                                                        <img align='center' border='0' src='https://www.privont.com/images/Privont-Logo.png' alt='Image' title='Image' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 10%;max-width: 58px;' width='58' />

                                                                    </td>
                                                                </tr>
                                                            </table>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:0px 10px 30px;font-family:Lato,sans-serif;' align='left'>

                                                            <div style='line-height: 140%; text-align: left; word-wrap: break-word;'>
                                                                <p style='font-size: 14px; line-height: 140%; text-align: center;'><span style='font-size: 28px; line-height: 39.2px; color: #ffffff; font-family: Lato, sans-serif;'>Please Verify your Email </span></p>
                                                            </div>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: transparent'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #ffffff;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:40px 40px 30px;font-family: Lato,sans-serif;' align='left'>

                                                            <div style='line-height: 140%; text-align: left; word-wrap: break-word;'>
                                                                <p style='font-size: 14px; line-height: 140%;'><span style='font-size: 18px; line-height: 25.2px; color: #666666;'>Hello " + Username + @",</span></p>
                                                                <p style='font-size: 14px; line-height: 140%;'>&nbsp;</p>
                                                                <p style='font-size: 14px; line-height: 140%;'><span style='font-size: 18px; line-height: 25.2px; color: #666666;'>We have sent you this email in response to your request to create account of Privont as real estate.</span></p>
                                                                <p style='font-size: 14px; line-height: 140%;'>&nbsp;</p>
                                                                <p style='font-size: 14px; line-height: 140%;'><span style='font-size: 18px; line-height: 25.2px; color: #666666;'>To verify you email, please click the link below to verify your account: </span></p>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:0px 40px;font-family: Lato,sans-serif;' align='left'>

                                                            <div align='center'>
                                                                <!--[if mso]><table width='100%' cellpadding='0' cellspacing='0' border='0' style='border-spacing: 0; border-collapse: collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;font-family:'Lato',sans-serif;'><tr><td style='font-family:'Lato',sans-serif;' align='left'><v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' href='' style='height:51px; v-text-anchor:middle; width:205px;' arcsize='2%' stroke='f' fillcolor='#075e55'><w:anchorlock/><center style='color:#FFFFFF;font-family:'Lato',sans-serif;'><![endif]-->
                                                                <a href='" + verificationLink + @"' target='_blank' style='box-sizing: border-box;display: inline-block;font-family: Lato,sans-serif;text-decoration: none;-webkit-text-size-adjust: none;text-align: center;color: #FFFFFF; background-color: #075e55; border-radius: 1px;-webkit-border-radius: 1px; -moz-border-radius: 1px; width:auto; max-width:100%; overflow-wrap: break-word; word-break: break-word; word-wrap:break-word; mso-border-alt: none;'>
                                                                    <span style='display:block;padding:15px 40px;line-height:120%;'><span style='font-size: 18px; line-height: 21.6px;'>Verify Email</span></span>
                                                                </a>
                                                                <!--[if mso]></center></v:roundrect></td></tr></table><![endif]-->
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:40px 40px 30px;font-family: Lato,sans-serif;' align='left'>

                                                            <div style='line-height: 140%; text-align: left; word-wrap: break-word;'>
                                                                <p style='font-size: 14px; line-height: 140%;'><span style='color: #888888; font-size: 14px; line-height: 19.6px;'><em><span style='font-size: 16px; line-height: 22.4px;'>Please ignore this email if you did not request to verify Email.</span></em></span><br /><span style='color: #888888; font-size: 14px; line-height: 19.6px;'><em><span style='font-size: 16px; line-height: 22.4px;'>&nbsp;</span></em></span></p>
                                                            </div>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: transparent'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #075e55;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #075e55;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='300' style='width: 300px;padding: 20px 20px 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='300' style='width: 300px;padding: 0px 0px 0px 20px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-50' style='margin:auto;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px 0px 0px 45px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:25px 18px 10px;font-family: Lato,sans-serif;' align='left'>

                                                            <div align='center'>
                                                                <div style='display: table; max-width:187px;'>
                                                                    <!--[if (mso)|(IE)]><table width='187' cellpadding='0' cellspacing='0' border='0'><tr><td style='border-collapse:collapse;' align='left'><table width='100%' cellpadding='0' cellspacing='0' border='0' style='border-collapse:collapse; mso-table-lspace: 0pt;mso-table-rspace: 0pt; width:187px;'><tr><![endif]-->
                                                                    <!--[if (mso)|(IE)]><td width='32' style='width:32px; padding-right: 15px;' valign='top'><![endif]-->
                                                                    <table align='center' border='0' cellspacing='0' cellpadding='0' width='32' height='32' style='width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px'>
                                                                        <tbody>
                                                                            <tr style='vertical-align: top'>
                                                                                <td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>
                                                                                    <a href='https://web.facebook.com/Privont' title='Facebook' target='_blank'>
                                                                                        <img src='https://img.freepik.com/premium-vector/blue-social-media-logo_197792-1759.jpg' alt='Facebook' title='Facebook' width='32' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important'>
                                                                                    </a>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                                    <!--[if (mso)|(IE)]><td width='32' style='width:32px; padding-right: 15px;' valign='top'><![endif]-->
                                                                    <table align='center' border='0' cellspacing='0' cellpadding='0' width='32' height='32' style='width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px'>
                                                                        <tbody>
                                                                            <tr style='vertical-align: top'>
                                                                                <td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>
                                                                                    <a href='https://twitter.com/Privont' title='Twitter' target='_blank'>
                                                                                        <img src='https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQkUf043IwpPGEVHlGqChxbRS6uLlAo2n19CAuX3gdd&s' alt='Twitter' title='Twitter' width='32' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important'>
                                                                                    </a>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                                    <!--[if (mso)|(IE)]><td width='32' style='width:32px; padding-right: 15px;' valign='top'><![endif]-->
                                                                    <table align='center' border='0' cellspacing='0' cellpadding='0' width='32' height='32' style='width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px'>
                                                                        <tbody>
                                                                            <tr style='vertical-align: top'>
                                                                                <td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>
                                                                                    <a href='https://www.instagram.com/privont/' title='Instagram' target='_blank'>
                                                                                        <img src='https://upload.wikimedia.org/wikipedia/commons/thumb/a/a5/Instagram_icon.png/600px-Instagram_icon.png' alt='Instagram' title='Instagram' width='32' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important'>
                                                                                    </a>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                                    <!--[if (mso)|(IE)]><td width='32' style='width:32px; padding-right: 0px;' valign='top'><![endif]-->
                                                                    <!--<table align='left' border='0' cellspacing='0' cellpadding='0' width='32' height='32' style='width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 0px'>
                                                                        <tbody>
                                                                            <tr style='vertical-align: top'>
                                                                                <td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>
                                                                                    <a href=' ' title='LinkedIn' target='_blank'>
                                                                                        <img src='https://zp.hisabdaar.com/Newimages/image-4.png' alt='LinkedIn' title='LinkedIn' width='32' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important'>
                                                                                    </a>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>-->
                                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                                    <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                                                </div>
                                                            </div>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:5px 20px 10px;font-family: Lato,sans-serif;' align='center'>

                                                            <div style='line-height: 140%; text-align: left; word-wrap: break-word;'>
                                                                <p style='line-height: 140%; font-size: 14px;'><span style='font-size: 14px; line-height: 19.6px;'><span style='color: #ecf0f1; font-size: 14px; line-height: 19.6px;'><span style='line-height: 19.6px; font-size: 14px;'>Privont &copy;&nbsp; All Rights Reserved</span></span></span></p>
                                                            </div>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: #f9f9f9'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #075e55;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: #f9f9f9;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #075e55;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:0px;font-family: Lato,sans-serif;' align='left'>

                                                            <table height='0px' align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 1px solid #075e55;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>
                                                                <tbody>
                                                                    <tr style='vertical-align: top'>
                                                                        <td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>
                                                                            <span>&#160;</span>
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: transparent'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #f9f9f9;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #f9f9f9;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:0px 40px 30px 20px;font-family: Lato,sans-serif;' align='left'>

                                                            <div style='line-height: 140%; text-align: left; word-wrap: break-word;'>

                                                            </div>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>


                    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->
                </td>
            </tr>
        </tbody>
    </table>
    <!--[if mso]></div><![endif]-->
    <!--[if IE]></div><![endif]-->
</body>

</html>
";
            return MailBody;
        }


        public string LeadBodyHtml(string Name, string InvitedPersonName, string GeneratedLink, int UserType)
        {
            string UserTypeName = "";
            if (UserType == 2)
            {
                UserTypeName = "real estate";
            }
            if (UserType == 3)
            {
                UserTypeName = "Lender";
            }

            string MailBody = @"<!DOCTYPE HTML PUBLIC '-//W3C//DTD XHTML 1.0 Transitional //EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>
<head>
    <!--[if gte mso 9]>
    <xml>
      <o:OfficeDocumentSettings>
        <o:AllowPNG/>
        <o:PixelsPerInch>96</o:PixelsPerInch>
      </o:OfficeDocumentSettings>
    </xml>
    <![endif]-->
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <meta name='x-apple-disable-message-reformatting'>
    <!--[if !mso]><!-->
    <meta http-equiv='X-UA-Compatible' content='IE=edge'><!--<![endif]-->
    <title></title>

    <style type='text/css'>
      @media only screen and (min-width: 620px) {
  .u-row {
    width: 600px !important;
  }
  .u-row .u-col {
    vertical-align: top;
  }

  .u-row .u-col-50 {
    width: 300px !important;
  }

  .u-row .u-col-100 {
    width: 600px !important;
  }

}

@media (max-width: 620px) {
  .u-row-container {
    max-width: 100% !important;
    padding-left: 0px !important;
    padding-right: 0px !important;
  }
  .u-row .u-col {
    min-width: 320px !important;
    max-width: 100% !important;
    display: block !important;
  }
  .u-row {
    width: calc(100% - 40px) !important;
  }
  .u-col {
    width: 100% !important;
  }
  .u-col > div {
    margin: 0 auto;
  }
}
body {
  margin: 0;
  padding: 0;
}

table,
tr,
td {
  vertical-align: top;
  border-collapse: collapse;
}

p {
  margin: 0;
}

.ie-container table,
.mso-container table {
  table-layout: fixed;
}

* {
  line-height: inherit;
}

a[x-apple-data-detectors='true'] {
  color: inherit !important;
  text-decoration: none !important;
}

table, td { color: #000000; } a { color: #075e55; text-decoration: underline; }
    </style>



    <!--[if !mso]><!-->
    <link href='https://fonts.googleapis.com/css?family=Lato:400,700&display=swap' rel='stylesheet' type='text/css'><!--<![endif]-->

</head>

<body class='clean-body u_body' style='margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #f9f9f9;color: #000000'>
    <!--[if IE]><div class='ie-container'><![endif]-->
    <!--[if mso]><div class='mso-container'><![endif]-->
    <table style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #f9f9f9;width:100%' cellpadding='0' cellspacing='0'>
        <tbody>
            <tr style='vertical-align: top'>
                <td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>
                    <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td align='center' style='background-color: #f9f9f9;'><![endif]-->


                    <div class='u-row-container' style='padding: 0px;background-color: #f9f9f9'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #f9f9f9;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: #f9f9f9;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #f9f9f9;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:15px;font-family: Lato,sans-serif;' align='left'>

                                                            <table height='0px' align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 1px solid #f9f9f9;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>
                                                                <tbody>
                                                                    <tr style='vertical-align: top'>
                                                                        <td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>
                                                                            <span>&#160;</span>
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: transparent'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #ffffff;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:25px 10px;font-family: Lato,sans-serif;' align='left'>

                                                            <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                                                                <tr>
                                                                    <td style='padding-right: 0px;padding-left: 0px;' align='center'>

                                                                        <!-- <img align='center' border='0' src='https://privont.softinn.pk/images/Privont-Logo.png' alt='Image' title='Image' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 29%;max-width: 168.2px;' width='168.2'/> -->
                                                                        <h1 style='color:#075e55'>PRIVONT</h1>
                                                                    </td>
                                                                </tr>
                                                            </table>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: transparent'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #075e55;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #075e55;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:35px 10px 10px;font-family: Lato,sans-serif;' align='left'>

                                                            <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                                                                <tr>
                                                                    <td style='padding-right: 0px;padding-left: 0px;' align='center'>

                                                                        <img align='center' border='0' src='https://www.privont.com/images/Privont-Logo.png' alt='Image' title='Image' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 10%;max-width: 58px;' width='58' />

                                                                    </td>
                                                                </tr>
                                                            </table>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:0px 10px 30px;font-family:Lato,sans-serif;' align='left'>

                                                            <div style='line-height: 140%; text-align: left; word-wrap: break-word;'>
                                                                <p style='font-size: 14px; line-height: 140%; text-align: center;'><span style='font-size: 28px; line-height: 39.2px; color: #ffffff; font-family: Lato, sans-serif;'>Invitation to create account on Privont </span></p>
                                                            </div>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: transparent'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #ffffff;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:40px 40px 30px;font-family: Lato,sans-serif;' align='left'>

                                                            <div style='line-height: 140%; text-align: left; word-wrap: break-word;'>
                                                                <p style='font-size: 14px; line-height: 140%;'><span style='font-size: 18px; line-height: 25.2px; color: #666666;'>Hello " + Name + @",</span></p>
                                                                <p style='font-size: 14px; line-height: 140%;'>&nbsp;</p>
                                                                <p style='font-size: 14px; line-height: 140%;'><span style='font-size: 18px; line-height: 25.2px; color: #666666;'>We have sent you this email in response to invitation from " + InvitedPersonName + @" to create account of Privont as " + UserTypeName + @"</span></p>
                                                                <p style='font-size: 14px; line-height: 140%;'>&nbsp;</p>
                                                                <p style='font-size: 14px; line-height: 140%;'><span style='font-size: 18px; line-height: 25.2px; color: #666666;'>Click the link below and fill up your data. </span></p>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:0px 40px;font-family: Lato,sans-serif;' align='left'>

                                                            <div align='center'>
                                                                <!--[if mso]><table width='100%' cellpadding='0' cellspacing='0' border='0' style='border-spacing: 0; border-collapse: collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;font-family:'Lato',sans-serif;'><tr><td style='font-family:'Lato',sans-serif;' align='left'><v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' href='' style='height:51px; v-text-anchor:middle; width:205px;' arcsize='2%' stroke='f' fillcolor='#075e55'><w:anchorlock/><center style='color:#FFFFFF;font-family:'Lato',sans-serif;'><![endif]-->
                                                                <a href='" + GeneratedLink + @"' target='_blank' style='box-sizing: border-box;display: inline-block;font-family: Lato,sans-serif;text-decoration: none;-webkit-text-size-adjust: none;text-align: center;color: #FFFFFF; background-color: #075e55; border-radius: 1px;-webkit-border-radius: 1px; -moz-border-radius: 1px; width:auto; max-width:100%; overflow-wrap: break-word; word-break: break-word; word-wrap:break-word; mso-border-alt: none;'>
                                                                    <span style='display:block;padding:15px 40px;line-height:120%;'><span style='font-size: 18px; line-height: 21.6px;'>Click Me</span></span>
                                                                </a>
                                                                <!--[if mso]></center></v:roundrect></td></tr></table><![endif]-->
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:40px 40px 30px;font-family: Lato,sans-serif;' align='left'>

                                                            <div style='line-height: 140%; text-align: left; word-wrap: break-word;'>
                                                                <p style='font-size: 14px; line-height: 140%;'><span style='color: #888888; font-size: 14px; line-height: 19.6px;'><em><span style='font-size: 16px; line-height: 22.4px;'>Please ignore this email if you do not want to create account on Privont.</span></em></span><br /><span style='color: #888888; font-size: 14px; line-height: 19.6px;'><em><span style='font-size: 16px; line-height: 22.4px;'>&nbsp;</span></em></span></p>
                                                            </div>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: transparent'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #075e55;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #075e55;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='300' style='width: 300px;padding: 20px 20px 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='300' style='width: 300px;padding: 0px 0px 0px 20px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-50' style='margin:auto;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px 0px 0px 45px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:25px 18px 10px;font-family: Lato,sans-serif;' align='left'>

                                                            <div align='center'>
                                                                <div style='display: table; max-width:187px;'>
                                                                    <!--[if (mso)|(IE)]><table width='187' cellpadding='0' cellspacing='0' border='0'><tr><td style='border-collapse:collapse;' align='left'><table width='100%' cellpadding='0' cellspacing='0' border='0' style='border-collapse:collapse; mso-table-lspace: 0pt;mso-table-rspace: 0pt; width:187px;'><tr><![endif]-->
                                                                    <!--[if (mso)|(IE)]><td width='32' style='width:32px; padding-right: 15px;' valign='top'><![endif]-->
                                                                    <table align='center' border='0' cellspacing='0' cellpadding='0' width='32' height='32' style='width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px'>
                                                                        <tbody>
                                                                            <tr style='vertical-align: top'>
                                                                                <td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>
                                                                                    <a href='https://web.facebook.com/Privont' title='Facebook' target='_blank'>
                                                                                        <img src='https://img.freepik.com/premium-vector/blue-social-media-logo_197792-1759.jpg' alt='Facebook' title='Facebook' width='32' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important'>
                                                                                    </a>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                                    <!--[if (mso)|(IE)]><td width='32' style='width:32px; padding-right: 15px;' valign='top'><![endif]-->
                                                                    <table align='center' border='0' cellspacing='0' cellpadding='0' width='32' height='32' style='width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px'>
                                                                        <tbody>
                                                                            <tr style='vertical-align: top'>
                                                                                <td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>
                                                                                    <a href='https://twitter.com/Privont' title='Twitter' target='_blank'>
                                                                                        <img src='https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQkUf043IwpPGEVHlGqChxbRS6uLlAo2n19CAuX3gdd&s' alt='Twitter' title='Twitter' width='32' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important'>
                                                                                    </a>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                                    <!--[if (mso)|(IE)]><td width='32' style='width:32px; padding-right: 15px;' valign='top'><![endif]-->
                                                                    <table align='center' border='0' cellspacing='0' cellpadding='0' width='32' height='32' style='width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px'>
                                                                        <tbody>
                                                                            <tr style='vertical-align: top'>
                                                                                <td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>
                                                                                    <a href='https://www.instagram.com/privont/' title='Instagram' target='_blank'>
                                                                                        <img src='https://upload.wikimedia.org/wikipedia/commons/thumb/a/a5/Instagram_icon.png/600px-Instagram_icon.png' alt='Instagram' title='Instagram' width='32' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important'>
                                                                                    </a>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                                    <!--[if (mso)|(IE)]><td width='32' style='width:32px; padding-right: 0px;' valign='top'><![endif]-->
                                                                    <!--<table align='left' border='0' cellspacing='0' cellpadding='0' width='32' height='32' style='width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 0px'>
                                                                        <tbody>
                                                                            <tr style='vertical-align: top'>
                                                                                <td align='left' valign='middle' style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>
                                                                                    <a href=' ' title='LinkedIn' target='_blank'>
                                                                                        <img src='https://zp.hisabdaar.com/Newimages/image-4.png' alt='LinkedIn' title='LinkedIn' width='32' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important'>
                                                                                    </a>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>-->
                                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                                    <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                                                </div>
                                                            </div>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:5px 20px 10px;font-family: Lato,sans-serif;' align='center'>

                                                            <div style='line-height: 140%; text-align: left; word-wrap: break-word;'>
                                                                <p style='line-height: 140%; font-size: 14px;'><span style='font-size: 14px; line-height: 19.6px;'><span style='color: #ecf0f1; font-size: 14px; line-height: 19.6px;'><span style='line-height: 19.6px; font-size: 14px;'>Privont &copy;&nbsp; All Rights Reserved</span></span></span></p>
                                                            </div>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: #f9f9f9'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #075e55;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: #f9f9f9;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #075e55;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:0px;font-family: Lato,sans-serif;' align='left'>

                                                            <table height='0px' align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 1px solid #075e55;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>
                                                                <tbody>
                                                                    <tr style='vertical-align: top'>
                                                                        <td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>
                                                                            <span>&#160;</span>
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>



                    <div class='u-row-container' style='padding: 0px;background-color: transparent'>
                        <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #f9f9f9;'>
                            <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>
                                <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:600px;'><tr style='background-color: #f9f9f9;'><![endif]-->
                                <!--[if (mso)|(IE)]><td align='center' width='600' style='width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->
                                <div class='u-col u-col-100' style='max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;'>
                                    <div style='width: 100% !important;'>
                                        <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>
                                            <!--<![endif]-->

                                            <table style='font-family: Lato,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                                <tbody>
                                                    <tr>
                                                        <td style='overflow-wrap:break-word;word-break:break-word;padding:0px 40px 30px 20px;font-family: Lato,sans-serif;' align='left'>

                                                            <div style='line-height: 140%; text-align: left; word-wrap: break-word;'>

                                                            </div>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <!--[if (!mso)&(!IE)]><!-->
                                        </div><!--<![endif]-->
                                    </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                            </div>
                        </div>
                    </div>


                    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->
                </td>
            </tr>
        </tbody>
    </table>
    <!--[if mso]></div><![endif]-->
    <!--[if IE]></div><![endif]-->
</body>

</html>
";
            return MailBody;
        }
        public ActionResult Verify(string token, string value, int Type)
        {
            // Look up the user with the given token in your database
            // Update the user's account status to "verified"
            int ID = int.Parse(General.Decrypt(value, General.key));
            string sql = "";
            if (Type == 1)
            {
                DataTable dt = General.FetchData($@"Select * from RealEstateAgentInfo Where RealEstateAgentID={ID}");
                if (dt.Rows.Count < 0)
                {
                    return RedirectToAction("LinkExpire", "RealEstateAgentInfo");
                }
                sql = $@"Update RealEstateAgentInfo Set IsEmailVerified=1 Where RealEstateAgentID={ID}";
            }
            else if (Type == 2)
            {
                DataTable dt = General.FetchData($@"Select * from LenderInfo Where LenderID={ID}");
                if (dt.Rows.Count < 0)
                {
                    return RedirectToAction("LinkExpire", "RealEstateAgentInfo");
                }
                sql = $@"Update LenderInfo set IsEmailVerified=1 Where LenderID ={ID}";
            }
            else
            {
                return RedirectToAction("LinkExpire", "RealEstateAgentInfo");
            }
            General.ExecuteNonQuery(sql);
            return View("EmailThankyou");
        }
        public ActionResult EmailThankyou()
        {
            return View();
        }
        public static bool InsertAPILogs(LogTypes Log, LogSource Source, int SourceID, string LogDescription, string NextLink)
        {
            string Query = $@"INSERT INTO APIsLogs
           (LogSource
           ,LogTypeID
           ,Source
           ,SourceID
           ,LogDateTime
           ,Description
           ,NextLink)
     VALUES
           ('{Source}'
           ,'{Log}'
           ,'{Source.ToString()}'
           ,{SourceID}
           ,GETDATE()
           ,'{LogDescription}'
           ,'{NextLink}')";
            try
            {
                General.ExecuteNonQuery(Query);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string LastAddedLink_APILog(LogSource Source)
        {
            string Query = $@"select NextLink from APIsLogs  
where SourceID in (select LeadID from LeadInfo where ApiSource is not null)  and Source='{Source.ToString()}'
order by LogID desc
";
            DataTable dt = General.FetchData(Query);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["NextLink"].ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetSourceOfAPI(int SourceID)
        {
            string Query = $@"
select APIConfig from APIConfigInfo where TypeID={SourceID} and RealEstateID={General.UserID}
";
            DataTable dt = General.FetchData(Query);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["APIConfig"].ToString();
            }
            else
            {
                return "";
            }
        }
        public bool CheckIfLeadsAlreadyExist(int UserID, int APILeadID)
        {
            string QueryCheck = $@"select * from LeadInfo where ApiLeadID={APILeadID} and EntrySource=2 and UserID=" + UserID;
            DataTable dt = General.FetchData(QueryCheck);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        //GeneralAPIs/SMSAPI

        public ActionResult SMSAPI(string PhoneNo, string message)
        {
            //var result = trueDialogService.SendMessage("" + PhoneNo + "", "" + message + "");
            return Json("true");
        }

        [HttpGet]
        public JsonResult GetUserInfo(string UserName, string Password)
        {
            string WhereClause = $@"where UserName='{UserName}' and Password='{Password}'";
            string sql1 = $@"select UserID,UserName,Inactive,1 as UserType from UserInfo {WhereClause}  union Select (RealEstateAgentId)UserID,UserName,Inactive,2 as UserType from RealEstateAgentInfo {WhereClause} union
Select (LenderId)UserID,UserName,Inactive,3 as UserType from LenderInfo {WhereClause}   ";
            DataTable dtproductinfo = General.FetchData(sql1);
            List<Dictionary<string, object>> dbrows = GetProductRows(dtproductinfo);
            Dictionary<string, object> JSResponse = new Dictionary<string, object>();
            if (JSResponse == null)
            {
                JSResponse.Add("Status", false);
            }
            else
            {
                JSResponse.Add("Status", true);
            }
            JSResponse.Add("Message", "Data for Login User");
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
        [ValidateInput(false)]
        public List<Dictionary<string, object>> GetProductRows(DataTable dtData)
        {
            List<Dictionary<string, object>>
            lstRows = new List<Dictionary<string, object>>();
            Dictionary<string, object> dictRow = null;

            foreach (DataRow dr in dtData.Rows)
            {
                dictRow = new Dictionary<string, object>();
                foreach (DataColumn col in dtData.Columns)
                {
                    dictRow.Add(col.ColumnName, dr[col]);
                }
                lstRows.Add(dictRow);
            }
            return (lstRows);
        }
        LeadInfo LeadInfoModel = new LeadInfo();
        [HttpPost]
        public ActionResult LeadInfoCreate(LeadInfo collection)
        {
            try
            {
                if (collection.LeadID == 0)
                {
                    collection.LeadID = LeadInfoModel.InsertRecord(collection);
                }
                else
                {
                    collection.LeadID = LeadInfoModel.UpdateRecord(collection);
                }
                return Json("true," + collection.LeadID);
            }
            catch
            {
                return View("false," + collection);
            }
        }
        [HttpGet]
        public JsonResult GetLeadInfo(int UserID, int UserType)
        {
            DataTable dataTable = new DataTable();
            string sql = "";
            string whereclause = "";
            if (UserType == 2)
            {
                whereclause = whereclause + $@" and UserID={UserID}";
            }
            if (UserType == 3)
            {

                sql = $@" Declare @LenderID int set @LenderID={UserID}
Declare @EntryTime int
Select @EntryTime=ExpiryTime from LeadExpiryTime
Select  A.LeadID,A.ZipCode,
case
        when ReadytoOptin = 1 and ClaimingLender = @LenderID then A.FirstName  -- Show LeadID if ReadytoOptin is 1
        else '****'  -- Show **** if ReadytoOptin is not 1
    end as FirstName,
case
        when ReadytoOptin = 1 and ClaimingLender = @LenderID then A.LastName  -- Show LeadID if ReadytoOptin is 1
        else '****'  -- Show **** if ReadytoOptin is not 1
    end as LastName,
case
        when ReadytoOptin = 1 and ClaimingLender = @LenderID then A.PhoneNo  -- Show LeadID if ReadytoOptin is 1
        else '****'  -- Show **** if ReadytoOptin is not 1
    end as PhoneNo,
case
 when ReadytoOptin = 1 and ClaimingLender = @LenderID then A.EmailAddress  -- Show LeadID if ReadytoOptin is 1
        else '****'  -- Show **** if ReadytoOptin is not 1
    end as EmailAddress,
case When A.ClaimingLender is not null and ClaimingLender = @LenderID then 'Claimed'
else '0' end as Claimed
 from (select LeadInfo.LeadID,LeadInfo.FirstName,LeadInfo.LastName,
isnull(OptInSMSStatus,0)OptInSMSStatus,PhoneNo,LeadInfo.EmailAddress,EntryDateTime,
isNull(ReadytoOptin,0)ReadytoOptin,LeadInfo.UserID,EntrySource as UserType , ZipCode.ZipCode , LeadClaiminfo.ClaimingLender,
case when DATEDIFF(minute, EntryDateTime, GetDATE())<=@EntryTime then 1 else 0 end IsBelowTime 
from leadinfo inner join RealEstateAgentInfo on RealEstateAgentInfo.RealEstateAgentID = LeadInfo.UserID 
inner join ZipCode on RealEstateAgentInfo.ZipCodeID = ZipCode.ZipCodeID 
Left outer join LeadClaimInfo on LeadInfo.LeadID = LeadClaimInfo.LeadID 
 Where LeadInfo.EntrySource = 2  
and LeadInfo.isClaimLead = 1 and LeadInfo.OptInSMSStatus=1 
)A 
where 1=case when isbelowtime=1 and (select Count(*) from favouritelender 
where favouritelender.UserID=A.Userid and favouritelender.UserID=@lenderid)>1 then 1 When  isbelowtime=0 
then 1  else 0 end order by LeadID desc
";
//                sql = $@"Declare @LenderID int set @LenderID={UserID}
//Declare @EntryTime int
//Select @EntryTime=ExpiryTime from LeadExpiryTime
//Select * from (select LeadID,LeadInfo.FirstName,LeadInfo.LastName,isnull(OptInSMSStatus,0)OptInSMSStatus,PhoneNo,LeadInfo.EmailAddress,EntryDateTime,isNull(ReadytoOptin,0)ReadytoOptin,LeadInfo.UserID,EntrySource as UserType , ZipCode.ZipCode , case when DATEDIFF(minute, EntryDateTime, GetDATE())<=@EntryTime then 1 else 0 end IsBelowTime from leadinfo inner join RealEstateAgentInfo on RealEstateAgentInfo.RealEstateAgentID = LeadInfo.UserID inner join ZipCode on RealEstateAgentInfo.ZipCodeID = ZipCode.ZipCodeID Where LeadInfo.EntrySource = 2 and LeadInfo.isClaimLead = 1 )A where 1=case when isbelowtime=1 and (select Count(*) from favouritelender where favouritelender.UserID=A.Userid and favouritelender.UserID=@lenderid)>1 then 1 When  isbelowtime=0 then 1  else 0 end order by LeadID desc";
            }
            else
            {
                sql = $@"SELECT
    LI.LeadID,
    LI.FirstName,
    LI.LastName,
    ISNULL(LI.OptInSMSStatus, 0) AS OptInSMSStatus,
    LI.PhoneNo,
    LI.EmailAddress,
    LI.EntryDateTime,
    ISNULL(LI.ReadytoOptin, 0) AS ReadytoOptin,
    LI.UserID,
    LI.EntrySource AS UserType,
    LI.PricePointID,
    LPP.PricePoint AS PricePointName,
    LI.isClaimLead";
                sql = sql + $@" FROM
    LeadInfo LI
LEFT OUTER JOIN
    LeadPricePoint LPP ON LI.PricePointID = LPP.PricePointID ";
                sql = sql + $@" {whereclause} Order by LeadID desc";
            }
            dataTable = General.FetchData(sql); ;
            List<Dictionary<string, object>> dbrows = GetProductRows(dataTable);
            Dictionary<string, object> JSResponse = new Dictionary<string, object>();
            if (JSResponse == null)
            {
                JSResponse.Add("Status", false);
            }
            else
            {
                JSResponse.Add("Status", true);
            }
            JSResponse.Add("Message", "Data for Lead Info");
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
        [HttpGet]
        public ActionResult GetLeadDetails(int LeadID)
        {
            try
            {
                DataTable dtLeadInfo = General.FetchData($@"Select LeadInfo.*,ZipCode from LeadInfo 
inner join RealEstateAgentInfo on RealEstateAgentInfo.RealEstateAgentID = LeadInfo.UserID 
inner join ZipCode on RealEstateAgentInfo.ZipCodeID = ZipCode.ZipCodeID Where LeadID= {LeadID}");
                DataTable dtLeadDetail = General.FetchData($@"Select LenderInfo.FirstName,LenderInfo.LastName from LeadClaimInfo 
inner join LenderInfo on LeadClaimInfo.ClaimingLender = LenderInfo.LenderID
Where LeadClaimInfo.LeadID= {LeadID}");
                Dictionary<string, object> dbrows = GetTableFirstRow(dtLeadInfo);
                List<Dictionary<string, object>> details = GetTableRows(dtLeadDetail);
                dbrows.Add("Details", details);

                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                if (JSResponse == null)
                {
                    JSResponse.Add("Status", false);
                }
                else
                {
                    JSResponse.Add("Status", true);
                }
                JSResponse.Add("Message", "Data for Lead Info");
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
                //return new JsonResult()
                //{
                //    Data = new
                //    {
                //        dbrows,
                //    },
                //    ContentType = "application/json",
                //    ContentEncoding = System.Text.Encoding.UTF8,
                //    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                //    MaxJsonLength = Int32.MaxValue
                //};
                //return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        public Dictionary<string, object> GetTableFirstRow(DataTable dtData)
        {

            Dictionary<string, object> dictRow = null;
            for (int i = 0; i < 1; i++)
            {
                dictRow = new Dictionary<string, object>();
                foreach (DataColumn col in dtData.Columns)
                {
                    dictRow.Add(col.ColumnName, dtData.Rows[i][col]);
                }

            }
            return dictRow;
        }
        [ValidateInput(false)]
        public List<Dictionary<string, object>> GetTableRows(DataTable dtData)
        {
            List<Dictionary<string, object>>
            lstRows = new List<Dictionary<string, object>>();
            Dictionary<string, object> dictRow = null;
            foreach (DataRow dr in dtData.Rows)
            {
                dictRow = new Dictionary<string, object>();
                foreach (DataColumn col in dtData.Columns)
                {
                    dictRow.Add(col.ColumnName, dr[col]);
                }
                lstRows.Add(dictRow);
            }
            return lstRows;
        }

[HttpGet]
        public JsonResult GetApiLeadType()
        {
            DataTable dataTable = new DataTable();
            string sql = "";
            sql = sql + $@" Select ApiTypeID,ApiTypeTitle from APITypeInfo";
            dataTable = General.FetchData(sql); ;
            List<Dictionary<string, object>> dbrows = GetProductRows(dataTable);
            Dictionary<string, object> JSResponse = new Dictionary<string, object>();
            if (JSResponse == null)
            {
                JSResponse.Add("Status", false);
            }
            else
            {
                JSResponse.Add("Status", true);
            }
            JSResponse.Add("Message", "Data for Api Type(Follow up Boss or Zillow)");
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
        [HttpGet]
        public JsonResult GetDashboard()
        {
            DataTable dataTable = new DataTable();
            string sql = "";
            sql = sql + $@" select Count(LeadID)LeadID,'Leads' as Title from LeadInfo
union all Select Count(RealEstateAgentID)RealEstateAgentID,'Real Estate' as Title from RealEstateAgentInfo
union all
Select Count(VendorID)VendorID, 'Vendor' as Title from VendorInfo
union all
Select Count(LenderID)LenderID,'Lender' as Title from LenderInfo";
            dataTable = General.FetchData(sql); ;
            List<Dictionary<string, object>> dbrows = GetProductRows(dataTable);
            Dictionary<string, object> JSResponse = new Dictionary<string, object>();
            if (JSResponse == null)
            {
                JSResponse.Add("Status", false);
            }
            else
            {
                JSResponse.Add("Status", true);
            }
            JSResponse.Add("Message", "Dashboard Details");
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
        [HttpPost]
        public async Task<ActionResult> SendEmailAsync(string value, int UserType, string GeneratedLink)
        {
            int ID = int.Parse(General.Decrypt(value, General.key));
            string Name = "";
            string Email = "";
            string PhoneNo = "";
            DataTable dt = new DataTable();
            if (UserType == 2)
            {
                dt = General.FetchData($@"Select (FirstName+' '+LastName)Name,EmailAddress,Contact1 from RealEstateAgentInfo Where RealEstateAgentID = {ID}");
            }
            if (UserType == 3)
            {
                dt = General.FetchData($@"Select (FirstName+' '+LastName)Name,EmailAddress,Contact1 From lenderInfo Where LenderID = {ID}");
            }
            Name = dt.Rows[0]["Name"].ToString();
            Email = dt.Rows[0]["EmailAddress"].ToString();
            PhoneNo = dt.Rows[0]["Contact1"].ToString();
            if(!PhoneNo.StartsWith("+1"))
            {
                PhoneNo = "+1"+PhoneNo;
            }
            string InvitedPersonName = General.FetchData($@"Select (FirstName+' '+LastName)Name from RealEstateAgentInfo WHere RealEstateAgentID={General.UserID}").Rows[0]["Name"].ToString();
            var subject = "Create Account";
            var body = LeadBodyHtml($@"{Name}", InvitedPersonName, GeneratedLink, UserType);
            //$"Please click the following link to verify your email address: <a href='{verificationLink}'>Verify Email</a>";

            using (var smtpClient = new SmtpClient())
            {
                var smtpSection = System.Configuration.ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                smtpClient.Host = smtpSection.Network.Host;
                smtpClient.Port = smtpSection.Network.Port;
                smtpClient.EnableSsl = smtpSection.Network.EnableSsl;
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
                catch
                {
                    testingValue = 0;
                    return Json(testingValue + ",");
                }
                DataTable dtSMS = General.FetchData($@"Select isnulL(SMSDetailInvite,'')SMSDetailInvite from SMSSetting Where UserID={General.UserID}");
                if (dtSMS.Rows.Count <= 0)
                {
                    testingValue = 3;
                    return Json(testingValue + ",");
                }
                string SMSDetailInvite = dtSMS.Rows[0]["SMSDetailInvite"].ToString();
                if (SMSDetailInvite != "")
                {
                    PhoneNo = PhoneNo.Trim().Replace("-", "").Replace("_", "").Replace(",", "");
                    SMSDetailInvite = SMSDetailInvite.Replace("[Name]", Name);
                    SMSDetailInvite = SMSDetailInvite.Replace("[YourName]", InvitedPersonName);
                    SMSDetailInvite = SMSDetailInvite.Replace("[Link]", GeneratedLink);
                    var SMSTrueDialog = new SMSTrueDialogController();
                    try
                    {
                        if (await SMSTrueDialog.SendPushCampaignAsyncbool(PhoneNo, SMSDetailInvite))
                        {
                            return Json("true,");
                        }
                        else
                        {
                            return Json("false,");
                        }
                    }
                    catch
                    {
                        return Json("false,");
                    }
                }
                else
                {
                    testingValue = 3;
                    return Json(testingValue + ",");
                }
            }
        }
        [HttpGet]
        public async Task<JsonResult> SendSmsToLead(int LeadID,int UserID,int UserType)
        {
            string y = EncryptLead(LeadID);
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
            string encryptedData = Encrypt(jsonData, secretKey);
            string encryptedData2 = Encrypt(jsonData2, secretKey);
            // Construct the URL
            string domainUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string userType = UserType.ToString();
            string userId = UserID.ToString();
            string generatedLink = $"{domainUrl}/GeneralApis/Lead?q={encryptedData}&d={userType}&i={userId}&y={y}&s={encryptedData2}";
            string value = new SMSSettingController().SendSmsString(LeadID, generatedLink,UserID);
            string[] resultArray = value.Split(',');
            int value1 = int.Parse(resultArray[0]);
            if(value1==1)
            {
                return Json("false,SMS Setting is not defined please Correct this first", JsonRequestBehavior.AllowGet);
            }
            else
            {
                string PhoneNo = resultArray[2];
                string Message = resultArray[1];
                if (await new SMSTrueDialogController().SendPushCampaignAsyncbool(PhoneNo, Message))
                {
                    return Json("true",JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("false",JsonRequestBehavior.AllowGet);
                }
            }
        }
        private string Encrypt(string data, string secretKey)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(General.key);
                aesAlg.Mode = CipherMode.CFB;

                // Perform encryption
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                byte[] encryptedBytes;
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(data);
                        }
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }

                // Combine IV and encrypted data
                byte[] combinedBytes = new byte[aesAlg.IV.Length + encryptedBytes.Length];
                aesAlg.IV.CopyTo(combinedBytes, 0);
                encryptedBytes.CopyTo(combinedBytes, aesAlg.IV.Length);
                return Convert.ToBase64String(combinedBytes);
            }
        }
        [HttpGet]
        public ActionResult CreateRealorLenderSendEmail(string Email, string Firstname, string Lastname, int Type, int userID)
        {
            string sql = "";
            if (Type == 2)
            {
                if (userID == 0)
                {
                    sql = sql + $@"Insert into RealEstateAgentInfo (FirstName,LastName,EmailAddress) values ('{Firstname}','{Lastname}','{Email}')";
                    sql = sql + " SELECT SCOPE_IDENTITY() as Id";
                }
                else
                {
                    sql = sql + $@" Update RealEstateAgentInfo Set FirstName='{Firstname}',Lastname='{Lastname}',EmailAddress='{Email}' Where RealEstateAgentID={userID}";
                }
            }
            else
            {
                if (userID == 0)
                {
                    sql = sql + $@" Insert into LenderInfo (FirstName,LastName,EmailAddress) values ('{Firstname}','{Lastname}','{Email}')";
                    sql = sql + " SELECT SCOPE_IDENTITY() as Id";
                }
                else
                {
                    sql = sql + $@" Update LenderInfo Set Set FirstName='{Firstname}',Lastname='{Lastname}',EmailAddress='{Email}' Where LenderID={userID}";
                }
            }
            string ID = "0";
            if (userID == 0)
            {
                ID = (General.FetchData(sql).Rows[0]["Id"].ToString());
            }
            else
            {
                ID = userID.ToString();
            }
            UserID = int.Parse(ID);
            // Generate a verification token (e.g., a GUID)
            var verificationToken = Guid.NewGuid().ToString();

            // Store the token and user's email address in your database
            // Replace this with your database code
            string Ec = General.Encrypt(ID, General.key);
            // Send the verification email
            var verificationLink = Url.Action("Verify", "GeneralApis", new { token = verificationToken, value = Ec }, Request.Url.Scheme);
            var subject = "Verify Your Email Address";
            var body = BodyHtml(UserID, $@"{Firstname} {Lastname}", verificationToken, Ec, Type);
            //$"Please click the following link to verify your email address: <a href='{verificationLink}'>Verify Email</a>";

            using (var smtpClient = new SmtpClient())
            {
                var smtpSection = System.Configuration.ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                smtpClient.Host = smtpSection.Network.Host;
                smtpClient.Port = smtpSection.Network.Port;
                smtpClient.EnableSsl = smtpSection.Network.EnableSsl;
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
                smtpClient.Send(message);
                ViewBag.Message = "Registration successful. Please check your email to verify your account.";
                return Json("true," + userID,JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult InvitelenderorRealEstate(int Type,string FirstName,string LastName,string PhoneNo,string EmailAddress,int UserID,int UserType)
        {
            try
            {
                string value = "";
                int ID = 0;
                if (Type == 2)
                {
                    string sql = $@"Insert into RealEstateAgentInfo (FirstName,LastName,Contact1,EmailAddress,UserID,UserType) values ('{FirstName}','{LastName}','{PhoneNo}','{EmailAddress}',{UserID},{UserType})";
                    sql = sql + $@"SELECT SCOPE_IDENTITY() as RealEstateAgentID";
                    DataTable dt = General.FetchData(sql);
                    value = dt.Rows[0]["RealEstateAgentID"].ToString();
                    ID = int.Parse(dt.Rows[0]["RealEstateAgentID"].ToString());
                }
                else
                {
                    string sql = $@"Insert into LenderInfo (FirstName,LastName,Contact1,EmailAddress,UserID,UserType) values ('{FirstName}','{LastName}','{PhoneNo}','{EmailAddress}',{UserID},{UserType})";
                    sql = sql + $@"SELECT SCOPE_IDENTITY() as LenderID";
                    DataTable dt = General.FetchData(sql);
                    value = dt.Rows[0]["LenderID"].ToString();
                    ID = int.Parse(dt.Rows[0]["LenderID"].ToString());
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
                string encryptedData = Encrypt(jsonData, secretKey);
                string encryptedData2 = Encrypt(jsonData2, secretKey);
                // Construct the URL
                string domainUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string userType = UserType.ToString();
                string userId = UserID.ToString();
                string generatedLink = "";
                // Function to generate a token with an expiration time of 30 minutes (1800000 milliseconds)
                if (Type == 2)
                {
                    generatedLink = domainUrl + "/RealEstateAgentInfo/SignUp?q=" + encryptedData + "&d=" + UserType + "&i=" + UserID + "&y=" + value + "&s=" + encryptedData2;
                }
                else if (Type == 3)
                {
                    generatedLink = domainUrl + "/LenderInfo/SignUp?q=" + encryptedData + "&d=" + UserType + "&i=" + UserID + "&y=" + value + "&s=" + encryptedData2;
                }
                Task<string> returnvalue = SendEmailAsyncString(value, Type,generatedLink,UserID);
                string answer = returnvalue.Result;
                string[] resultArray = answer.Split(',');
                string value1 = resultArray[0];
                if(int.Parse(value1)==0)
                {
                    return Json($@"{value1},{"Email Sending Error"},{ID}",JsonRequestBehavior.AllowGet);
                }
                else if (int.Parse(value1) == 3)
                {
                    return Json($@"{value1},{"Email Successfully Send but SMS not send because you have not SMS setting"},{ID}", JsonRequestBehavior.AllowGet);
                }
                return Json($@"true,{ID}",JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("false,");
            }
        }
        [HttpPost]
        public async Task<string> SendEmailAsyncString(string value, int UserType, string GeneratedLink,int UserID)
        {
            int ID = int.Parse(General.Decrypt(value, General.key));
            string Name = "";
            string Email = "";
            string PhoneNo = "";
            DataTable dt = new DataTable();
            if (UserType == 2)
            {
                dt = General.FetchData($@"Select (FirstName+' '+LastName)Name,EmailAddress,Contact1 from RealEstateAgentInfo Where RealEstateAgentID = {ID}");
            }
            if (UserType == 3)
            {
                dt = General.FetchData($@"Select (FirstName+' '+LastName)Name,EmailAddress,Contact1 From lenderInfo Where LenderID = {ID}");
            }
            Name = dt.Rows[0]["Name"].ToString();
            Email = dt.Rows[0]["EmailAddress"].ToString();
            PhoneNo = dt.Rows[0]["Contact1"].ToString();
            if (!PhoneNo.StartsWith("+1"))
            {
                PhoneNo = "+1" + PhoneNo;
            }
            string InvitedPersonName = General.FetchData($@"Select (FirstName+' '+LastName)Name from RealEstateAgentInfo WHere RealEstateAgentID={UserID}").Rows[0]["Name"].ToString();
            var subject = "Create Account";
            var body = LeadBodyHtml($@"{Name}", InvitedPersonName, GeneratedLink, UserType);
            //$"Please click the following link to verify your email address: <a href='{verificationLink}'>Verify Email</a>";

            using (var smtpClient = new SmtpClient())
            {
                var smtpSection = System.Configuration.ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                smtpClient.Host = smtpSection.Network.Host;
                smtpClient.Port = smtpSection.Network.Port;
                smtpClient.EnableSsl = smtpSection.Network.EnableSsl;
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
                catch
                {
                    testingValue = 0;
                    return $@"{testingValue},";
                }
                DataTable dtSMS = General.FetchData($@"Select isnulL(SMSDetailInvite,'')SMSDetailInvite from SMSSetting Where UserID={UserID}");
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
                    var SMSTrueDialog = new SMSTrueDialogController();
                    try
                    {
                        if (await SMSTrueDialog.SendPushCampaignAsyncbool(PhoneNo, SMSDetailInvite))
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
        }
    }
}