using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Core.Workspace
{
    [DataContract(Name = "Solution", Namespace = "http://davenport.edu")]
    public class Solution
    {
        [DataMember(Name = "SolutionId", IsRequired = true)]
        internal SolutionId solutionId;

        /// <summary>
        /// Create an empty solution in memory
        /// </summary>
        internal Solution() {
            solutionId = new SolutionId();
            Projects = new List<ProjectId>();
        }

        /// <summary>
        /// Create an empty solution in the default location
        /// </summary>
        internal Solution(string name) : this()
        {
            // Name can't be null, empty, or whitespace
            // TODO: check for invalid characters
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty");
            }

            // Solutions must have a name
            solutionId.Name = name;

            // Set the path the "My Documents" folder and use the IDE subfolder
            solutionId.Path = Workspace.UserProjectsDirectory + @"\" + name;

            // Can't use a directory that already exists
            if (Directory.Exists(Path))
            {
                throw new Exception("Directory already exists!");
            }
            else
            {
                Directory.CreateDirectory(Path);

                // Create a subdirectory to store project in
                Directory.CreateDirectory(Path + @"\" + name);
            }
        }

        /// <summary>
        /// Create an empty solution in a specific location
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        internal Solution(string name, string path)
        {

        }

        /// <summary>
        /// Create a new solution with a new specific project in the default location
        /// </summary>
        /// <param name="name">The name of the solution</param>
        /// <param name="Kind">The type of project to add</param>
        internal Solution(string name, OutputKind kind) : this(name)
        {
            // Create a new project
            var project = new Project(name, kind);

            // Assign the directory
            project.directory = Path + @"\" + name;
        }

        /// <summary>
        /// Create a new solution with a new specific type of project in a specific location
        /// </summary>
        /// <param name="name"></param>
        /// <param name="kind"></param>
        /// <param name="path"></param>
        internal Solution(string name, OutputKind kind, string path) : this()
        {

        }

        /// <summary>
        /// Open an existing solution
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        internal Solution(Guid id, string path)
        {

        }

        [DataMember]
        public List<ProjectId> Projects { get; set; }

        public ProjectId this[int key]
        {
            get { return Projects[key]; }
        }

        public Project this[string name]
        {
            // TODO: provide access of projects by name
            get { return null; }
        }

        public Guid Id
        {
            get { return solutionId.Id; }
        }

        public string Name
        {
            get { return solutionId.Name; }
        }

        public string Path
        {
            get { return solutionId.Path; }
        }

        public void CreateProject(string name, OutputKind kind)
        {
            var project = new Project(name, kind);

            // Set the directory of the project
            project.directory = this.Path + @"\" + project.ProjectName;

            // Write project file
            project.WriteTo(project.directory + @"\" + string.Format("{0}.proj.xml", project.ProjectName));

            //Projects.Add(project);
        }

        public void ImportProject(string path)
        {
            // Locate a project file in the directory, but do not look in subfolders
        }

        public void Build(object sender, EventArgs e)
        {

        }

        public void Run(object sender, EventArgs e)
        {

        }

        public void Package(object sender, EventArgs e)
        {

        }

        public void Build()
        {
            // Save all open documents

            // Compile all project files in the solution


            /*var project = new Core.Workspace.Project();

project.AssemblyName = "Test";
project.OutputFile = "Test.exe";
project.Type = OutputKind.ConsoleApplication;

project.Trees = new[] { parser.tree };

project.Compile();
project.Emit();*/
        }

        public void Run()
        {
            Build();
        }


        /// <summary>
        /// Saves a solution file using the default naming scheme
        /// </summary>
        internal void WriteTo()
        {
            WriteTo(string.Format(@"{0}\{1}.sln.xml", Path, Name));
        }

        /// <summary>
        /// Saves a solution file to the specified path
        /// </summary>
        /// <param name="path">The file path being written to</param>
        internal void WriteTo(string path)
        {
            var fs = new FileStream(path, FileMode.Create);
            var settings = new XmlWriterSettings()
            {
                Indent = true
            };

            var writer = XmlWriter.Create(fs, settings);
            var serializer = new DataContractSerializer(typeof(Solution));

            serializer.WriteObject(writer, this);
            writer.Close();
            fs.Close();
        }
    }
}
