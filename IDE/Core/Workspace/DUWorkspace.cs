using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Core.Workspace
{
    public class DUWorkspace : BaseWorkspace
    {
        public override event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentAdded;
        public override event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentChanged;
        public override event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentReloaded;
        public override event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentRemoved;
        public override event EventHandler<DocumentEventArgs> DocumentOpened;
        public override event EventHandler<DocumentEventArgs> DocumentClosed;
        public override event EventHandler<WorkspaceChangeEventArgs> DocumentAdded;
        public override event EventHandler<WorkspaceChangeEventArgs> DocumentChanged;
        public override event EventHandler<WorkspaceChangeEventArgs> DocumentReloaded;
        public override event EventHandler<WorkspaceChangeEventArgs> DocumentRemoved;
        public override event EventHandler<WorkspaceChangeEventArgs> ProjectAdded;
        public override event EventHandler<WorkspaceChangeEventArgs> ProjectChanged;
        public override event EventHandler<WorkspaceChangeEventArgs> ProjectReloaded;
        public override event EventHandler<WorkspaceChangeEventArgs> ProjectRemoved;
        public override event EventHandler<WorkspaceChangeEventArgs> SolutionAdded;
        public override event EventHandler<WorkspaceChangeEventArgs> SolutionChanged;
        public override event EventHandler<WorkspaceChangeEventArgs> SolutionCleared;
        public override event EventHandler<WorkspaceChangeEventArgs> SolutionReloaded;
        public override event EventHandler<WorkspaceChangeEventArgs> SolutionRemoved;
        //public override event EventHandler<WorkspaceChangeEventArgs> WorkspaceChanged;
        public override event EventHandler<WorkspaceDiagnosticEventArgs> WorkspaceFailed;

        private CustomWorkspace workspace;

        public DUWorkspace()
        {
            workspace = new CustomWorkspace();

            workspace.DocumentOpened += Workspace_DocumentOpened;
            workspace.DocumentClosed += Workspace_DocumentClosed;
            workspace.WorkspaceChanged += Workspace_WorkspaceChanged;
            workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
        }

        /* event handlers */

        private void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            if(WorkspaceFailed != null)
            {
                WorkspaceFailed(this, e);
            }
        }

        private void Workspace_WorkspaceChanged(object sender, WorkspaceChangeEventArgs e)
        {
            Action<EventHandler<WorkspaceChangeEventArgs>, WorkspaceChangeEventArgs> EventFire = (evt, args) => { if (evt != null) { evt(this, args); } };

            switch (e.Kind)
            {
                case WorkspaceChangeKind.SolutionAdded:
                    EventFire(SolutionAdded, e);
                    break;
                case WorkspaceChangeKind.SolutionChanged:
                    EventFire(SolutionAdded, e);
                    break;
                case WorkspaceChangeKind.SolutionCleared:
                    EventFire(SolutionCleared, e);
                    break;
                case WorkspaceChangeKind.SolutionReloaded:
                    EventFire(SolutionReloaded, e);
                    break;
                case WorkspaceChangeKind.SolutionRemoved:
                    EventFire(SolutionRemoved, e);
                    break;
                case WorkspaceChangeKind.DocumentAdded:
                    EventFire(DocumentAdded, e);
                    break;
                case WorkspaceChangeKind.DocumentChanged:
                    EventFire(DocumentChanged, e);
                    break;
                case WorkspaceChangeKind.DocumentReloaded:
                    EventFire(DocumentReloaded, e);
                    break;
                case WorkspaceChangeKind.DocumentRemoved:
                    EventFire(DocumentRemoved, e);
                    break;
                case WorkspaceChangeKind.ProjectAdded:
                    EventFire(ProjectAdded, e);
                    break;
                case WorkspaceChangeKind.ProjectChanged:
                    EventFire(ProjectChanged, e);
                    break;
                case WorkspaceChangeKind.ProjectReloaded:
                    EventFire(ProjectReloaded, e);
                    break;
                case WorkspaceChangeKind.ProjectRemoved:
                    EventFire(ProjectRemoved, e);
                    break;
                case WorkspaceChangeKind.AdditionalDocumentAdded:
                    EventFire(AdditionalDocumentAdded, e);
                    break;
                case WorkspaceChangeKind.AdditionalDocumentChanged:
                    EventFire(AdditionalDocumentChanged, e);
                    break;
                case WorkspaceChangeKind.AdditionalDocumentReloaded:
                    EventFire(AdditionalDocumentReloaded, e);
                    break;
                case WorkspaceChangeKind.AdditionalDocumentRemoved:
                    EventFire(AdditionalDocumentRemoved, e);
                    break;
                default:
                    break;
            }

            Debug.WriteLine(string.Format("Workspace Changed: {0}", e.Kind));
        }

        private void Workspace_DocumentClosed(object sender, DocumentEventArgs e)
        {
            if(DocumentClosed != null)
            {
                DocumentClosed(this, e);
            }
        }

        private void Workspace_DocumentOpened(object sender, DocumentEventArgs e)
        {
            if(DocumentOpened != null)
            {
                DocumentOpened(this, e);
            }
        }

        /* properties */

        public override bool CanCreateSolutions { get { return true; } }
        public override bool CanCreateProjects { get { return File.Exists(CurrentSolution.FilePath); } }
        public override bool CanOpenDocuments { get { return workspace.CanOpenDocuments; } }
        public override Solution CurrentSolution { get { return workspace.CurrentSolution; } }

        /* workspace methods */

        protected void TryApplyChanges(Solution solution)
        {
            var message = workspace.TryApplyChanges(solution) ? "Solution Updated" : "Update Failed";

            Debug.WriteLine("Workspace Apply Changes: " + message);
        }

        /* solution methods */

        public override Solution CreateSolution(string name)
        {
            // Obtain the path of the newly created directory
            var path = BaseWorkspace.CreateSolutionDirectory(name);

            // Create a logical solution representation
            var si = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), path);

            // Add it to the workspace
            var solution = workspace.AddSolution(si);

            TryApplyChanges(CurrentSolution);

            WriteSolution(CurrentSolution);

            return solution;
        }

        public override Solution OpenSolution(string solutionFilePath)
        {
            return OpenSolutionAsync(solutionFilePath).Result;
        }

        public override Task<Solution> OpenSolutionAsync(string solutionFilePath)
        {
            throw new NotImplementedException();
        }

        void WriteSolution(Solution solution)
        {
            // Write the SolutionInfo fields, and the ProjectIds

            // XML is easy to work with
            Debug.WriteLine("Writing to: " + solution.FilePath);

            var path = new FileInfo(solution.FilePath).Directory;
            var writer = XmlWriter.Create(solution.FilePath, new XmlWriterSettings() { Indent = true });

            writer.WriteStartDocument();
            writer.WriteStartElement("Solution");
            writer.WriteAttributeString("Path", path.FullName);
            writer.WriteStartElement("SolutionInfo");
            writer.WriteElementString("Id", solution.Id.Id.ToString());
            writer.WriteElementString("Version", solution.Version.ToString());
            writer.WriteElementString("Path", solution.FilePath.Remove(0, path.FullName.Length));

            writer.WriteStartElement("Projects");

            foreach(var project in CurrentSolution.Projects)
            {
                writer.WriteStartElement("Project");
                writer.WriteElementString("Id", project.Id.Id.ToString());
                writer.WriteElementString("Version", project.Version.ToString());
                writer.WriteElementString("Path", project.FilePath.Remove(0, path.FullName.Length));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Close();
        }

        public override void CloseSolution()
        {

        }

        public override void ClearSolution()
        {
            workspace.ClearSolution();
        }

        /* project methods */

        public override Project OpenProject(string projectFilePath)
        {
            throw new NotImplementedException();
        }

        public override Task<Project> OpenProjectAsync(string projectFilePath)
        {
            // TODO: read a custom project file
            throw new NotImplementedException();
        }

        public override IEnumerable<DocumentId> GetOpenDocumentIds(ProjectId projectId)
        {
            throw new NotImplementedException();
        }

        internal override Project CreateProject(Solution solution, string name, string assemblyName)
        {
            Debug.WriteLine(string.Format("Creating New Project in Solution {0}", solution.Id.Id));

            var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), name, assemblyName, LanguageNames.CSharp);

            var project = workspace.AddProject(projectInfo);
            
            Debug.WriteLine(string.Format("Adding Project {0} to Solution {1}", project.Id.Id, project.Solution.Id.Id));

            var message = workspace.TryApplyChanges(CurrentSolution) ? "Solution Updated" : "Update Failed";

            Debug.WriteLine("Workspace Apply Changes: " + message);
            
            return project;
        }

        public override Project CreateConsoleApplicationProject(Solution solution, string name, string assemblyName)
        {
            var path = BaseWorkspace.CreateProjectDirectory(name, solution.FilePath);

            var outputFile = Path.Combine(path, "bin", string.Format("{0}.exe", name));

            var projectInfo = ProjectInfo.Create(
                ProjectId.CreateNewId(),
                VersionStamp.Create(),
                name,
                assemblyName,
                LanguageNames.CSharp,
                path,
                outputFile,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication),
                new CSharpParseOptions(LanguageVersion.CSharp5)
                );

            var project = workspace.AddProject(projectInfo);

            TryApplyChanges(CurrentSolution);

            WriteSolution(CurrentSolution);

            return project;
        }

        public override Project CreateClassLibraryProject(Solution solution, string name, string assemblyName)
        {
            var path = BaseWorkspace.CreateProjectDirectory(name, solution.FilePath);

            var outputFile = Path.Combine(path, "bin", string.Format("{0}.dll", name));

            var projectInfo = ProjectInfo.Create(
                ProjectId.CreateNewId(),
                VersionStamp.Create(),
                name,
                assemblyName,
                LanguageNames.CSharp,
                path,
                outputFile,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                new CSharpParseOptions(LanguageVersion.CSharp5)
                );

            var project = workspace.AddProject(projectInfo);

            TryApplyChanges(CurrentSolution);

            CreateDocument(project.Id, "test.cs");

            WriteSolution(CurrentSolution);

            return project;
        }

        /* document methods */

        public override Document CreateDocument(ProjectId id, string name)
        {
            var docInfo = DocumentInfo.Create(DocumentId.CreateNewId(id), name)
                .WithFilePath(name);

            var doc = workspace.AddDocument(docInfo);

            TryApplyChanges(CurrentSolution);

            return doc;
        }

        public override Document CreateDocument(ProjectId id, string name, string text)
        {
            var doc = CreateDocument(id, name);

            return doc;       
        }

        public override void OpenDocument(DocumentId documentId)
        {
            workspace.OpenDocument(documentId);
        }

        public override void CloseDocument(DocumentId documentId)
        {
            workspace.CloseDocument(documentId);
        }

        public override void SaveDocument(DocumentId documentId)
        {
            throw new NotImplementedException();
        }
    }
}