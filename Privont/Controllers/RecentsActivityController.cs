using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class RecentsActivityController : Controller
    {
        // GET: RecentsActivity
        [HttpGet]
        public JsonResult Recents(int UserID, int UserType)
        {
            try
            {
                string WhereClause = "";
                DataTable dataTable = new DataTable();
                JsonResult jr = new JsonResult();
                string VendorQuery = $@"
declare @UserID as int = {UserID}
declare @UserType as int = {UserType}

Select top (3) VendorInfo.*,ZipCode.ZipCode,OrganizationInfo.OrganizationTitle,(select count(*) from FavouriteVendorInfo where FavouriteVendorInfo.FavouriteVendorID=VendorInfo.VendorID and FavouriteVendorInfo.UserID=@UserID
                    and FavouriteVendorInfo.UserType=@UserType ) as Favourite 
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

order by VendorID desc";
                DataTable dtVendors = General.FetchData(VendorQuery);

                //Transaction History Top 3
                string TransQuery = $@"select top (3) * From TransactionDetails where UserID={UserID} and UserType={UserType}  order by TransactionID desc";
                DataTable dtTrans = General.FetchData(TransQuery);

                List<Dictionary<string, object>> bdRowsVendors = new General().GetAllRowsInDictionary(dtVendors);
                List<Dictionary<string, object>> bdRowsTrans = new General().GetAllRowsInDictionary(dtTrans);

                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.OK);
                JSResponse.Add("Message", "Recent Activity");
                JSResponse.Add("Vendors", bdRowsVendors);
                JSResponse.Add("Transaction", bdRowsTrans);

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
                JsonResult jr = GeneralApisController.ResponseMessage(HttpStatusCode.BadRequest, "Error: " + ex.Message, null);
                return jr;
            }
        }
    }
}