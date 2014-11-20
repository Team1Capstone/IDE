using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Runtime.Serialization;

using Microsoft.CodeAnalysis;

namespace Core.Workspace
{
    /// <summary>
    /// This class deals with the file organization of the workspace, the solution file, and any project files in that solution
    /// </summary>
    [DataContract(Name = "Workspace", Namespace = "http://davenport.edu")]
    public class Workspace
    {
        [DataMember]
        public static string UserDirectory;

        [DataMember]
        public static string UserProjectsDirectory;

        [DataMember]
        public static string UserWorkspaceDirectory;

        [DataMember]
        public static string GACDirectory;

        [DataMember]
        public List<SolutionId> solutions;

        static Workspace()
        {
            // Easy way to determine the folder of the GAC
            GACDirectory = new FileInfo(typeof(object).Assembly.Location).DirectoryName;

            // Assign values for user directories
            UserDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\IDE";
            UserProjectsDirectory = UserDirectory + @"\Projects";
            UserWorkspaceDirectory = UserDirectory + @"\Workspace";

            // Verify main user directory
            if (!Directory.Exists(UserDirectory))
            {
                Directory.CreateDirectory(UserDirectory);                
            }

            // Verify "Projects" directory
            if (!Directory.Exists(UserProjectsDirectory))
            {
                Directory.CreateDirectory(UserProjectsDirectory);
            }

            // Verify "Workspaces" directory
            if (!Directory.Exists(UserWorkspaceDirectory))
            {
                Directory.CreateDirectory(UserWorkspaceDirectory);
            }
        }

        public Workspace()
        {
            solutions = new List<SolutionId>();

            
            CurrentSolution = new Solution(Guid.NewGuid(), string.Empty);
        }

        /// <summary>
        /// The current workspace being used
        /// </summary>
        /// <returns></returns>
        [IgnoreDataMember]
        public Workspace CurrentWorkspace { get; internal set; }

        /// <summary>
        /// The current solution that is being worked on
        /// </summary>
        /// <returns></returns>
        [IgnoreDataMember]
        public Solution CurrentSolution { get; internal set; }

        /// <summary>
        /// Creates and adds an empty solution to the workspace
        /// </summary>
        /// <param name="name"></param>
        public Solution AddNewSolution(string name)
        {
            var solution = new Solution(name);

            // Add identifier to list
            solutions.Add(solution.solutionId);

            solution.WriteTo();

            return solution;
        }

        /// <summary>
        /// Creates and adds a new solution to the workspace. A project of the same name is created in the solution
        /// </summary>
        /// <param name="name"></param>
        /// <param name="kind"></param>
        public Solution AddNewSolution(string name, OutputKind kind)
        {
            var solution = new Solution(name, kind);

            // Add identifier to list
            solutions.Add(solution.solutionId);

            // Write solution file
            solution.WriteTo();

            return solution;
        }

        /// <summary>
        /// Adds an existing solution to the workspace
        /// </summary>
        /// <param name="path"></param>
        public void AddExistingSolution(string path)
        {
            // TODO: Deserialize the XML file into a Solution object
        }

        /// <summary>
        /// Open a solution from the workspace
        /// </summary>
        /// <param name="Id"></param>
        public void OpenSolution(Guid Id)
        {

        }

        /// <summary>
        /// Remove a solution from the workspace
        /// </summary>
        /// <param name="Id"></param>
        public void RemoveSolution(Guid Id)
        {
            // This only removes the solutionId from the workplace
        }

        /// <summary>
        /// Delete a solution and remove a solution from the workspace
        /// </summary>
        /// <param name="Id"></param>
        public void DeleteSolution(Guid Id)
        {

        }

        /// <summary>
        /// Packages entire workspace into a ZIP file
        /// </summary>
        public void Package()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Saves a workspace file to a specified path
        /// </summary>
        /// <param name="file">The name of the name being written to</param>
        public void WriteTo(string file)
        {
            var fs = new FileStream(Workspace.UserWorkspaceDirectory + @"\" + file, FileMode.Create);
            var settings = new XmlWriterSettings()
            {
                Indent = true
            };

            var writer = XmlWriter.Create(fs, settings);
            var serializer = new DataContractSerializer(typeof(Workspace));

            serializer.WriteObject(writer, this);
            writer.Close();
            fs.Close();
        }
    }
}
