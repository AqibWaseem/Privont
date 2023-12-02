using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrivontAdmin
{
    public class DropDown
    {

        //public SelectList GetZipInfo(string whereclause = "", int selectedvalue = 0)
        //{
        //    DataTable dtEmployee = General.FetchData(@"select * from ZipCode	  " + whereclause);
        //    List<SelectListItem> objsli = new List<SelectListItem>();
        //    SelectListItem si = new SelectListItem();
        //    foreach (DataRow dr in dtEmployee.Rows)
        //    {
        //        si = new SelectListItem();
        //        si.Text = dr["ApiTypeTitle"].ToString();
        //        si.Value = dr["ApiTypeID"].ToString();
        //        objsli.Add(si);
        //    }
        //    SelectList sl = new SelectList(objsli, "Value", "Text", selectedvalue);
        //    return sl;
        //}
        public SelectList GetAPISourceInfo(string whereclause = "", int selectedvalue = 0)
        {
            DataTable dtEmployee = General.FetchData(@"select ApiSource from LeadInfo	  " + whereclause);
            List<SelectListItem> objsli = new List<SelectListItem>();
            SelectListItem si = new SelectListItem();
            foreach (DataRow dr in dtEmployee.Rows)
            {
                si = new SelectListItem();
                si.Text = dr["ApiSource"].ToString();
                si.Value = dr["ApiSource"].ToString();
                objsli.Add(si);
            }
            SelectList sl = new SelectList(objsli, "Value", "Text", selectedvalue);
            return sl;
        }public SelectList GetOrganizationInfo(string whereclause = "", int selectedvalue = 0)
        {
            DataTable dtEmployee = General.FetchData(@"Select OrganizationID,OrganizationTitle from OrganizationInfo	  " + whereclause);
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