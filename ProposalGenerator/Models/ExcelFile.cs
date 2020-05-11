using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;

namespace ProposalGenerator.Models
{
    public class ExcelFile
    {
        public ExcelFile(IFormFile formFile, char separator)
        {
            Read(formFile, separator);
            Name = Path.GetFileNameWithoutExtension(formFile.FileName);
        }

        public string Name { get; private set; }
        public List<WorkSheet> Content { get; protected set; }

        protected void Read(IFormFile formFile, char separator)
        {
            var listWorkSheets = new List<WorkSheet>();
            using var stream = formFile.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);
            do
            {
                var name = reader.Name.Split(separator);
                var type = GetWorkSheetType(name[1]);
                var workSheet = new WorkSheet(name[0], type);
                while (reader.Read())
                {
                    var row = new Row();
                    for (int column = 0; column < reader.FieldCount; column++)
                    {
                        var value = reader.GetValue(column)?.ToString();
                        if (value == null)
                            break;

                        row.Cells.Add(value);
                    }

                    if (row.Cells.Count == 0)
                        continue;

                    if (workSheet.HeaderRow.Cells.Count == 0)
                        workSheet.HeaderRow = row;
                    else
                        workSheet.Rows.Add(row);
                }

                listWorkSheets.Add(workSheet);
            } while (reader.NextResult());

            Content = listWorkSheets;
        }

        private static WorkSheetTypeEnum GetWorkSheetType(string type)
        {
            return (type.ToUpperInvariant()) switch
            {
                "TABELA" => WorkSheetTypeEnum.Table,
                "CAMPO" => WorkSheetTypeEnum.Field,
                _ => default,
            };
        }
    }
}
