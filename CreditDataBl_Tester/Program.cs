using CreditDataBl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl_Tester
{
    class Program
    {
        const string ConnectionStr = @"Data Source=.;Initial Catalog=smach_tests;Integrated Security=True;";
        //const string ConnectionStr = @"Server=nl1-wsq1.a2hosting.com;DATABASE=websimpl_websimpl_db;User Id=websimpl_webs_adm;Password=webs_admgjdshgfjdsg574uhhgg!]]safsafsas;";
        //const string ConnectionStr = @Server=.\SQLEXPRESS;DATABASE=DbMusic;Integrated Security=True;";
        static void Main(string[] args)
        {
            //SqlMigrationHandler<CreditCardTransaction> localmig = new SqlMigrationHandler<CreditCardTransaction>(ConnectionStr);
            //localmig.MigrateTable();

            var ext = new List<string> { ".xls" };
            var dirPath = @"C:\Users\smachew.WISMAIN\Desktop\tmp\Transactions";
            var myFiles = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories).Where(s => ext.Contains(Path.GetExtension(s)));

            foreach (var excelPath in myFiles)
            {
                HandleXls(excelPath);
                Console.WriteLine($"Done Proccessing { Path.GetFileNameWithoutExtension(excelPath)}");
            }

            Console.WriteLine("End !");
            Console.ReadLine();
        }

        private static void HandleXls(string excelPath)
        {
            var mapper = new DataNamesMapper<CreditCardTransaction>();
            var dt = ExcelToDataTable(excelPath, Path.GetFileNameWithoutExtension(excelPath));
            var excelTransactions = mapper.Map(dt).ToList();
            var dbTransactions = new SqlEntityProfileManager<CreditCardTransaction>(ConnectionStr);
            
            foreach (var excelTransaction in excelTransactions)
            {
                if (excelTransaction.Valid)
                {
                    if (excelTransaction.TransactionDate.Date == DateTime.MinValue.Date)
                    {
                        excelTransaction.TransactionDate = excelTransaction.DebitDate;
                        excelTransaction.TransactionDetails += $" [TransactionDate from DebitDate]";
                    }
                    if (excelTransaction.DebitDate.Date == DateTime.MinValue.Date)
                    {
                        excelTransaction.DebitDate = excelTransaction.TransactionDate;
                        excelTransaction.TransactionDetails += $" [DebitDate from TransactionDate]";
                    }
                    dbTransactions.Insert(excelTransaction);
                }
            }
        }

        public static DataTable ExcelToDataTable(string inputXlsPath, string sheetName)
        {
            string altXLSXPath = Path.Combine(Path.GetDirectoryName(inputXlsPath), $"{ Path.GetFileNameWithoutExtension(inputXlsPath)}.xlsx");
            // convert to xlsx
            if (!File.Exists(altXLSXPath))
            {
                var app = new Microsoft.Office.Interop.Excel.Application();
                var wb = app.Workbooks.Open(inputXlsPath);
                wb.SaveAs(Filename: altXLSXPath, FileFormat: Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
                wb.Close();
                app.Quit();
            }
            return ReadFromExcel(altXLSXPath, sheetName);
        }
        private static DataTable ReadFromExcel(string pathName, string sheetName)
        {
            DataTable tbContainer = new DataTable();
            string strConn = string.Empty;
            if (string.IsNullOrEmpty(sheetName)) { sheetName = "Sheet1"; }
            FileInfo file = new FileInfo(pathName);
            if (!file.Exists) { throw new Exception("Error, file doesn't exists!"); }
            string extension = file.Extension;
            switch (extension)
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";Extended Properties='Excel 12.0;HDR=Yes;'";
                    break;
                default:
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;'";
                    break;
            }
            OleDbConnection cnnxls = new OleDbConnection(strConn);
            OleDbDataAdapter oda = new OleDbDataAdapter(string.Format("select * from [{0}$]", sheetName), cnnxls);
            DataSet ds = new DataSet();
            oda.Fill(tbContainer);
            return tbContainer;
        }

    }
}
