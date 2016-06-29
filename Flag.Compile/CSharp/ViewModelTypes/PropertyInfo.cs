using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public class PropertyInfo
    {
        public PropertyInfo(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public string Type { get; private set; }
        public string Name { get; private set; }
    }
}
