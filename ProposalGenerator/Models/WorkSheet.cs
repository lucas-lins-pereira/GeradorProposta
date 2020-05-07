using System.Collections.Generic;

namespace ProposalGenerator.Models
{
    public class WorkSheet
    {
        public WorkSheet(string name)
        {
            Name = name;
            HeaderRow = new Row();
            Rows = new List<Row>();
        }

        public string Name { get; private set; }
        public Row HeaderRow { get; set; }
        public List<Row> Rows { get; private set; }
    }
}
