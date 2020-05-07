using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ProposalGenerator.Interfaces;
using ProposalGenerator.Models.Http;
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
            log.LogInformation("C# HTTP trigger function processed a request.");

            var formData = await req.ReadFormAsync();
            var requestBody = new RequestBody { ExcelFile = formData.Files["Planilha"], TemplateFile = formData.Files["Template"] };

            var result = _generatorService.Create(requestBody);

            return new ObjectResult(result) { StatusCode = (int)result.StatusCode };
        }
    }
}
