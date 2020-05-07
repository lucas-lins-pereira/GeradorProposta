using ProposalGenerator.Models.Http;

namespace ProposalGenerator.Interfaces
{
    public interface IGeneratorService
    {
        BaseResponse Create(RequestBody request);
    }
}
