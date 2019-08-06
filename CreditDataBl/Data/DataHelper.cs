using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public class DataHelper
    {
        public static DataTable ExcelToDataTable(string inputPath, string sheetName)
        {
            if (inputPath.IsXLS())
            {
                string altXLSXPath = Path.Combine(Path.GetDirectoryName(inputPath), $"{ Path.GetFileNameWithoutExtension(inputPath)}.xlsx");
                // convert to xlsx
                var app = new Microsoft.Office.Interop.Excel.Application();
                var wb = app.Workbooks.Open(inputPath);
                wb.SaveAs(Filename: altXLSXPath, FileFormat: Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
                wb.Close();
                app.Quit();
                return ReadFromXLSX(altXLSXPath, sheetName);
            }
            return ReadFromXLSX(inputPath, sheetName);
        }

        private static DataTable ReadFromXLSX(string pathName, string sheetName)
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
