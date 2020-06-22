using System.Collections.Generic;

namespace ProposalGenerator.Models
{
    public class Row
    {
        public Row()
        {
            Cells = new List<string>();
        }

        public List<string> Cells { get; private set; }
    }
}
