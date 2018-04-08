using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csv2sql
{
    public class Options
    {
        public Options()
        {
            ColumnTypes = new List<SqlMacroTypes>();
        }
        public string TableName { get; set; }
        public string Schema { get; set; }
        public bool FirstRowIsHeader { get; set; }
        public string Separator { get; set; }
        public List<SqlMacroTypes> ColumnTypes { get; set; }
    }
}
