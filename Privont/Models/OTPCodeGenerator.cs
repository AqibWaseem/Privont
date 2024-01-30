using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using TrueDialog.Model;

namespace Privont.Models
{
    public class OTPCodeGenerator
    {
        public DataTable SendOTPCode(int UserID, int UserType, int RequestedBy)
        {
            bool flag = false;
            DataTable dt = new DataTable();
            string WhereClause = $@"where UserID='{UserID}' and Password='{UserType}'";
            dt = GetUserDetails(WhereClause);
            if (dt.Columns.Count == 0)
            {
                dt = new DataTable();
                dt.Columns.Add("SMSOtp");
                dt.Columns.Add("EmailOtp");
                dt.Columns.Add("Status");
                return dt;
            }
            string MobileNo = dt.Rows[0]["Contact1"].ToString();
            string Email = dt.Rows[0]["EmailAddress"].ToString();
            int Current_SMSOTP = 0;
            int Current_EmailOTP = 0;
            GeneratorOTP(UserID, UserType, MobileNo, Email, ref Current_SMSOTP, ref Current_EmailOTP);

            //Where To Send OTP
            if (RequestedBy == 1)//Send By SMS
            {
                try
                {
                    string smstext = "OTP is generated: " + Current_SMSOTP + ". This OTP is valid for next 10 minutes.";

                }
                catch (Exception ex)
                {
                    throw new Exception("Unabled to send SMS!");
                }
            }
            else if (RequestedBy == 2)//Send Email
            {
                try
                {
                    string smstext = "OTP is generated: " + Current_EmailOTP + ". This OTP is valid for next 10 minutes.";

                }
                catch (Exception ex)
                {
                    throw new Exception("Unabled to send Email!");
                }
            }
            else//Send OTP on Both SMS and EMAIL
            {

            }

            //Get OTP Status
            string status = GetStatus(RequestedBy,MobileNo, Email, Current_EmailOTP, Current_SMSOTP);


            DataRow dr = dt.NewRow();
            if(RequestedBy == 1)
            {
                dr["SMSOtp"] = Current_SMSOTP.ToString();
                dr["EmailOtp"] = "".ToString();
            }
            else if(RequestedBy == 2)
            {
                dr["SMSOtp"] = "".ToString();
                dr["EmailOtp"] = Current_EmailOTP.ToString();
            }
            else
            {
                dr["SMSOtp"] = Current_SMSOTP.ToString();
                dr["EmailOtp"] = Current_EmailOTP.ToString();
            }
           
            dr["Status"] = status;
            dt.Rows.Add(dr);
            return dt;
        }
        public string GetStatus(int RequestedBy, string MobileNo,string Email,int EmailOTP,int SMSOTP)
        {
            var maskedMoble = MobileNo.Substring(MobileNo.Length - 3);
            string[] emailstring = Email.Split('@');
            string frstpartemail = emailstring[0].ToString();
            string secondpartemail = emailstring[1].ToString();
            string maskedemail = frstpartemail.Substring(0, 1).ToString() + "xxxxxxx" + frstpartemail.Substring(frstpartemail.Length - 1).ToString() + "@" + secondpartemail;

            string status = "";
            if (RequestedBy == 1)
            {
                status = "An Otp Has been sent to your Registered Mobile no : 03xxxxxxx" + maskedMoble.ToString() + ", SMSOTP : " + SMSOTP;
            }
            else if (RequestedBy == 2)
            {
                status = "An Otp Has been sent to your Registered Email :" + maskedemail + ", Email OTP : " + EmailOTP ;
            }
            else
            {
                status = "An Otp Has been sent to your Registered Mobile no : 03xxxxxxx" + maskedMoble.ToString() + ". and Your Registered Email :" + maskedemail + " Email OTP : " + EmailOTP + ", SMSOTP : " + SMSOTP;
            }
            
            return status;
        }
        public DataTable VerifyOTP(string Username,int SMSOTP,int EmailOTP)
        {
            string WhereClause = $@" where Username='{Username}'";
            DataTable dt = GetUserDetails(WhereClause);
            if (dt.Rows.Count==0)
            {
                throw new Exception("User not exist!");
                
            }
            dt = General.FetchData($@"select * from OTPInfo where UserID={dt.Rows[0]["UserID"]} and UserType={dt.Rows[0]["UserType"]} and (SMSOTP='{SMSOTP}' or EmailTOP='{EmailOTP}')");
            
            return dt;
        }
        private void GeneratorOTP(int UserID,int UserType,string MobileNo,string Email,ref int OTP_SMS,ref int OTP_Email )
        {
            OTP_SMS = 0;
            OTP_Email = 0;
            OTP_SMS = GenerateRandomNo();
            OTP_Email = GenerateRandomNo(OTP_SMS);
            if (OTP_SMS == OTP_Email)
            {
                OTP_Email = GenerateRandomNo(OTP_Email);
            }

            string query = $@" update OTPInfo set InActive = 1 where UserId = {UserID}  and UserType={UserType}


insert into OTPInfo (
 SMSOTP
,EmailOTP
,UserID
,UserType
,SendDate
,MobileNo
,Email
,Inactive
) values (" + OTP_SMS + "," + OTP_Email + "," + UserID + "," + UserType + ",GETDATE(),'" + MobileNo + "','" + Email + "',0)";
            General.ExecuteNonQuery(query);

        }
        private int GenerateRandomNo(int min = 0001)
        {
            int _min = min;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        public static DataTable GetUserDetails(string WhereClause="")
        {
            string sql1 = $@"
select * from 

(
Select (RealEstateAgentId)UserID,UserName,Password,FirstName,LastName,StreetNo,StreetName,EmailAddress,Contact1,Website,Remarks,Inactive,2 as UserType 
from RealEstateAgentInfo 

union all
Select (LenderId)UserID,UserName,Password,FirstName,LastName,StreetNo,StreetName,EmailAddress,Contact1,Website,Remarks,Inactive,3 as UserType 
from LenderInfo  
union all

SELECT        LeadID,Username,Password, FirstName, LastName,'' StreetNo, '' as StreetName, EmailAddress, Contact1,'' as Website,  Remarks,0 as
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
            return dt;
        }
    }
}