using Square.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TrueDialog.Model;

namespace Privont.Models
{
    public class TransactionDetails
    {
        public int TransactionID { get; set; }
        public string TransactionNo { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string RefNo { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; }
        public int UserID { get; set; }
        public int UserType { get; set; }
        public int TransactionActionID { get; set; }
        public string APITransactionID { get; set; }

        public string GenerateTransactionNo()
        {
            string Query = "";
            Query = $@"SELECT right('0000' + convert(nvarchar(150), count(TransactionID)+1), 2) AS 'TransactionNo' from TransactionDetails";
            string str = "";
            if (string.IsNullOrEmpty(Query))
            {
                str = DateTime.Now.ToString("yyMMdd") + 0001;
            }
            else
            {
                str = DateTime.Now.ToString("yyMMdd") + General.FetchData(Query).Rows[0]["TransactionNo"].ToString();
            }
            return str;
        }
        public int InsertRecords()
        {
            int TransactionID = 0;

            try
            {
                string Query = $@"INSERT INTO [dbo].[TransactionDetails]
           ([TransactionNo]
           ,[TransactionDateTime]
           ,[RefNo]
           ,[Amount]
           ,[Status]
           ,[Remarks]
           ,[UserID]
           ,[UserType]
           ,[TransactionActionID]
           ,[APITransactionID])
     VALUES
           ('{GenerateTransactionNo()}'
           ,GETDATE()
           ,'{RefNo}'
           ,{Amount}
           ,{Status}
           ,'{Remarks}'
           ,'{UserID}'
           ,'{UserType}'
           ,'{TransactionActionID}'
           ,'{APITransactionID}')";
                Query = Query + $@" SELECT SCOPE_IDENTITY() as TransactionID";
                DataTable dt = General.FetchData(Query);
                TransactionID = int.Parse(dt.Rows[0]["TransactionID"].ToString());
                return TransactionID;
            }
            catch (Exception ex)
            {
                return TransactionID;
            }
        }
    }
}