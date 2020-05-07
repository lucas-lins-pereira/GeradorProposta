using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ProposalGenerator.Models
{
    public abstract class File<T>
    {
        protected File(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public List<T> Content { get; protected set; }

        protected abstract void Read(IFormFile formFile);
        protected abstract void Write(List<T> contents);
    }
}
