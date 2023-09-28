using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
namespace Privont
{    
    public class DropDown
    {

        public SelectList GetOrganizationList(string whereclause = "", int selectedvalue = 0)
        {
            DataTable dtEmployee = General.FetchData(@"select OrganizationID,OrganizationTitle from OrganizationInfo	  " + whereclause);
            List<SelectListItem> objsli = new List<SelectListItem>();
            SelectListItem si = new SelectListItem();
            foreach (DataRow dr in dtEmployee.Rows)
            {
                si = new SelectListItem();
                si.Text = dr["OrganizationTitle"].ToString();
                si.Value = dr["OrganizationID"].ToString();
                objsli.Add(si);
            }
            SelectList sl = new SelectList(objsli, "Value", "Text", selectedvalue);
            return sl;
        }
        public SelectList GetPricePoint(string whereclause = "", int selectedvalue = 0)
        {
            DataTable dtEmployee = General.FetchData(@"Select PricePointID,PricePoint from LeadPricePoint	  " + whereclause);
            List<SelectListItem> objsli = new List<SelectListItem>();
            SelectListItem si = new SelectListItem();
            foreach (DataRow dr in dtEmployee.Rows)
            {
                si = new SelectListItem();
                si.Text = dr["PricePoint"].ToString();
                si.Value = dr["PricePointID"].ToString();
                objsli.Add(si);
            }
            SelectList sl = new SelectList(objsli, "Value", "Text", selectedvalue);
            return sl;
        }
        public SelectList GetLenderInfo(string whereclause = "", int selectedvalue = 0)
        {
            DataTable dtEmployee = General.FetchData(@"Select LenderId,(FirstName+' '+LastName)Name from LenderInfo	  " + whereclause);
            List<SelectListItem> objsli = new List<SelectListItem>();
            SelectListItem si = new SelectListItem();
            foreach (DataRow dr in dtEmployee.Rows)
            {
                si = new SelectListItem();
                si.Text = dr["Name"].ToString();
                si.Value = dr["LenderId"].ToString();
                objsli.Add(si);
            }
            SelectList sl = new SelectList(objsli, "Value", "Text", selectedvalue);
            return sl;
        }
        
        public SelectList GetZipCode(string whereclause = "", int selectedvalue = 0)
        {
            DataTable dtEmployee = General.FetchData(@"Select ZipCodeID,ZipCode from ZipCode " + whereclause);
            List<SelectListItem> objsli = new List<SelectListItem>();
            SelectListItem si = new SelectListItem();
            foreach (DataRow dr in dtEmployee.Rows)
            {
                si = new SelectListItem();
                si.Text = dr["ZipCode"].ToString();
                si.Value = dr["ZipCodeID"].ToString();
                objsli.Add(si);
            }
            SelectList sl = new SelectList(objsli, "Value", "Text", selectedvalue);
            return sl;
        }
        public SelectList GetApiLeadType(string whereclause = "", int selectedvalue = 0)
        {
            DataTable dtEmployee = General.FetchData(@"Select ApiTypeID,ApiTypeTitle from APITypeInfo	  " + whereclause);
            List<SelectListItem> objsli = new List<SelectListItem>();
            SelectListItem si = new SelectListItem();
            foreach (DataRow dr in dtEmployee.Rows)
            {
                si = new SelectListItem();
                si.Text = dr["ApiTypeTitle"].ToString();
                si.Value = dr["ApiTypeID"].ToString();
                objsli.Add(si);
            }
            SelectList sl = new SelectList(objsli, "Value", "Text", selectedvalue);
            return sl;
        }
    }
}
