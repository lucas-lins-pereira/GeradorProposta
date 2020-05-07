using Microsoft.AspNetCore.Http;

namespace ProposalGenerator.Models.Http
{
    public class RequestBody
    {
        public IFormFile ExcelFile { get; set; }
        public IFormFile TemplateFile { get; set; }
    }
}
