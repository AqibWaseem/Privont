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
            return Json("true,"+value);
        }
        public ActionResult Lead(string q,string d,string i,string y, string s)
        {
            int value = int.Parse(General.Decrypt(y, General.key));
            List<LeadInfo> lst = General.ConvertDataTable<LeadInfo>(LeadModel.GetAllRecord(" where LeadID=" + value));
            if(lst.Count<=0)
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
            if(dt.Rows.Count>0)
            {
                return Json("1,");
            }else
            {
                return Json("2,");
            }

        }
        public ActionResult VerifyEmail(string Email,string Firstname, string Lastname,int Type,int userID)
        {
            string sql = "";
            int UserID = 0;
            if(Type==1)
            {
                if(userID==0)
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
                if(userID==0)
                {
                    sql = sql + $@" Insert into LenderInfo (FirstName,LastName,EmailAddress) values ('{Firstname}','{Lastname}','{Email}')";
                    sql = sql + " SELECT SCOPE_IDENTITY() as Id";
                }
                else
                {
                    sql = sql +$@" Update LenderInfo Set Set FirstName='{Firstname}',Lastname='{Lastname}',EmailAddress='{Email}' Where LenderID={userID}";
                }
            }
            string ID = "0";
            if(userID==0)
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
            string Ec = General.Encrypt(ID,"@AFG");
            // Send the verification email
            var verificationLink = Url.Action("Verify", "GeneralApis", new { token = verificationToken,value= Ec}, Request.Url.Scheme);
            var subject = "Verify Your Email Address";
            var body = BodyHtml(UserID, $@"{Firstname} {Lastname}",verificationToken,Ec,Type); 
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
                return Json("true,"+userID);
            }
        }
        public string BodyHtml(int UserID, string Username,string token,string value,int Type)
        {
            var verificationLink = Url.Action("Verify", "GeneralApis", new { token = token, value = value,Type=Type }, Request.Url.Scheme);
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

                                                                        <img align='center' border='0' src='https://privont.softinn.pk/images/Privont-Logo.png' alt='Image' title='Image' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 10%;max-width: 58px;' width='58' />

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
                                                                <p style='font-size: 14px; line-height: 140%;'><span style='font-size: 18px; line-height: 25.2px; color: #666666;'>We have sent you this email in response to your request to create Account of Privont as Real Estate.</span></p>
                                                                <p style='font-size: 14px; line-height: 140%;'>&nbsp;</p>
                                                                <p style='font-size: 14px; line-height: 140%;'><span style='font-size: 18px; line-height: 25.2px; color: #666666;'>To reset your password, please click the link below to Verify your Account: </span></p>
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
                                                                <a href='"+verificationLink+@"' target='_blank' style='box-sizing: border-box;display: inline-block;font-family: Lato,sans-serif;text-decoration: none;-webkit-text-size-adjust: none;text-align: center;color: #FFFFFF; background-color: #075e55; border-radius: 1px;-webkit-border-radius: 1px; -moz-border-radius: 1px; width:auto; max-width:100%; overflow-wrap: break-word; word-break: break-word; word-wrap:break-word; mso-border-alt: none;'>
                                                                    <span style='display:block;padding:15px 40px;line-height:120%;'><span style='font-size: 18px; line-height: 21.6px;'>Reset Password</span></span>
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
                                                                <p style='font-size: 14px; line-height: 140%;'><span style='color: #888888; font-size: 14px; line-height: 19.6px;'><em><span style='font-size: 16px; line-height: 22.4px;'>Please ignore this email if you did not request a password change.</span></em></span><br /><span style='color: #888888; font-size: 14px; line-height: 19.6px;'><em><span style='font-size: 16px; line-height: 22.4px;'>&nbsp;</span></em></span></p>
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
                                                                                        <img src='https://zp.hisabdaar.com/Newimages/image-2.png' alt='Facebook' title='Facebook' width='32' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important'>
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
                                                                                        <img src='https://zp.hisabdaar.com/Newimages/image-1.png' alt='Twitter' title='Twitter' width='32' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important'>
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
                                                                                        <img src='https://zp.hisabdaar.com/Newimages/image-3.png' alt='Instagram' title='Instagram' width='32' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important'>
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
        public ActionResult Verify(string token,string value,int Type)
        {
            // Look up the user with the given token in your database
            // Update the user's account status to "verified"
            int ID = int.Parse(General.Decrypt(value, "@AFG"));
            string sql = "";
            if (Type==1)
            {
                DataTable dt = General.FetchData($@"Select * from RealEstateInfo Where RealEstateAgentID={ID}");
                if(dt.Rows.Count>0)
                {
                    return RedirectToAction("LinkExpire", "RealEstateAgentInfo");
                }
                sql = $@"Update RealEstateInfo Set IsEmailVerified=1 Where RealEstateAgentID={ID}";
            }
            else if(Type==2)
            {
                DataTable dt = General.FetchData($@"Select * from LenderInfo Where LenderID={ID}");
                if (dt.Rows.Count > 0)
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
        public ActionResult GetSourceOfAPILeadIndex(int SourceID)
        {
            string Query = $@"
select APIConfig from APIConfigInfo where TypeID={SourceID} and RealEstateID={General.UserID}
";
            DataTable dt = General.FetchData(Query);
            if (dt.Rows.Count > 0)
            {
                return Json("true,"+dt.Rows[0]["APIConfig"].ToString());
            }
            else
            {
                return Json("false,");
            }
        }
    }
}