using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ProposalGenerator.Interfaces;
using ProposalGenerator.Models.Http;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace ProposalGenerator
{
    public class Functions
    {
        private readonly IGeneratorService _generatorService;

        public Functions(IGeneratorService generatorService)
        {
            _generatorService = generatorService;
        }

        [FunctionName(nameof(Gerar))]
        public async Task<IActionResult> Gerar(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/gerar")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"Função {nameof(Gerar)} iniciada.");

            try
            {
                var formData = await req.ReadFormAsync();
                var body = new RequestBody(
                    planilha: formData.Files["Planilha"],
                    template: formData.Files["Template"],
                    separadorNomeTipo: formData["SeparadorNomeTipo"],
                    nomeArquivoSaida: formData["NomeArquivoSaida"],
                    alterarCabecalhoTemplate: formData["AlterarCabecalhoTemplate"]
                );

                log.LogInformation($"Validar Request.");
                var errorMessage = ValidateRequest(body);
                if (errorMessage != null)
                    return new ObjectResult(new BaseResponse { Message = errorMessage }) { StatusCode = (int)HttpStatusCode.BadRequest };

                var result = _generatorService.Create(body);
                if (result.File != null)
                {
                    log.LogInformation($"Retornar arquivo gerado.");
                    return result.File;
                }

                return new ObjectResult(result) { StatusCode = (int)result.StatusCode };
            }
            catch (Exception ex)
            {
                var erro = new BaseResponse { StatusCode = HttpStatusCode.InternalServerError, Message = $"Ocorreu um erro inesperado. Exceção: {ex.Message}. {ex.StackTrace}" };
                log.LogError(erro.Message);
                return new ObjectResult(erro) { StatusCode = (int)erro.StatusCode };
            }
            finally
            {
                log.LogInformation($"Função {nameof(Gerar)} finalizada.");
            }
        }

        private static string ValidateRequest(RequestBody request)
        {
            if (request.Planilha == null)
                return $"Campo {nameof(RequestBody.Planilha)} obrigatório.";

            if (request.Template == null)
                return $"Campo {nameof(RequestBody.Template)} obrigatório.";

            var planilhaExtensions = new[] { ".xlsx", ".xls", ".csv" };
            var templateExtensions = new[] { ".docx", ".doc" };

            var fileExtension = Path.GetExtension(request.Planilha.FileName).ToLowerInvariant();
            if (!planilhaExtensions.Contains(fileExtension))
                return $"Campo {nameof(RequestBody.Planilha)} inválido.";

            fileExtension = Path.GetExtension(request.Template.FileName).ToLowerInvariant();
            if (!templateExtensions.Contains(fileExtension))
                return $"Campo {nameof(RequestBody.Template)} inválido.";

            return null;
        }
    }
}
