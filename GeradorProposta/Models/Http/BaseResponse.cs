using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace ProposalGenerator.Models.Http
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class BaseResponse
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public FileContentResult File { get; set; }
    }
}
