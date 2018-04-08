using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csv2sql
{
    class Program
    {
        static void Main(string[] args)
        {
            runScript();
            split();
            var switches = new ConsoleSwitches(args);
            Console.WriteLine("Starting...");
            Exec(switches);
            Console.WriteLine("Completed");
        }

        private static void Exec(ConsoleSwitches switches)
        {
            var schema = string.Empty;//switches["/schema"];
            var table = switches["/table"];
            var source = switches["/source"];
            var destination = switches["/destination"];
            var content = File.ReadAllText(source);
            var options = new Options() { Schema = schema, TableName = table, Separator = ",", FirstRowIsHeader = true };
            var data = (new Parser(content, options)).ToData();
            var sqlText = (new SqlServerConverter(data, options)).ToString();
            File.WriteAllText(destination, sqlText);
        }

        private static void runScript()
        {
            var dir = new DirectoryInfo(@"C:\Data");
            var files = dir.GetFiles("sqlpart*.sql");
            foreach (var file in files)
            {
                Console.WriteLine("Running {0}", file.Name);
                var args = string.Format("-S mleyca.database.windows.net -d ML-Data -i {0} -U eycaml -P 3yc@Machin3L3@rning!", file.FullName);
                var p = Process.Start(new ProcessStartInfo()
                {
                    WorkingDirectory = @"C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\130\Tools\Binn",
                    Arguments = args,
                    FileName = "sqlcmd.exe"
                });
                p.WaitForExit();
            }
        }

        private static void split()
        {
            var data = File.ReadAllLines(@"C:\Data\data-only.sql");
            var fileCount = 1;
            var chunk = 20000;
            var count = 0;
            var newFile = new StringBuilder();
            foreach (var line in data)
            {
                newFile.Append(line);
                count++;
                if (count >= chunk)
                {
                    File.WriteAllText(string.Format(@"C:\Data\sqlpart{0}.sql", fileCount.ToString().PadLeft(3, '0')), newFile.ToString());
                    newFile.Clear();
                    fileCount++;
                    count = 0;
                }
            }
        }
    }
}
