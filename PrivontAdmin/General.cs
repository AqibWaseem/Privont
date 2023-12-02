using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PrivontAdmin
{
    public class General
    {

        public static int UserID
        {
            get
            {
                try
                {
                    if (System.Web.HttpContext.Current.Request.Cookies["UserID"] != null)
                        return int.Parse(System.Web.HttpContext.Current.Request.Cookies["UserID"].Value.ToString());
                    else
                        return 0;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                return;
            }
        }
        public static int UserType
        {
            get
            {
                try
                {
                    if (System.Web.HttpContext.Current.Request.Cookies["UserType"] != null)
                    {
                        return int.Parse(System.Web.HttpContext.Current.Request.Cookies["UserType"].Value.ToString());
                    }
                    else
                        return 0;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                return;
            }
        }
        public static int LeadExpiryTime
        {
            get
            {
                try
                {
                    return int.Parse(General.FetchData($@"Select ExpiryTime from LeadExpiryTime").Rows[0]["ExpiryTime"].ToString());
                }
                catch
                {
                    return 0;
                }
            }
            set
            {

            }
        }

        public static string key = "HisabdaarPrivont";
        public static string ConnectionString { get; set; }
        public static string CompanyInformation { get { return "Privont"; } }
        public static DataTable FetchData(string query)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            ad.SelectCommand.CommandTimeout = 0;
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
        public static void ExecuteNonQuery(string query)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            if (items != null)
            {
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
            }
            return dataTable;
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        if (dr[column.ColumnName] != DBNull.Value)
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        //pro.SetValue(obj, dr[column.ColumnName].ToString(), null);
                        else
                            continue;
                }
            }
            return obj;
        }
        public List<Dictionary<string, object>> GetAllRowsInDictionary(DataTable dtData)
        {
            List<Dictionary<string, object>>
            lstRows = new List<Dictionary<string, object>>();
            Dictionary<string, object> dictRow = null;

            foreach (DataRow dr in dtData.Rows)
            {
                dictRow = new Dictionary<string, object>();
                foreach (DataColumn col in dtData.Columns)
                {
                    dictRow.Add(col.ColumnName, dr[col]);
                }
                lstRows.Add(dictRow);
            }
            return lstRows;

        }
        public static string Encrypt(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16]; // IV (Initialization Vector) should be unique and unpredictable, but it's a constant for simplicity here.

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16]; // IV (Initialization Vector) should match the one used for encryption.

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        public static string FetchDataFromAPIs(string APIsPath, string Auth)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);
            //client.BaseAddress = new Uri(BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", Auth);
            client.Timeout = TimeSpan.FromMilliseconds(60000);
            ///System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            StringContent jcontent = new StringContent("", Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.GetAsync(APIsPath).Result;
            string Jsonstring = "";
            if (response.IsSuccessStatusCode)
            {
                Jsonstring = response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                Jsonstring = "Error";
            }
            return Jsonstring;
        }
        public static string GetAPIAuthKey(int UserID, int APITypeSource)
        {
            string KEY = "";
            DataTable dt = General.FetchData($@"select * from APIConfigInfo where RealEstateID={UserID} and TypeID=" + APITypeSource);
            if (dt.Rows.Count > 0)
            {
                KEY = dt.Rows[0]["APIConfig"].ToString();
                return KEY;
            }
            return "";
        }
        public enum LogTypes
        {
            New,
            Edit,
            Delete,
            Logout,
            Login,
            APICall,
            ChangePassword
        }
        public enum LogSource
        {
            FollowUpBoss,
            Zillow
        }
        public static string PostFromAPIs(string APIsPath, string Auth)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);
            //client.BaseAddress = new Uri(BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", Auth);
            client.Timeout = TimeSpan.FromMilliseconds(60000);
            ///System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            StringContent jcontent = new StringContent("", Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.GetAsync(APIsPath).Result;
            string Jsonstring = "";
            if (response.IsSuccessStatusCode)
            {
                Jsonstring = response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                Jsonstring = "Error";
            }
            return Jsonstring;
        }

    }
}