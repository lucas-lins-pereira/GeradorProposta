using Microsoft.AspNetCore.Http;
using System;

namespace ProposalGenerator.Models.Http
{
    public class RequestBody
    {
        public RequestBody(IFormFile planilha, IFormFile template, string separadorNomeTipo, string nomeArquivoSaida, string alterarCabecalhoTemplate)
        {
            Planilha = planilha;
            Template = template;
            SeparadorNomeTipo = string.IsNullOrWhiteSpace(separadorNomeTipo) ? '-' : separadorNomeTipo.Trim().ToCharArray()[0];
            NomeArquivoSaida = nomeArquivoSaida.Trim().Replace(" ", "_") ?? DateTime.Now.ToString("yyyyMMddhhmmss");
            SetAlterarCabecalhoTemplate(alterarCabecalhoTemplate);
        }

        public IFormFile Planilha { get; private set; }
        public IFormFile Template { get; private set; }
        public char SeparadorNomeTipo { get; private set; }
        public string NomeArquivoSaida { get; private set; }
        public bool AlterarCabecalhoTemplate { get; private set; }

        private void SetAlterarCabecalhoTemplate(string alterarCabecalhoTemplate)
        {
            if (string.IsNullOrWhiteSpace(alterarCabecalhoTemplate))
                return;

            var value = alterarCabecalhoTemplate.Trim().ToUpperInvariant();
            if (value.StartsWith("N") || value.StartsWith("0"))
                return;

            AlterarCabecalhoTemplate = true;
        }
    }
}
