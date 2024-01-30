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
    public class NotificationInfoController : Controller
    {
        // GET: NotificationInfo
        public ActionResult GetNotificationInfo(int UserID,int UserType,int EntryUserID,int EntryUserType)
        {

            string Query = $@"
select * from NotificationINfo 

inner join

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

)ProfileLogin on NotificationInfo.UserID=ProfileLogin.UserID and ProfileLogin.UserType={UserType} and NotificationInfo.EntryUserID !={EntryUserID} and NotificationInfo.EntryUserType={EntryUserType}

";
            //if(UserType == 2)
            //{

            //}
            //else if (UserType == 3)
            //{

            //}
            //else if (UserType == 4)
            //{

            //}
            //else if (UserType == 5)
            //{

            //}
            return View();
        }
        [HttpPost]
        public JsonResult PostNotification(NotificationInfo collection)
        {
            try
            {
                collection.InsertRecords();
                JsonResult jr = new JsonResult();
                //List<Dictionary<string, object>> dbrows = new General().GetAllRowsInDictionary(new DataTable());
                jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "Notification Created!", null);
                return jr;
            }
            catch (Exception ex)
            {
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
    }
}