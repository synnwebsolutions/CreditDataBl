using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public class ExcelHelper<TEntity> where TEntity : class, new()
    {
        List<string> validFiles = new List<string> { ".xls" , ".xlsx" };

        public IEnumerable<TEntity> MapDirectoryEntities(string directoryPath)
        {
            List<TEntity> lst = new List<TEntity>();
            if (!string.IsNullOrEmpty(directoryPath))
            {
                var myFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories).Where(s => validFiles.Contains(Path.GetExtension(s)));

                foreach (var excelPath in myFiles)
                {
                    List<TEntity> entities = ReadEntitiesFromFile(excelPath);
                    lst.AddRange(entities);
                }
            }
            return lst;
        }
        public IEnumerable<TEntity> MapFileEntities(string filePath)
        {
            List<TEntity> lst = new List<TEntity>();
            if (!string.IsNullOrEmpty(filePath))
            {
                List<TEntity> entities = ReadEntitiesFromFile(filePath);
                lst.AddRange(entities);
            }
            return lst;
        }

        private List<TEntity> ReadEntitiesFromFile(string inputFile, string sheetName = null)
        {
            if (string.IsNullOrEmpty(sheetName))
                sheetName = Path.GetFileNameWithoutExtension(inputFile);

            DataTable table = DataHelper.ExcelToDataTable(inputFile, sheetName);
            return MapEntities(table).ToList();
        }

        public IEnumerable<TEntity> MapEntities(DataTable table)
        {
            DataNamesMapper<TEntity> mapper = new DataNamesMapper<TEntity>();
            List<TEntity> entities = mapper.Map(table).ToList();

            return entities;
        }
    }
}
