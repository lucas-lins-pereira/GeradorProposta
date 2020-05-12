﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProposalGenerator.Interfaces;
using ProposalGenerator.Models;
using ProposalGenerator.Models.Http;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TemplateEngine.Docx;

namespace ProposalGenerator.Services
{
    public class GeneratorService : IGeneratorService
    {
        public BaseResponse Create(RequestBody request)
        {
            var excelFile = new ExcelFile(request.Planilha, request.SeparadorNomeTipo);
            if (excelFile.Error.Key)
                return new BaseResponse { StatusCode = HttpStatusCode.BadRequest, Message = excelFile.Error.Value };

            var proposalContent = PrepareProposalContent(excelFile, request.AlterarCabecalhoTemplate);
            var bytes = GetProposalFileBytes(request.Template, proposalContent).Result;

            return new BaseResponse
            {
                StatusCode = HttpStatusCode.OK,
                File = new FileContentResult(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                {
                    FileDownloadName = $"{request.NomeArquivoSaida}.docx",
                }
            };
        }

        private static async Task<byte[]> GetProposalFileBytes(IFormFile templateFile, Content proposalContent)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                await templateFile.CopyToAsync(memoryStream);

                using (var templateProcessor = new TemplateProcessor(memoryStream).SetRemoveContentControls(true))
                {
                    templateProcessor.FillContent(proposalContent);
                    templateProcessor.SaveChanges();
                }

                bytes = memoryStream.ToArray();
            }

            return bytes;
        }

        private static Content PrepareProposalContent(ExcelFile excelFile, bool changeTemplateHeader)
        {
            var listTable = new List<TableContent>();
            var listFieldContent = new List<FieldContent>();
            foreach (var workSheet in excelFile.Content)
            {
                switch (workSheet.Type)
                {
                    case WorkSheetTypeEnum.Table:
                        var tableContent = new TableContent(workSheet.Name);
                        if (tableContent.Rows == null && changeTemplateHeader)
                        {
                            var arrayHeaderRow = new FieldContent[workSheet.HeaderRow.Cells.Count];
                            for (int column = 0; column < workSheet.HeaderRow.Cells.Count; column++)
                                arrayHeaderRow[column] = new FieldContent(workSheet.HeaderRow.Cells[column], workSheet.HeaderRow.Cells[column]);

                            tableContent.AddRow(arrayHeaderRow);
                        }

                        foreach (var row in workSheet.Rows)
                        {
                            var arrayRowField = new FieldContent[row.Cells.Count];
                            for (int column = 0; column < row.Cells.Count; column++)
                                arrayRowField[column] = new FieldContent(workSheet.HeaderRow.Cells[column], row.Cells[column]);

                            tableContent.AddRow(arrayRowField);
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
