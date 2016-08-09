using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Flag.CommandLine
{
    using Compile;
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var path = Path.GetFullPath(args[0]);
                var directory = Directory.GetParent(path).FullName;
                var name = Path.GetFileNameWithoutExtension(path);
                var parts = new Regex("\\w+").Matches(name).Cast<Match>().Select(m => m.Value).ToArray();
                var ns = string.Join(".", parts.Take(parts.Length - 1));
                var className = parts.Last();
                if (string.IsNullOrEmpty(ns))
                    ns = "Flag";
                if (string.IsNullOrEmpty(className))
                    className = "Templates";

                var text = File.ReadAllText(path);

                var sb = new StringBuilder();

                using (var sw = new StringWriter(sb))
                {
                    switch (Path.GetExtension(path))
                    {
                        case ".flag":
                            new Compile.Javascript.TemplateCompiler(ns, className).Compile("execute", text, sw);
                            break;
                        case ".bunting":
                            new BuntingCompiler(new Compile.Javascript.TemplateCompiler(ns, className)).Compile(text, sw);
                            break;
                        default:
                            throw new Exception("Unrecognized file type: " + Path.GetExtension(path));
                    }
                }

                File.WriteAllText(Path.Combine(directory, string.Format("{0}.{1}.js", ns, className)), sb.ToString());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
