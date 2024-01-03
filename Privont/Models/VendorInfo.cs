﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Privont.Models
{
    public class VendorInfo:RealEstateAgentInfo
    {
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string PhoneNo { get; set; }
        public string RegistrationNo { get; set; }
        public int ZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string OfficeNo { get; set; }
        public string StreetName { get; set; }
        public string StreetNo { get; set; }
        public string Website { get; set; }
        public string EmailAddress { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public string Remarks { get; set; }
        public bool Inactive { get; set; }
        public DataTable GetAllRecord(string WhereClause = "")
        {
            DataTable dataTable = new DataTable();
            dataTable = General.FetchData($@"Select VendorInfo.*,ZipCode.ZipCode from VendorInfo
inner join ZipCode on VendorInfo.ZipCodeID = ZipCode.ZipCodeID

" + WhereClause);
            return dataTable;
        }
        public int InsertRecord(VendorInfo obj)
        {
            int VendorID = 0;
            string Query = $@"INSERT INTO [dbo].[VendorInfo]
           ([VendorName]
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
)
     VALUES
           ('{obj.VendorName}'
           ,'{obj.PhoneNo}'
           ,'{obj.RegistrationNo}'
           ,{obj.ZipCodeID}
,'{obj.OfficeNo}'
,'{obj.StreetNo}'
,'{obj.StreetName}'
,'{obj.Website}'
,'{obj.EmailAddress}'
,'{obj.Contact1}'
,'{obj.Contact2}'
,'{obj.Remarks}'
,{(obj.Inactive == true ? 1 : 0)}
)";
            try
            {
                Query = Query + $@"SELECT SCOPE_IDENTITY() as VendorID";
                DataTable dt = General.FetchData(Query);
                obj.VendorID = int.Parse(dt.Rows[0]["VendorID"].ToString());
                return obj.VendorID;
            }
            catch (Exception ex)
            {
                return VendorID;
            }
        }
        public int UpdateRecord(VendorInfo obj)
        {
            string Query = $@"UPDATE [dbo].[VendorInfo]
   SET [VendorName] = '{obj.VendorName}'
      ,[RegistrationNo] = '{obj.RegistrationNo}'
      ,[ZIPCodeID] = {obj.ZipCodeID}
      ,[OfficeNo] = '{obj.OfficeNo}'
      ,[StreetName] = '{obj.StreetName}'
      ,[StreetNo] = '{obj.StreetNo}'
      ,[Website] = '{obj.Website}'
      ,[EmailAddress] = '{obj.EmailAddress}'
      ,[Contact1] = '{obj.Contact1}'
      ,[Contact2] = '{obj.Contact2}'
      ,[Remarks] = '{obj.Remarks}'
      ,[Inactive] = {(obj.Inactive == true ? 1 : 0)}
 WHERE VendorID=" + obj.VendorID;
            try
            {
                General.ExecuteNonQuery(Query);
                return obj.VendorID;
            }
            catch (Exception ex)
            {
                return obj.VendorID;
            }
        }


        public int UpdateProfileRecord(VendorInfo obj)
        {
            string Query = $@"UPDATE [dbo].[VendorInfo]
   SET [VendorName] = '{obj.VendorName}'
      ,[StreetName] = '{obj.StreetName}'
      ,[StreetNo] = '{obj.StreetNo}'
      ,[Website] = '{obj.Website}'
      ,[EmailAddress] = '{obj.EmailAddress}'
      ,[Remarks] = '{obj.Remarks}'
      ,[FirstName] = '{obj.FirstName}'
      ,[LastName] = '{obj.LastName}'
 WHERE VendorID=" + obj.VendorID;
            try
            {
                General.ExecuteNonQuery(Query);
                return obj.VendorID;
            }
            catch (Exception ex)
            {
                return obj.VendorID;
            }
        }
    }
}