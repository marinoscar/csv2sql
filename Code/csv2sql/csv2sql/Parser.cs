using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csv2sql
{
    public class Parser
    {
        private string _content;
        private Options _options;

        public Parser(string content, Options options)
        {
            _options = options;
            _content = content;
        }

        public IEnumerable<IEnumerable<string>> ToData()
        {
            var data = new List<List<string>>();
            var lines = _content.Split("\n".ToCharArray());
            foreach(var line in lines)
            {
                data.Add(new List<string>(line.Split(_options.Separator.ToCharArray())));
            }
            return data;
        }
    }
}
