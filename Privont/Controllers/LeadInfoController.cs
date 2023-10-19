﻿using Newtonsoft.Json;
using Privont.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static Privont.General;
using static Privont.Models.APILeads;

namespace Privont.Controllers
{
    public class LeadInfoController : Controller
    {
        // GET: LeadsInfo
        LeadInfo Model = new LeadInfo();
        public ActionResult Index(string Value)
        {
            string WhereClause= " Where 1=1 ";
            ViewBag.Message = Value;
            if(General.UserType == 2)
            {
                WhereClause = WhereClause+ $@" and UserID={General.UserID}";
            }
            List<LeadInfo> lst = General.ConvertDataTable<LeadInfo>(Model.GetIndexAllRecord(WhereClause));
            ViewBag.PricePoint = new DropDown().GetPricePoint();
            ViewBag.ApiLeadType = new DropDown().GetApiLeadType();
            Session[$"TotalCount"] = 0;
            return View(lst);
        }
        public ActionResult ClaimLead(int LeadID,int LenderID)
        {
            string Value = "0";
            bool Return = bool.Parse(General.FetchData($@"Select Isnull(ReadyToOptin,0)ReadyToOptin from LeadInfo Where LeadID = {LeadID}").Rows[0]["ReadyToOptin"].ToString());
            string Return1 = "0";
            string AlreadyAdded = "0";
            DataTable dt2 = General.FetchData($@"Select * from LeadClaimInfo Where LeadID = {LeadID} and ClaimingLender = {LenderID}");
            if(dt2.Rows.Count==0)
            {
                if (Return == false)
                {
                    string sql = $@"Insert Into LeadClaimInfo Values({LeadID},{LenderID},GetDate())";
                    General.ExecuteNonQuery(sql);
                    DataTable dt = General.FetchData($@"select Count(LeadID)LeadID from LeadClaimInfo Where LeadID = {LeadID}");
                    if (int.Parse(dt.Rows[0]["LeadID"].ToString()) == 3)
                    {
                        Value = "1";
                        General.ExecuteNonQuery($@"Update LeadInfo Set ReadyToOptin=1 Where LeadID={LeadID}");
                    }
                }
                else
                {
                    Value = "1";
                    Return1 = "1";
                }
            }
            else
            {
                AlreadyAdded = "1";
            }
            return Json("true,"+Value+","+ Return1 + "," + AlreadyAdded);
        }
        public ActionResult Create()
        {
            LeadInfo model = new LeadInfo();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(LeadInfo collection)
        {
            try
            {
                if (collection.LeadID == 0)
                {
                    Model.InsertRecord(collection);
                }
                else
                {
                    Model.UpdateRecord(collection);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View(collection);
            }
        }
        [HttpPost]
        public ActionResult UploadLeadFile(HttpPostedFileBase file)
        {
            string _path = "";
            string Message = "";
            try
            {

                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string fname = _FileName;
                    _path = Path.Combine(Server.MapPath("~/UploadedFiles/Lead"), _FileName);
                    int j = 1;
                    //string fileext = "";
                    while (System.IO.File.Exists(_path))
                    {
                        _FileName = j + "-" + _FileName;
                        _path = Path.Combine(Server.MapPath("~/UploadedFiles/Lead"), _FileName);
                        j++;
                    }
                    string filepath = ("/UploadedFiles/Lead/") + _FileName;
                    file.SaveAs(_path);
                    using (var reader = new StreamReader(_path))
                    {
                        List<LeadInfo> lstLeadInfo = new List<LeadInfo>();
                        LeadInfo Lead;
                        int i = 0;
                        while (!reader.EndOfStream)
                        {
                            Lead = new LeadInfo();
                            var line = reader.ReadLine();
                            if (i == 0)
                            {

                            }
                            else
                            {
                                var values = line.Split(',');
                                if (values.Length >= 4)
                                {
                                    Lead.FirstName = values[0].Trim().Replace("'", "");
                                    Lead.LastName = values[1].Trim().Replace("'", "").Replace("*", " x ");
                                    Lead.PhoneNo = values[2].Trim().Replace("'","").Replace("*", " x ");
                                    Lead.EmailAddress = values[3].Trim().Replace("'", "").Replace("*", " x ");
                                    lstLeadInfo.Add(Lead);
                                }
                            }
                            i++;
                        }
                       int Value =  SaveRecord(lstLeadInfo);
                        if(Value>0)
                        {
                            Message = "Error Uploading File";
                            return RedirectToAction("Index", new { id=Message});
                        }
                        else
                        {
                            Message = "Data SuccessFully Uploaded";
                            return RedirectToAction("Index", new { id = Message });
                        }
                    }
                }
                Message = "Data SuccessFully Uploaded";
                return RedirectToAction("Index", new { id = Message });
            }
            catch (Exception ex)
            {
                Message = "File upload failed!! (exception: " + ex.Message + ") to path " + _path;
                return RedirectToAction("Index", new { id = Message });
            }
        }

        public int SaveRecord(List<LeadInfo> lstLeadInfo)
        {
            try
            {
                if (lstLeadInfo is null)
                {
                    lstLeadInfo = new List<LeadInfo>();
                }
                string sql = "";
                foreach (LeadInfo sd in lstLeadInfo)
                {
                    sql = sql + $@"INSERT INTO[dbo].[LeadInfo]
                    ([FirstName]
           ,[LastName]
                    ,[PhoneNo]
           ,[EmailAddress],
[UserID],
[EntryDateTime],
[EntrySource])
     VALUES
           ('{sd.FirstName}'
           , '{sd.LastName}'
           , '{sd.PhoneNo}'
           , '{sd.EmailAddress}'
,{General.UserID},
GetDate(),
{General.UserType})";
                }
                General.ExecuteNonQuery(sql);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        public DataTable Validate(List<Models.LeadInfo> lstLeadInfo)
        {
            DataTable dtanomalies = new DataTable();
            dtanomalies.Columns.Add("SrNo");
            dtanomalies.Columns.Add("RowNo");
            dtanomalies.Columns.Add("Anomalie");

            int index = 1;
            //Check Name Duplication
            //Check Code Duplication
            //Check Code Missing
            //Check Empty Category
            //Check Empty Brand
            //Check Empty Sale account and Purchase Account

            if (lstLeadInfo.Count > 2500)
            {
                string problem = "Maximum 2500 Leads can be uploaded in one go.";

                DataRow dr = dtanomalies.NewRow();
                dr["RowNo"] = 1;
                dr["Anomalie"] = problem;
                dtanomalies.Rows.Add(dr.ItemArray);
                return dtanomalies;

            }

            foreach (LeadInfo pi in lstLeadInfo)
            {
                string problem = "";
                bool anomaliefound = false;
                if (lstLeadInfo.Where(x => x.PhoneNo == pi.PhoneNo && x.FirstName==pi.FirstName && x.LastName == pi.LastName && x.EmailAddress == pi.EmailAddress).Count() > 1)
                {
                    problem = "Duplication of Data";
                    anomaliefound = true;
                }
                if (anomaliefound == true)
                {
                    DataRow dr = dtanomalies.NewRow();
                    dr["RowNo"] = index;
                    dr["Anomalie"] = problem;
                    dtanomalies.Rows.Add(dr.ItemArray);
                }
                index = index + 1;
            }
            return dtanomalies;
        }
        public ActionResult Edit(int id)
        {
            List<LeadInfo> lst = General.ConvertDataTable<LeadInfo>(Model.GetAllRecord(" where LeadID=" + id));
            return View("Create", lst[0]);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string SQL = $@"Delete from LeadInfo where LeadID=" + id;
            General.ExecuteNonQuery(SQL);
            return Json("true");
        }

        public ActionResult GetleadInfo(string txtTitle)
      {
            if (txtTitle is null)
            {
                txtTitle = "";
            }
            DataTable dtEmployee = General.FetchData($@"Select Top 30 (FirstName+' '+lastName)Value,PhoneNo,EmailAddress from LeadInfo Where FirstName like '%{txtTitle}%' or LastName Like '%{txtTitle}%' or PhoneNo Like '%{txtTitle}%' ");
            List<Dictionary<string, object>> dbrows = new List<Dictionary<string, object>>();//GetTableRows(dtEmployee);

            Dictionary<string, object> dbitem = new Dictionary<string, object>();
            foreach (DataRow dr in dtEmployee.Rows)
            {
                dbitem = new Dictionary<string, object>();
                dbitem.Add("value", dr["PhoneNo"].ToString());
                dbitem.Add("Email", dr["EmailAddress"].ToString());
                dbitem.Add("label", dr["Value"].ToString());
                dbrows.Add(dbitem);
            }
            Dictionary<string, object> products = new Dictionary<string, object>();
            products.Add("ZipCode", dbrows);
            return Json(dbrows, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddLeadExpiryTime()
        {
            ViewBag.ExpiryDate = General.FetchData("Select isnull(ExpiryTime,0)ExpiryTime from LeadExpiryTime").Rows[0]["ExpiryTime"].ToString();
            return View();   
        }
        public ActionResult SaveLeadExpiryTime(int Time)
        {
            string sql = " Delete from LeadExpiryTime";
            sql = sql + $@" Insert Into LeadExpiryTime values({Time})";
            General.ExecuteNonQuery(sql);
            return Json("true");
        }

        [HttpPost]
        public ActionResult CreateLeadPricePoint(string PricePoint)
        {
            try
            {
                string SQL = "";
                // TODO: Add insert logic here
                string Query = "Insert into LeadPricePoint(PricePoint,Inactive) ";
                Query = Query + "Values ('" + PricePoint + "'," + 0 + ")";
                Query = Query + "   Select @@IDENTITY AS PricePointID";
                int PricePointID = int.Parse(General.FetchData(Query).Rows[0]["PricePointID"].ToString());
                return Json(PricePointID);
            }
            catch
            {
                return View();
            }
        }
        public ActionResult LeadPricePoint(int LeadID,int PricePoint)
        {
            string value = "0";
            DataTable dt = General.FetchData($@"Select PricePointID from leadInfo Where LeadID={LeadID}");
            if (dt.Rows[0]["PricePointID"]==DBNull.Value)
            {
                string sql = $@"Update LeadInfo Set PricePointID={PricePoint} Where LeadID={LeadID}";
                General.ExecuteNonQuery(sql);
            }
            else
            {
                value = "1";
            }
            return Json("true,"+value);
        }
        public int TotalCount{get;set;} 
        public ActionResult GetInfoSync(string ApiSource)
        {
            int val = 0;
            int TotalCount = int.Parse(Session[$"TotalCount"].ToString());
            DataTable dt = General.FetchData($@"select count(*)AddedVal from LeadInfo where EntrySource=2 and ApiSource='{ApiSource}' and UserID={General.UserID} and ApiLeadID>0");
            int.TryParse(dt.Rows[0]["AddedVal"].ToString(), out val);
            if(TotalCount > 0 )
                val= (val / TotalCount)*100;
            return Json(val);
        }
        public ActionResult GetSourceOfAPILeadIndex(int UserID,int SourceID)
        {
            if(UserID==0)
            {
                UserID = General.UserID;
            }
            string Query = $@"
select APIConfig from APIConfigInfo where TypeID={SourceID} and RealEstateID={UserID}
";
            DataTable dt = General.FetchData(Query);
            if (dt.Rows.Count > 0)
            {
                FetchingLeadsFromAPIs(UserID,SourceID);
                return Json("true," + dt.Rows[0]["APIConfig"].ToString());
            }
            else
            {
                return Json("false,");
            }
        }
        public string GetAPIOfFollowUPBoss()
        {
            return ""+ $@"https://api.followupboss.com/v1/people?sort=created&limit=100&offset=0&includeTrash=false&includeUnclaimed=false";
        }
        public void FetchingLeadsFromAPIs(int UserID,int APISourceID)
        {
            var NextLink = GetAPIOfFollowUPBoss();
            string GetingLastAddedLink = new GeneralApisController().LastAddedLink_APILog(LogSource.FollowUpBoss);
            if (!string.IsNullOrEmpty(GetingLastAddedLink))
            {
                NextLink = GetingLastAddedLink;
            }
            TotalCount = 2;
            for(int i = 1; i <= TotalCount;)
            {

                string Response = General.FetchDataFromAPIs(NextLink, General.GetAPIAuthKey(UserID,APISourceID));
                if (Response != "Error")
                {
                    Root root = JsonConvert.DeserializeObject<Root>(Response);
                    TotalCount = root._metadata.total;
                    Session[$"TotalCount"] = TotalCount;
                    foreach (var item in root.people)
                    {
                        LeadInfo objnew = new LeadInfo();
                        objnew.FirstName = item.firstName;
                        objnew.LastName = item.lastName;
                        if (item.phones.Count > 0)
                        {
                            objnew.PhoneNo = item.phones[0].value;
                        }
                        if (item.emails.Count > 0)
                        {
                            objnew.EmailAddress = item.emails[0].value;
                        }
                        objnew.ApiLeadID = item.id;
                        objnew.APITypeID = APISourceID;
                        objnew.ApiSource = item.source;
                        objnew.UserID = UserID;
                        objnew.UserType = 2;
                        if(!new GeneralApisController().CheckIfLeadsAlreadyExist(UserID,objnew.ApiLeadID))
                        {
                            Model.InsertRecord(objnew);
                        }
                        i++;
                    }
                    NextLink = root._metadata.nextLink;
                    // Last Add Leads
                    int ID = LastAddedAPI();
                    GeneralApisController.InsertAPILogs(LogTypes.APICall, LogSource.FollowUpBoss, ID, "Follow Up Boss API Records", root._metadata.nextLink);
                }
                else
                {

                }
            }
           
        }
        public int LastAddedAPI()
        {
            int LeadID = 0;
            DataTable dt = General.FetchData($@"select MAX(LeadInfo.LeadID)LeadID from LeadInfo");
            int.TryParse(dt.Rows[0]["LeadID"].ToString(),out LeadID);
            return LeadID;
        }
    }
}