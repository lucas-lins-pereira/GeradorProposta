using ProposalGenerator.Interfaces;
using ProposalGenerator.Models;
using ProposalGenerator.Models.Http;
using System.Collections.Generic;
using System.Net;
using TemplateEngine.Docx;

namespace ProposalGenerator.Services
{
    public class GeneratorService : IGeneratorService
    {

        public BaseResponse Create(RequestBody request)
        {
            var excelFile = new ExcelFile(request.ExcelFile);

            var content = PrepareTemplateContent(excelFile);

            using (var outputDocument = new TemplateProcessor(request.TemplateFile.OpenReadStream())
                .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(content);
                outputDocument.SaveChanges();
            }

            return new BaseResponse { StatusCode = HttpStatusCode.OK, Message = "Proposta gerada com sucesso.", Content = excelFile };
        }

        private Content PrepareTemplateContent(ExcelFile excelFile)
        {
            var listTable = new List<TableContent>();
            var listFieldContent = new List<FieldContent>();
            foreach (var workSheet in excelFile.Content)
            {
                switch (workSheet.Type)
                {
                    case WorkSheetTypeEnum.Table:
                        var tableContent = new TableContent(workSheet.Name);
                        foreach (var row in workSheet.Rows)
                        {
                            var listRowField = new List<FieldContent>();
                            for (int column = 0; column < row.Cells.Count; column++)
                                listRowField.Add(new FieldContent(workSheet.HeaderRow.Cells[column], row.Cells[column]));

                            tableContent.AddRow(listRowField.ToArray());
                        }

                        listTable.Add(tableContent);
                        break;

                    case WorkSheetTypeEnum.Field:
                        foreach (var row in workSheet.Rows)
                            listFieldContent.Add(new FieldContent(row.Cells[0], row.Cells[1]));
                        break;

                    default:
                        break;
                }
            }

            return new Content
            {
                Tables = listTable,
                Fields = listFieldContent
            };
        }
    }
}
