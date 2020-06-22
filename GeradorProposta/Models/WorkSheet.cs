using System.Collections.Generic;

namespace ProposalGenerator.Models
{
    public class WorkSheet
    {
        public WorkSheet(string name, WorkSheetTypeEnum type)
        {
            Name = name;
            HeaderRow = new Row();
            Rows = new List<Row>();
            Type = type;
        }

        public string Name { get; private set; }
        public Row HeaderRow { get; set; }
        public List<Row> Rows { get; private set; }
        public WorkSheetTypeEnum Type { get; private set; }
    }
}
