using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ProposalGenerator.Interfaces;
using ProposalGenerator.Models.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ProposalGenerator
{
    public class ProposalGeneratorFunctions
    {
        private readonly IGeneratorService _generatorService;

        public ProposalGeneratorFunctions(IGeneratorService generatorService)
        {
            _generatorService = generatorService;
        }

        [FunctionName(nameof(ProposalGenerator))]
        public async Task<IActionResult> ProposalGenerator(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/proposal")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"Função {nameof(ProposalGenerator)} iniciada.");

            try
            {
                var formData = await req.ReadFormAsync();
                var requestBody = new RequestBody(
                    planilha: formData.Files["Planilha"],
                    template: formData.Files["Template"],
                    separadorNomeTipo: formData["SeparadorNomeTipo"],
                    nomeArquivoSaida: formData["NomeArquivoSaida"],
                    alterarCabecalhoTemplate: formData["AlterarCabecalhoTemplate"]
                );

                log.LogInformation($"Validar Request.");
                var errorMessage = ValidateRequest(requestBody);
                if (errorMessage != null)
                    return new ObjectResult(new BaseResponse { Message = errorMessage }) { StatusCode = (int)HttpStatusCode.BadRequest };

                var result = _generatorService.Create(requestBody);
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
                log.LogInformation($"Função {nameof(ProposalGenerator)} finalizada.");
            }
        }

        private static string ValidateRequest(RequestBody request)
        {
            if (request.Planilha == null)
                return $"Campo {nameof(RequestBody.Planilha)} obrigatório.";

            if (request.Template == null)
                return $"Campo {nameof(RequestBody.Template)} obrigatório.";

            return null;
        }
    }
}
