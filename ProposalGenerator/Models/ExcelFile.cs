using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProposalGenerator.Models
{
    public class ExcelFile : File<WorkSheet>
    {
        public ExcelFile(IFormFile formFile) : base(Path.GetFileNameWithoutExtension(formFile.FileName))
        {
            Read(formFile);
        }

        protected override void Read(IFormFile formFile)
        {
            var listWorkSheets = new List<WorkSheet>();
            using var stream = formFile.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);
            do
            {
                var workSheet = new WorkSheet(reader.Name);
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

        protected override void Write(List<WorkSheet> contents)
        {
            throw new NotImplementedException();
        }
    }
}
