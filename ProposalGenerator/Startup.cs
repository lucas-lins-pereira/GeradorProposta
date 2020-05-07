using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ProposalGenerator.Interfaces;
using ProposalGenerator.Services;
using System.Text;

[assembly: FunctionsStartup(typeof(ProposalGenerator.Startup))]
namespace ProposalGenerator
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            builder.Services.AddTransient<IGeneratorService, GeneratorService>();
        }
    }
}
