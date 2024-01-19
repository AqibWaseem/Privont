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
    public class UserProfileController : Controller
    {
        // GET: UserProfile

        //Update User Profile Information
        [HttpPost]
        public JsonResult Update_UserProfileInformation(RealEstateAgentInfo collection)
        {
            string Query = "";
            Dictionary<string, object> JSResponse = new Dictionary<string, object>();
            DataTable dt = new DataTable();
            //DataTable dt = new RealEstateAgentInfo().UserExistanceInfo(collection.Username);
            //if (dt.Rows.Count > 0)
            //{
            //    JSResponse = new Dictionary<string, object>();
            //    JSResponse.Add("Status", 401);
            //    JSResponse.Add("Message", "Username Already Exist Please Use different Username!");
            //    JSResponse.Add("Data", DBNull.Value);

            //    JsonResult jr = new JsonResult()
            //    {
            //        Data = JSResponse,
            //        ContentType = "application/json",
            //        ContentEncoding = System.Text.Encoding.UTF8,
            //        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            //        MaxJsonLength = Int32.MaxValue
            //    };
            //    return jr;
            //}
            int _ZipCodeID = ZipCodeController.GetZipCodeIDVIACodeName(collection.ZipCode);
            if (_ZipCodeID > 0)
            {
                collection.ZipCodeID = _ZipCodeID;
            }
            else
            {
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", 402);
                JSResponse.Add("Message", "Zip Code Not exist!");
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
            int RealEstateAgentId = 0;
            if (collection.UserType == 2)
            {
                RealEstateAgentId = new RealEstateAgentInfo().UpdateProfileRecord(collection);
                bool Response = new SocialMediaInfoController().InsertSocialMediaVIAUserIDandUserType(collection.lstSocialMediaInfo, RealEstateAgentId, 2);
            }
            else if (collection.UserType == 3)
            {
                LenderInfo LenderCollection = new LenderInfo();
                General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, LenderInfo>(collection, LenderCollection);
                RealEstateAgentId = new LenderInfo().UpdateProfileRecord(LenderCollection);
                bool Response = new SocialMediaInfoController().InsertSocialMediaVIAUserIDandUserType(collection.lstSocialMediaInfo, RealEstateAgentId, 3);
            }
            else if (collection.UserType == 4)
            {
                LeadInfo LeadInfoCollection = new LeadInfo();
                General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, LeadInfo>(collection, LeadInfoCollection);
                RealEstateAgentId = new LeadInfo().UpdateProfileRecord(LeadInfoCollection);
                bool Response = new SocialMediaInfoController().InsertSocialMediaVIAUserIDandUserType(collection.lstSocialMediaInfo, RealEstateAgentId, 4);
            }
            else if (collection.UserType == 5)
            {
                VendorInfo VendorInfoCollection = new VendorInfo();
                General.MappingValueFromSourceClassToTargetClass<RealEstateAgentInfo, VendorInfo>(collection, VendorInfoCollection);
                RealEstateAgentId = new VendorInfo().UpdateProfileRecord(VendorInfoCollection);
                bool Response = new SocialMediaInfoController().InsertSocialMediaVIAUserIDandUserType(collection.lstSocialMediaInfo, RealEstateAgentId, 5);
            }
            if (RealEstateAgentId == 0)
            {
                JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Unabled to Update Records... Please contact to the administration!");
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
            else
            {
                List<Dictionary<string, object>> dbrows = new List<Dictionary<string, object>>();
                if (collection.UserType == 2)
                {

                    Query = $@"select RealEstateAgentInfo.*,OrganizationInfo.OrganizationTitle,ZipCode.ZipCode
 from RealEstateAgentInfo
inner join OrganizationInfo on RealEstateAgentInfo.OrganizationID = OrganizationInfo.OrganizationID
 inner join ZipCode on RealEstateAgentInfo.ZipCodeID = ZipCode.ZipCodeID
 Where  RealEstateAgentId=" + RealEstateAgentId;
                    dt = General.FetchData(Query);
                    List<RealEstateAgentInfo> ModelLst = new RealEstateAgentInfo().GetRealEstateAgentRecordsInList(dt, collection.UserType);
                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "Data Updated Successfully!");
                    JSResponse.Add("Data", ModelLst);
                }
                else if (collection.UserType == 3)
                {
                    Query = $@"Select LenderInfo.*,OrganizationInfo.OrganizationTitle,ZipCode.ZipCode from LenderInfo 
left outer join OrganizationInfo on LenderInfo.OrganizationID = OrganizationInfo.OrganizationID
left outer join ZipCode on LenderInfo.ZipCodeID = ZipCode.ZipCodeID

 Where LenderId=" + RealEstateAgentId;
                    dt = General.FetchData(Query);
                    List<LenderInfo> ModelLst = new LenderInfo().GetLenderInfoRecordsInList(dt, collection.UserType);
                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "Data Updated Successfully!");
                    JSResponse.Add("Data", ModelLst);
                }
                else if (collection.UserType == 4)
                {

                    Query = $@"select * from LeadInfo where LeadID=" + RealEstateAgentId;
                    dt = General.FetchData(Query);

                    List<LeadInfo> ModelLst = new LeadInfo().GetLeadInfoRecordsInList(dt, collection.UserType);
                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "Data Updated Successfully!");
                    JSResponse.Add("Data", ModelLst);
                }
                else if (collection.UserType == 5)
                {
                    Query = $@"select * from VendorInfo
inner join OrganizationInfo on VendorInfo.OrganizationID = OrganizationInfo.OrganizationID
 inner join ZipCode on VendorInfo.ZipCodeID = ZipCode.ZipCodeID
where VendorID=" + RealEstateAgentId;
                    dt = General.FetchData(Query);

                    List<VendorInfo> ModelLst = new VendorInfo().GetVendorInfoRecordsInList(dt, collection.UserType);
                    JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "Data Updated Successfully!");
                    JSResponse.Add("Data", ModelLst);
                }

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
        [HttpGet]
        public JsonResult UserProfileInfo(int UserID, int UserType)
        {
            try
            {
                JsonResult jr = new JsonResult();
                string Query = "";
                DataTable dt = new DataTable();
                DataTable dtSocialMedia = new DataTable();
                List<Dictionary<string, object>> dbrows = new List<Dictionary<string, object>>();
                List<Dictionary<string, object>> dbSocialMedia = new List<Dictionary<string, object>>();
                if (UserType == 1 || UserType > 5)
                {
                    jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "This User not exist!", null);
                }
                else if (UserType == 2)//
                {
                    Query = $@"select RealEstateAgentInfo.*,OrganizationInfo.OrganizationTitle,ZipCode.ZipCode
 from RealEstateAgentInfo
inner join OrganizationInfo on RealEstateAgentInfo.OrganizationID = OrganizationInfo.OrganizationID
 inner join ZipCode on RealEstateAgentInfo.ZipCodeID = ZipCode.ZipCodeID
 Where  RealEstateAgentId=" + UserID;
                    dt = General.FetchData(Query);
                    List<RealEstateAgentInfo> ModelLst = new RealEstateAgentInfo().GetRealEstateAgentRecordsInList(dt, UserType);
                    //                     dtSocialMedia=General.FetchData($@"SELECT        SocialMediaInfo.SocialMediaID, SocialMediaInfo.SocialMediaTitle, UserSocialMediaInfo.ProfileName, 
                    //UserSocialMediaInfo.ProfileLink, UserSocialMediaInfo.UserID, 
                    //UserSocialMediaInfo.UserTypeID 

                    //FROM            SocialMediaInfo LEFT OUTER JOIN
                    //                         UserSocialMediaInfo ON SocialMediaInfo.SocialMediaID = UserSocialMediaInfo.SocialMediaID
                    //						 where UserSocialMediaInfo.UserID={UserID} and UserTypeID={UserType}");
                    //                    dbSocialMedia = new General().GetAllRowsInDictionary(dtSocialMedia);


                    //dbrows = new General().GetAllRowsInDictionary(dt);

                    //jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "User Profile Information!", dbrows);
                    Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "User Profile Information!");
                    JSResponse.Add("Data", ModelLst);

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
                else if (UserType == 3)//
                {
                    Query = $@"Select LenderInfo.*,OrganizationInfo.OrganizationTitle,ZipCode.ZipCode from LenderInfo 
left outer join OrganizationInfo on LenderInfo.OrganizationID = OrganizationInfo.OrganizationID
left outer join ZipCode on LenderInfo.ZipCodeID = ZipCode.ZipCodeID

 Where LenderId=" + UserID;
                    dt = General.FetchData(Query);
                    List<LenderInfo> ModelLst = new LenderInfo().GetLenderInfoRecordsInList(dt, UserType);
                    //                    dtSocialMedia = General.FetchData($@"SELECT        SocialMediaInfo.SocialMediaID, SocialMediaInfo.SocialMediaTitle, UserSocialMediaInfo.ProfileName, 
                    //UserSocialMediaInfo.ProfileLink, UserSocialMediaInfo.UserID, 
                    //UserSocialMediaInfo.UserTypeID 

                    //FROM            SocialMediaInfo LEFT OUTER JOIN
                    //                         UserSocialMediaInfo ON SocialMediaInfo.SocialMediaID = UserSocialMediaInfo.SocialMediaID
                    //						 where UserSocialMediaInfo.UserID={UserID} and UserTypeID={UserType}");
                    //                    dbSocialMedia = new General().GetAllRowsInDictionary(dtSocialMedia);


                    //dbrows = new General().GetAllRowsInDictionary(dt);

                    //jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "User Profile Information!", dbrows);
                    Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "User Profile Information!");
                    JSResponse.Add("Data", ModelLst);

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
                else if (UserType == 4)
                {
                    Query = $@"select * from LeadInfo where LeadID=" + UserID;
                    dt = General.FetchData(Query);

                    List<LeadInfo> ModelLst = new LeadInfo().GetLeadInfoRecordsInList(dt, UserType);
                    //                    dtSocialMedia = General.FetchData($@"SELECT        SocialMediaInfo.SocialMediaID, SocialMediaInfo.SocialMediaTitle, UserSocialMediaInfo.ProfileName, 
                    //UserSocialMediaInfo.ProfileLink, UserSocialMediaInfo.UserID, 
                    //UserSocialMediaInfo.UserTypeID 

                    //FROM            SocialMediaInfo LEFT OUTER JOIN
                    //                         UserSocialMediaInfo ON SocialMediaInfo.SocialMediaID = UserSocialMediaInfo.SocialMediaID
                    //						 where UserSocialMediaInfo.UserID={UserID} and UserTypeID={UserType}");
                    //                    dbSocialMedia = new General().GetAllRowsInDictionary(dtSocialMedia);


                    //dbrows = new General().GetAllRowsInDictionary(dt);

                    //jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "User Profile Information!", dbrows);
                    Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "User Profile Information!");
                    JSResponse.Add("Data", ModelLst);

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
                else if (UserType == 5)
                {
                    Query = $@"select * from VendorInfo
inner join OrganizationInfo on VendorInfo.OrganizationID = OrganizationInfo.OrganizationID
 inner join ZipCode on VendorInfo.ZipCodeID = ZipCode.ZipCodeID
where VendorID=" + UserID;
                    dt = General.FetchData(Query);

                    List<VendorInfo> ModelLst = new VendorInfo().GetVendorInfoRecordsInList(dt, UserType);


                    //                    dtSocialMedia = General.FetchData($@"SELECT        SocialMediaInfo.SocialMediaID, SocialMediaInfo.SocialMediaTitle, UserSocialMediaInfo.ProfileName, 
                    //UserSocialMediaInfo.ProfileLink, UserSocialMediaInfo.UserID, 
                    //UserSocialMediaInfo.UserTypeID 

                    //FROM            SocialMediaInfo LEFT OUTER JOIN
                    //                         UserSocialMediaInfo ON SocialMediaInfo.SocialMediaID = UserSocialMediaInfo.SocialMediaID
                    //						 where UserSocialMediaInfo.UserID={UserID} and UserTypeID={UserType}");
                    //                    dbSocialMedia = new General().GetAllRowsInDictionary(dtSocialMedia);


                    //dbrows = new General().GetAllRowsInDictionary(dt);

                    //jr = GeneralApisController.ResponseMessage(HttpStatusCode.OK, "User Profile Information!", dbrows);
                    Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                    JSResponse.Add("Status", HttpStatusCode.OK);
                    JSResponse.Add("Message", "User Profile Information!");
                    JSResponse.Add("Data", ModelLst);

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
                return jr;

            }
            catch (Exception ex)
            {

                Dictionary<string, object> JSResponse = new Dictionary<string, object>();
                JSResponse.Add("Status", HttpStatusCode.BadRequest);
                JSResponse.Add("Message", "Error: " + ex.Message);
                JSResponse.Add("Data", DBNull.Value);
                JSResponse.Add("SocialMedia", DBNull.Value);

                var jr = new JsonResult()
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
    }
}