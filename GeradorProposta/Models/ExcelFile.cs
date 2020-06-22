using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using ProposalGenerator.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProposalGenerator.Models
{
    public class ExcelFile
    {
        public ExcelFile(IFormFile formFile, char separator)
        {
            Read(formFile, separator);
            Name = Path.GetFileNameWithoutExtension(formFile.FileName);
        }

        public KeyValuePair<bool, string> Error { get; private set; }
        public string Name { get; private set; }
        public List<WorkSheet> Content { get; private set; }

        private void Read(IFormFile formFile, char separator)
        {
            var listWorkSheets = new List<WorkSheet>();
            using var stream = formFile.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);
            do
            {
                var errorMessage = ValidateReaderName(reader.Name, separator);
                if (errorMessage != null)
                {
                    Error = new KeyValuePair<bool, string>(true, errorMessage);
                    return;
                }

                var workSheetName = reader.Name.Split(separator);
                var workSheetType = GetWorkSheetType(workSheetName[1]);
                if (workSheetType == default)
                {
                    Error = new KeyValuePair<bool, string>(true, $"Tipo de estrutura inválida. Tipo: {workSheetType} | Planilha: {reader.Name}.");
                    return;
                }

                var workSheet = new WorkSheet(workSheetName[0], workSheetType);
                while (reader.Read())
                {
                    var row = new Row();
                    for (int column = 0; column < reader.FieldCount; column++)
                        row.Cells.Add(reader.GetFormattedValue(column));

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

        private static string ValidateReaderName(string readerName, char separator)
        {
            if (!readerName.Contains(separator))
                return $"Separador não corresponde. Separador: {separator} | Planilha: {readerName}.";

            if (readerName.Count(x => x.Equals(separator)) > 1)
                return $"Separador precisa ser único. Separador: {separator} | Planilha: {readerName}.";

            if (readerName.Trim().EndsWith(separator) || readerName.Trim().StartsWith(separator))
                return $"Nome da planilha não pode terminar ou começar com o separador informado. Separador: {separator} | Planilha: {readerName}.";

            return null;
        }
    }
}
