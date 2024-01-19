using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrivontAdmin.Controllers
{
    public class ReportsController : Controller
    {
        // Reports
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetLeadsInfo(string SourceID,int? UserID)
        {
            string Query = $@"select * from LeadInfo where 1=1";
            if (!string.IsNullOrEmpty(SourceID))
            {
                Query = Query + $@" ApiSource='{SourceID}'";
            }
            if (UserID>0)
            {
                Query = Query + $@" UserID='{UserID}'";
            }
            DataTable dt = General.FetchData(Query);
            return View(dt);
        }
        public ActionResult GetRealEstateInfo()
        {
            string Query = $@"select RealEstateAgentInfo.*,ZipCode.ZipCode,OrganizationInfo.OrganizationTitle,OrganizationInfo.OrganizationType from RealEstateAgentInfo 
	inner join ZipCode on ZipCode.ZipCodeID=RealEstateAgentInfo.ZipCodeID
	inner join OrganizationInfo on OrganizationInfo.OrganizationID=RealEstateAgentInfo.OrganizationID";
            DataTable dt = General.FetchData(Query);
            return View(dt);
        }
        public ActionResult GetLendersInfo(int? LenderType)
        {
            if(LenderType is null)
            {
                LenderType = 0;
            }
            string Query = $@"SELECT LenderInfo.LenderId, LenderInfo.FirstName, LenderInfo.LastName, LenderInfo.LicenseNo, LenderInfo.OrganizationID, LenderInfo.ZipCodeID, LenderInfo.OfficeNo, LenderInfo.StreetNo, LenderInfo.StreetName, LenderInfo.Website, 
                  LenderInfo.EmailAddress, LenderInfo.Contact1, LenderInfo.Contact2, LenderInfo.Remarks, LenderInfo.Inactive, LenderInfo.Password, LenderInfo.Username, LenderInfo.UserID, LenderInfo.UserType, LenderInfo.IsApproved, 
                  LenderInfo.ApprovedRemarks, LenderInfo.IsEmailVerified, ZipCode.ZipCode, OrganizationInfo.OrganizationTitle
FROM     LenderInfo INNER JOIN
                  ZipCode ON LenderInfo.ZipCodeID = ZipCode.ZipCodeID INNER JOIN
                  OrganizationInfo ON LenderInfo.OrganizationID = OrganizationInfo.OrganizationID";
            if (LenderType > 0)
            {
                Query = Query + $@" where LenderInfo.LenderId in (select LenderID from FavouriteLender)";
            }
            ViewBag.LenderType = LenderType;
            DataTable dt = General.FetchData(Query);
            return View(dt);
        }
        public ActionResult GetVendorInfo()
        {
            string Query = $@"
SELECT VendorInfo.VendorID, VendorInfo.VendorName, VendorInfo.PhoneNo, VendorInfo.RegistrationNo, VendorInfo.ZipCodeID, VendorInfo.OfficeNo, VendorInfo.StreetNo, VendorInfo.StreetName, VendorInfo.Website, VendorInfo.EmailAddress, 
                  VendorInfo.Contact1, VendorInfo.Contact2, VendorInfo.Remarks, VendorInfo.Inactive, ZipCode.ZipCode
FROM     VendorInfo INNER JOIN
                  ZipCode ON VendorInfo.ZipCodeID = ZipCode.ZipCodeID
";

            DataTable dt = General.FetchData(Query);
            return View(dt);
        }
        public ActionResult GetMessageInfo()
        {
            return View();
        }
      
    }
}
