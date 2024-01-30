﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class ZipCodeController : Controller
    {
        // GET: ZipCode
        [HttpGet]
        public JsonResult GetZipCodeInfo(string ZipCodeID)
        {
            try
            {
                if (string.IsNullOrEmpty(ZipCodeID))
                {
                    ZipCodeID = 0.ToString();
                }
               DataTable dataTable = General.FetchData($@"select top (30) * from ZipCode where ZipCode like '%{ZipCodeID}%' order by ZipCodeID");

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dataTable);
                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", (dataTable.Rows.Count == 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK));
                JSResponse.Add("Message", "Zip Code Information");
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
        public static int GetZipCodeIDVIACodeName(string ZipCode)
        {
            DataTable dt = General.FetchData($@"select ZipCodeID from ZipCode where ZipCode = '{ZipCode}'");
            if(dt.Rows.Count > 0)
            {
                if (dt.Rows[0][0]!=DBNull.Value)
                {
                    return int.Parse(dt.Rows[0][0].ToString());
                }
            }
            return 0;
        }
    }
}