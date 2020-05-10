using ProposalGenerator.Models.Http;

namespace ProposalGenerator.Interfaces
{
    public interface IGeneratorService
    {
        byte[] Create(RequestBody request);
    }
}
