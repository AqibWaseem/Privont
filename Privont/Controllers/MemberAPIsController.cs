using Newtonsoft.Json;
using Privont.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Privont.General;

namespace Privont.Controllers
{
    public class MemberAPIsController : Controller
    {
        // GET: MemberAPIs
        [HttpGet]
        public JsonResult GetAPIType()
        {
            try
            {
                JsonResult jr = new JsonResult();
              
                string Query = $@"select * from APITypeInfo" ;

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(General.FetchData(Query));

                jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "API Types Information!", dbrows);
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
        [HttpGet]
        public JsonResult GetAPIConfigInfo(int RealEstateID = 0)
        {
            try
            {
                string WhereClause = "";
                JsonResult jr = new JsonResult();
                if (RealEstateID == 0)
                {
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "ID cannot be zero", null);
                    return jr;
                }
                string Query = $@" SELECT  APITypeInfo.APITypeID as TypeID,
		APITypeInfo.APITypeTitle, 
		ISNULL(APIConfigInfo.APIConfigID,0)APIConfigID, 
		ISNULL(APIConfigInfo.RealEstateID,0)RealEstateID, 
		ISNULL(APIConfigInfo.APIConfig,'')APIConfig
FROM            APITypeInfo LEFT OUTER JOIN
                         APIConfigInfo ON APIConfigInfo.TypeID = APITypeInfo.APITypeID AND APIConfigInfo.RealEstateID = " + RealEstateID;

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(General.FetchData(Query));

                jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "API Configuration Information!", dbrows);
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
        [HttpPost]
        public ActionResult PostAPIConfiguration(List<APIConfigInfo> collection)
        {
            try
            {
                string Query = "";
                General.ExecuteNonQuery($@" delete from APIConfigInfo where RealEstateID=" + collection[0].RealEstateID);
                foreach (APIConfigInfo item in collection)
                {
                    Query = $@"INSERT INTO APIConfigInfo
           (RealEstateID
           ,TypeID
           ,APIConfig)
     VALUES
           ({item.RealEstateID}
           ,{item.TypeID}
           ,'{item.APIConfig}')";
                    General.ExecuteNonQuery(Query);
                }
                Query = $@" select APITypeInfo.APITypeTitle,APIConfigInfo.* from APITypeInfo
	left outer join APIConfigInfo on APIConfigInfo.TypeID=APITypeInfo.APITypeID  and RealEstateID=" + collection[0].RealEstateID;

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(General.FetchData(Query));
                JsonResult jr = new JsonResult();
                jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "API Configuration Information!", dbrows);
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }





        //Sync Method for Consuming APIs
        LeadInfoController LeadController = new LeadInfoController();
        public int TotalCount { get; set; }
        public void FetchingLeadsFromAPIs(int UserID, int APISourceID)
        {
            var NextLink = LeadController.GetAPIOfFollowUPBoss();
            string GetingLastAddedLink = new GeneralApisController().LastAddedLink_APILog(APISourceID);
            if (!string.IsNullOrEmpty(GetingLastAddedLink))
            {
                NextLink = GetingLastAddedLink;
            }
            TotalCount = 2;
            for (int i = 1; i <= TotalCount;)
            {

                string Response = General.FetchDataFromAPIs(NextLink, General.GetAPIAuthKey(UserID, APISourceID));
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
                            objnew.Contact1 = item.phones[0].value;
                            objnew.Contact2 = item.phones[0].value;
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
                        objnew.UniqueIdentifier = GeneralApisController.GenerateUniqueIdentifier(objnew.UserType);
                        if (!new GeneralApisController().CheckIfLeadsAlreadyExist(UserID, objnew.ApiLeadID))
                        {
                            InsertAPILeads(objnew);
                        }
                        i++;
                    }
                    NextLink = root._metadata.nextLink;
                    // Last Add Leads
                    int ID = LeadController.LastAddedAPI();
                    GeneralApisController.InsertAPILogs(LogTypes.APICall, LogSource.FollowUpBoss, ID, LeadController.APITypeTitle(APISourceID) + " API Records", root._metadata.nextLink);
                }
                else
                {

                }
            }

        }
        public int InsertAPILeads(LeadInfo Collection)
        {
            int LeadID = 0;
            string Query = $@"INSERT INTO [dbo].[LeadInfo]
           ([FirstName]
           ,[LastName]
           ,[PhoneNo]
           ,[EmailAddress]
           ,[EntryDateTime]
           ,[EntryClientID]
           ,[EntrySource]
           ,[OptInSMSStatus]
           ,[HasOptIn]
           ,[OptInDateTime]
           ,[AvailingLender]
           ,[UserID]
           ,[ReadytoOptin]
           ,[PricePointID]
           ,[isClaimLead]
           ,[ApiSource]
           ,[ApiLeadID]
           ,[APITypeID]
           ,[SMSSent]
           ,[Username]
           ,[Password]
           ,[Contact1]
           ,[UserType]
           ,[IsApproved]
           ,[IsEmailVerified]
           ,[UniqueIdentifier]
           ,[SourceID]
           ,[PriceRangeID]
           ,[State]
           ,[FirstTimeBuyer]
           ,[IsMilitary]
           ,[TypeID]
           ,[BestTimeToCall]
           ,[IsPrivontFamily]
           ,[Remarks])
     VALUES
           ('{Collection.FirstName}'
           ,'{Collection.LastName}'
           ,'{Collection.PhoneNo}'
           ,'{Collection.EmailAddress}'
           ,GETDATE()
           ,'{Collection.UserID}'
           ,2
           ,'{Collection.OptInSMSStatus}'
           ,'0'
           ,NULL
           ,0
           ,'{Collection.UserID}'
           ,'{Collection.ReadytoOptin}'
           ,'{Collection.PricePointID}'
           ,'{Collection.isClaimLead}'
           ,'{Collection.ApiSource}'
           ,'{Collection.ApiLeadID}'
           ,'{Collection.APITypeID}'
           ,'{Collection.SMSSent}'
           ,'{Collection.Username}'
           ,'{Collection.Password}'
           ,'{Collection.Contact1}'
           ,'{Collection.UserType}'
           ,'{(Collection.IsApproved ?1:0)}'
           ,'{Collection.IsEmailVerified}'
           ,'{Collection.UniqueIdentifier}'
           ,'{Collection.SourceID}'
           ,'{Collection.PriceRangeID}'
           ,'{Collection.State}'
           ,'{(Collection.FirstTimeBuyer ? 1 : 0)}'
           ,'{(Collection.IsMilitary ? 1 : 0)}'
           ,'{Collection.TypeID}'
           ,'{Collection.BestTimeToCall}'
           ,'0'
           ,'{Collection.Remarks}')";
            General.ExecuteNonQuery(Query);
            return LeadID;
        }


    }
}