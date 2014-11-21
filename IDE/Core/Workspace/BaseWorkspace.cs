using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Core.Workspace
{
    public abstract class BaseWorkspace
    {
        internal static string UserDirectory;
        internal static string ProjectDirectory;
        internal static string WorkspaceDirectory;
        internal static string GACDirectory;

        static BaseWorkspace()
        {
            // Easy way to determine the folder of the GAC
            GACDirectory = new FileInfo(typeof(object).Assembly.Location).DirectoryName;

            // Assign values for user directories
            UserDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\IDE";
            ProjectDirectory = UserDirectory + @"\Projects";
            WorkspaceDirectory = UserDirectory + @"\Workspace";

            // Verify main user directory
            if (!Directory.Exists(UserDirectory))
            {
                Directory.CreateDirectory(UserDirectory);
            }

            // Verify "Projects" directory
            if (!Directory.Exists(ProjectDirectory))
            {
                Directory.CreateDirectory(ProjectDirectory);
            }

            // Verify "Workspaces" directory
            if (!Directory.Exists(WorkspaceDirectory))
            {
                Directory.CreateDirectory(WorkspaceDirectory);
            }
        }

        internal static string CreateSolutionDirectory(string name)
        {
            //TODO verify name

            var path = Path.Combine(ProjectDirectory, name);
            var di = new DirectoryInfo(path);

            if (!di.Exists)
            {
                // Create Solution Directory
                di.Create();

                // Create Solution File
                path = Path.Combine(path, name + ".dusln");
                var writer = File.CreateText(path);

                writer.Write("mumbo jumbo");
                writer.Close();
            }
            else
            {
                throw new Exception("Solution already exists");
            }

            return path;
        }

        internal static string CreateProjectDirectory(string name, string solutionDirectory)
        {
            var solutionFile = new FileInfo(solutionDirectory);

            if (!solutionFile.Exists)
            {
                throw new FileNotFoundException("Solution File Not Found");
            }        

            var solutionDir = solutionFile.Directory;

            if (!solutionDir.Exists)
            {
                throw new DirectoryNotFoundException("Solution Directory Not Found");
            }

            // Create Project Directory in Solution Directory
            var projectDir = solutionDir.CreateSubdirectory(name);

            var path = Path.Combine(projectDir.FullName, name + ".duproj");

            // Create Project File in Project Directory
            var writer = File.CreateText(path);

            writer.Write("project file");
            writer.Close();

            return path;
        }

        /* events */
        public abstract event EventHandler<WorkspaceChangeEventArgs> SolutionAdded;
        public abstract event EventHandler<WorkspaceChangeEventArgs> SolutionChanged;
        public abstract event EventHandler<WorkspaceChangeEventArgs> SolutionCleared;
        public abstract event EventHandler<WorkspaceChangeEventArgs> SolutionReloaded;
        public abstract event EventHandler<WorkspaceChangeEventArgs> SolutionRemoved;
        public abstract event EventHandler<WorkspaceChangeEventArgs> DocumentAdded;
        public abstract event EventHandler<WorkspaceChangeEventArgs> DocumentChanged;
        public abstract event EventHandler<WorkspaceChangeEventArgs> DocumentReloaded;
        public abstract event EventHandler<WorkspaceChangeEventArgs> DocumentRemoved;
        public abstract event EventHandler<DocumentEventArgs> DocumentOpened;
        public abstract event EventHandler<DocumentEventArgs> DocumentClosed;
        public abstract event EventHandler<WorkspaceChangeEventArgs> ProjectAdded;
        public abstract event EventHandler<WorkspaceChangeEventArgs> ProjectChanged;
        public abstract event EventHandler<WorkspaceChangeEventArgs> ProjectReloaded;
        public abstract event EventHandler<WorkspaceChangeEventArgs> ProjectRemoved;
        public abstract event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentAdded;
        public abstract event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentChanged;
        public abstract event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentReloaded;
        public abstract event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentRemoved;
        //public abstract event EventHandler<WorkspaceChangeEventArgs> WorkspaceChanged;
        public abstract event EventHandler<WorkspaceDiagnosticEventArgs> WorkspaceFailed;

        /* properties */
        public abstract bool CanCreateSolutions { get; }
        public abstract bool CanCreateProjects { get; }
        public abstract bool CanOpenDocuments { get; }
        public abstract Solution CurrentSolution { get; }

        /* solution methods */
        public abstract Solution CreateSolution(string name);
        public abstract Solution OpenSolution(string solutionFilePath);
        public abstract Task<Solution> OpenSolutionAsync(string solutionFilePath);
        public abstract void ClearSolution();
        public abstract void CloseSolution();

        /* project methods */
        internal abstract Project CreateProject(Solution solution, string name, string assemblyName);
        public abstract Project CreateConsoleApplicationProject(Solution solution, string name, string assemblyName);
        public abstract Project CreateClassLibraryProject(Solution solution, string name, string assemblyName);
        public abstract Project OpenProject(string projectFilePath);
        public abstract Task<Project> OpenProjectAsync(string projectFilePath);
        public abstract IEnumerable<DocumentId> GetOpenDocumentIds(ProjectId projectId);

        /* document methods */
        public abstract Document CreateDocument(ProjectId id, string name);
        public abstract Document CreateDocument(ProjectId id, string name, string text);
        public abstract void OpenDocument(DocumentId documentId);
        public abstract void CloseDocument(DocumentId documentId);
        public abstract void SaveDocument(DocumentId documentId);
    }
}
