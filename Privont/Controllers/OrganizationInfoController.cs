using Privont.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class OrganizationInfoController : Controller
    {
        // GET: OrganizationInfo

        #region APIs
        [HttpGet]
        public JsonResult GetAllOrganization()
        {
            try
            {

                DataTable dt2 = General.FetchData($@"
select OrganizationInfo.*,ZipCode.ZipCode from OrganizationInfo
	left outer join ZipCode on ZipCode.ZipCodeID=OrganizationInfo.ZIPCodeID
");


                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt2);

                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Organization Information!", dbrows);
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
        [HttpPost]
        public JsonResult CreateOrganization(OrganizationInfo collection)
        {
            try
            {

                DataTable dtexist = General.FetchData($@"select * from OrganizationInfo where OrganizationTitle like '{collection.OrganizationTitle}'");
                if (dtexist.Rows.Count > 0)
                {
                    JsonResult res = GeneralApisController.ResponseMessage(HttpStatusCode.Conflict, "This Organization is already exist!", null);
                    return res;
                }
                string Quer = $@"INSERT INTO [dbo].[OrganizationInfo]
           ([OrganizationTitle]
           ,[Address]
           ,[PhoneNo]
           ,[RegistrationNo]
           ,[ZIPCodeID]
           ,[OfficeNo]
           ,[StreetNo]
           ,[StreetName]
           ,[Website]
           ,[EmailAddress]
           ,[Contact1]
           ,[Contact2]
           ,[Remarks]
           ,[Inactive]
           ,[OrganizationType])
     VALUES
           ('{collection.OrganizationTitle}'
           ,'{collection.Address}'
           ,'{collection.PhoneNo}'
           ,'{collection.RegistrationNo}'
           ,'{collection.ZIPCodeID}'
           ,'{collection.OfficeNo}'
           ,'{collection.StreetNo}'
           ,'{collection.StreetName}'
           ,'{collection.Website}'
           ,'{collection.EmailAddress}'
           ,'{collection.Contact1}'
           ,'{collection.Contact2}'
           ,'{collection.Remarks}'
           ,{(collection.Inactive ? 1 : 0)}
           ,{collection.OrganizationType})";
                Quer = Quer + " select @@identity as OrganizationID";
                DataTable dt = General.FetchData(Quer);
                DataTable dt2 = General.FetchData($@"
select OrganizationInfo.*,ZipCode.ZipCode from OrganizationInfo
	left outer join ZipCode on ZipCode.ZipCodeID=OrganizationInfo.ZIPCodeID where OrganizationInfo.OrganizationID={dt.Rows[0]["OrganizationID"].ToString()}
");

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt2);

                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Organization Information!", dbrows);
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
        [HttpPost]
        public JsonResult UpdateOrganization(OrganizationInfo collection)
        {
            try
            {
                DataTable dtexist = General.FetchData($@"select * from OrganizationInfo where OrganizationID!={collection.OrganizationID} and OrganizationTitle like '{collection.OrganizationTitle}'");
                if (dtexist.Rows.Count > 0)
                {
                    JsonResult res = GeneralApisController.ResponseMessage(HttpStatusCode.Conflict, "This Organization is already exist!", null);
                    return res;
                }
                string Quer = $@"
UPDATE [dbo].[OrganizationInfo]
   SET [OrganizationTitle] = '{collection.OrganizationTitle}'
      ,[Address] = '{collection.Address}'
      ,[PhoneNo] = '{collection.PhoneNo}'
      ,[RegistrationNo] = '{collection.RegistrationNo}'
      ,[ZIPCodeID] = '{collection.ZIPCodeID}'
      ,[OfficeNo] = '{collection.OfficeNo}'
      ,[StreetNo] = '{collection.StreetNo}'
      ,[StreetName] = '{collection.StreetName}'
      ,[Website] = '{collection.Website}'
      ,[EmailAddress] = '{collection.EmailAddress}'
      ,[Contact1] = '{collection.Contact1}'
      ,[Contact2] = '{collection.Contact2}'
      ,[Remarks] = '{collection.Remarks}'
      ,[Inactive] = {(collection.Inactive ? 1 : 0)}
      ,[OrganizationType] = {collection.OrganizationType}
 WHERE OrganizationID={collection.OrganizationID}
";
                General.ExecuteNonQuery(Quer);
                DataTable dt2 = General.FetchData($@"
select OrganizationInfo.*,ZipCode.ZipCode from OrganizationInfo
	left outer join ZipCode on ZipCode.ZipCodeID=OrganizationInfo.ZIPCodeID where OrganizationInfo.OrganizationID={collection.OrganizationID}
");

                List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(dt2);

                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Organization Information!", dbrows);
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
        #endregion


    }
}
