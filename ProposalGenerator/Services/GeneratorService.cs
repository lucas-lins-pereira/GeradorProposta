using ProposalGenerator.Interfaces;
using ProposalGenerator.Models;
using ProposalGenerator.Models.Http;
using System.Net;

namespace ProposalGenerator.Services
{
    public class GeneratorService : IGeneratorService
    {

        public BaseResponse Create(RequestBody request)
        {
            var excelFile = new ExcelFile(request.ExcelFile);

            return new BaseResponse { StatusCode = HttpStatusCode.OK, Message = "Proposta gerada com sucesso.", Content = excelFile };
        }
    }
}
