using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csv2sql
{
    public class SqlServerConverter
    {
        public SqlServerConverter(IEnumerable<IEnumerable<object>> data, Options options)
        {
            Options = options;
            Items = new List<IEnumerable<object>>(data);
            if (Items.Count <= 0) throw new ArgumentException("At least one row of data needs to be provided");
        }

        public List<IEnumerable<object>> Items { get; private set; }
        public Options Options { get; private set; }


        private List<string> GetColumnNames()
        {
            var colNames = new List<string>();
            var first = Items[0].ToList();
            if (!Options.FirstRowIsHeader)
            {
                for (var i = 0; i < first.Count; i++)
                    colNames.Add(string.Format("[column_{0}]", Convert.ToString(i + 1).PadLeft(3, '0')));
                return colNames;
            }
            foreach(var col in first)
                colNames.Add(string.Format("[{0}]", col));
            return colNames;
        }

        private string GetTableName()
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Options.Schema)) parts.Add(string.Format("[{0}]", Options.Schema));
            parts.Add(string.Format("[{0}]", Options.TableName));
            return string.Join(".", parts);
        }

        private string GetValues(IEnumerable<object> values)
        {
            var formattedValues = new List<string>();
            foreach(var val in values)
            {
                var sVal = Convert.ToString(val).Replace("\n", "").Replace("\r", "");
                formattedValues.Add(string.Format("'{0}'", sVal));
            }
            return string.Join(", ", formattedValues);
        }

        public string CreateTableScript()
        {
            var tn = GetTableName();
            var cols = GetColumnNames();
            var sw = new StringWriter();
            sw.WriteLine("IF OBJECT_ID('{0}', 'U') IS NOT NULL", tn);
            sw.WriteLine("BEGIN");
            sw.WriteLine("  DROP TABLE {0}", tn);
            sw.WriteLine("END");
            sw.WriteLine("GO");
            sw.WriteLine("CREATE TABLE ({0})", string.Join(",",cols.Select(i =>  string.Format("{0} varchar(1000) NULL", i))));
            sw.WriteLine("GO");
            return sw.ToString();
        }

        public string ToSqlInsert()
        {
            var sw = new StringWriter();
            var tn = GetTableName();
            var start = 0;
            if (Options.FirstRowIsHeader) start++;
            for (var i = start; i < Items.Count; i++)
            {
                sw.Write("INSERT INTO {0} VALUES (", tn);
                sw.Write(GetValues(Items[i]));
                sw.Write(");");
                sw.WriteLine();
            }
            return sw.ToString();
        }
        public override string ToString()
        {
            var sw = new StringWriter();
            sw.WriteLine();
            sw.WriteLine(CreateTableScript());
            sw.WriteLine();
            sw.WriteLine(ToSqlInsert());
            sw.WriteLine();
            return sw.ToString();
        }

    }
}
