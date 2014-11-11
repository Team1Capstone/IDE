using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Workspace
{
    public class Project
    {
        public string Directory { get; protected set; }
        public IEnumerable<string> Files { get; protected set; }
        public IEnumerable<string> References { get; protected set; }
    }

}
