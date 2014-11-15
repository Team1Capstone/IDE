using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Workspace
{
    /// <summary>
    /// This class deals with the file organization of the workspace, the solution file, and any project files in that solution
    /// </summary>
    public class Workspace
    {
        // The root directory of the workspace
        public string Directory { get; protected set; }
        public IEnumerable<string> Solutions { get; protected set; }

        // New Solution
        // Existing Solution
        // Remove Solution
        // Delete Solution

        /// <summary>
        /// Packages entire workspace into a ZIP file
        /// </summary>
        public void Package()
        {

        }
    }
}
