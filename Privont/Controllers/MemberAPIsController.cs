using Privont.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

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
                string Query = $@" select APITypeInfo.APITypeTitle,APIConfigInfo.* from APITypeInfo
	left outer join APIConfigInfo on APIConfigInfo.TypeID=APITypeInfo.APITypeID  and RealEstateID=" + RealEstateID;

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
    }
}