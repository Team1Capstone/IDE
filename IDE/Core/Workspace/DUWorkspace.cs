﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;

namespace Core.Workspace
{
    public class DUWorkspace : Microsoft.CodeAnalysis.Workspace, IWorkspace
    {
        public event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentAdded;
        public event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentChanged;
        public event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentReloaded;
        public event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentRemoved;
        public event EventHandler<WorkspaceChangeEventArgs> DocumentAdded;
        public event EventHandler<WorkspaceChangeEventArgs> DocumentChanged;
        public event EventHandler<WorkspaceChangeEventArgs> DocumentReloaded;
        public event EventHandler<WorkspaceChangeEventArgs> DocumentRemoved;
        public event EventHandler<WorkspaceChangeEventArgs> ProjectAdded;
        public event EventHandler<WorkspaceChangeEventArgs> ProjectChanged;
        public event EventHandler<WorkspaceChangeEventArgs> ProjectReloaded;
        public event EventHandler<WorkspaceChangeEventArgs> ProjectRemoved;
        public event EventHandler<WorkspaceChangeEventArgs> SolutionAdded;
        public event EventHandler<WorkspaceChangeEventArgs> SolutionChanged;
        public event EventHandler<WorkspaceChangeEventArgs> SolutionCleared;
        public event EventHandler<WorkspaceChangeEventArgs> SolutionReloaded;
        public event EventHandler<WorkspaceChangeEventArgs> SolutionRemoved;
        public new event EventHandler<DocumentEventArgs> DocumentOpened;
        public new event EventHandler<DocumentEventArgs> DocumentClosed;
        public new event EventHandler<WorkspaceDiagnosticEventArgs> WorkspaceFailed;

        private Action<EventHandler<WorkspaceChangeEventArgs>, WorkspaceChangeEventArgs> EventFire;

        private const string LANGUAGE = "C#";

        public DUWorkspace() : base(MefHostServices.DefaultHost, "DU")
        {
            EventFire = (evt, args) =>
            {
                if (evt != null)
                {
                    evt(this, args);
                }
            };

            DocumentOpened += Workspace_DocumentOpened;
            DocumentClosed += Workspace_DocumentClosed;
            WorkspaceChanged += Workspace_WorkspaceChanged;
            WorkspaceFailed += Workspace_WorkspaceFailed;
        }

        public override bool IsDocumentOpen(DocumentId documentId)
        {
            return base.IsDocumentOpen(documentId);
        }

        public override IEnumerable<DocumentId> GetOpenDocumentIds(ProjectId projectId = null)
        {
            Debug.WriteLine(base.GetOpenDocumentIds(projectId));

            return base.GetOpenDocumentIds(projectId);
        }

        public override bool CanApplyChange(ApplyChangesKind feature)
        {
            return true;
        }

        private void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e) { }

        private void Workspace_WorkspaceChanged(object sender, WorkspaceChangeEventArgs e)
        {
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
                    Debug.WriteLine("Not supported: {0}", e.Kind);
                    break;
            }
        }

        private void Workspace_DocumentClosed(object sender, DocumentEventArgs e) { }

        private void Workspace_DocumentOpened(object sender, DocumentEventArgs e) { }

        public override bool CanOpenDocuments { get { return true; } }

        public override bool TryApplyChanges(Solution solution)
        {
            var result = base.TryApplyChanges(solution);

            Debug.WriteLine("Workspace Apply Changes: " + (result ? "Solution Updated" : "Update Failed"));

            return result;
        }

        /// <summary>
        /// Creates a new solution
        /// </summary>
        /// <param name="name">The name of the solution</param>
        /// <param name="path">Optional: the path to the directory of the solution</param>
        /// <returns></returns>
        public Solution AddSolution(string name, string path = "")
        {
            // Create and obtain the path of the newly created solution directory
            if (string.IsNullOrEmpty(path))
            {
                path = CoreWorkspace.CreateSolutionDirectory(name);
            }

            // Create a logical solution representation
            var si = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), Path.Combine(path, name + ".dusln"));

            // Add it to the workspace
            var solution = AddSolution(si);

            TryApplyChanges(CurrentSolution);

            WriteSolutionFile(CurrentSolution);

            return solution;
        }

        /// http://source.roslyn.codeplex.com/#Microsoft.CodeAnalysis.Workspaces/Workspace/CustomWorkspace.cs,aa170dd3c92e9844
        public Solution AddSolution(SolutionInfo solutionInfo)
        {
            if (solutionInfo == null)
            {
                throw new ArgumentNullException("solutionInfo");
            }

            this.OnSolutionAdded(solutionInfo);

            return this.CurrentSolution;
        }

        // http://source.roslyn.codeplex.com/#Microsoft.CodeAnalysis.Workspaces/Workspace/CustomWorkspace.cs,d3bce3fc2eb1f972
        /// <summary>
        /// Adds a project to the workspace. All previous projects remain intact.
        /// </summary>
        public Project AddProject(ProjectInfo projectInfo)
        {
            if (projectInfo == null)
            {
                throw new ArgumentNullException("projectInfo");
            }

            this.OnProjectAdded(projectInfo);

            var project = this.CurrentSolution.GetProject(projectInfo.Id);

            return project;
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="name">The name of the project</param>
        /// <param name="path">Optional: the path to the directory of the solution</param>
        /// <param name="kind">Optional: the kind of output from this project</param>
        /// <returns></returns>
        public Project AddProject(string name, string path = "", OutputKind kind = OutputKind.DynamicallyLinkedLibrary)
        {
            // Use the project name as the assembly name
            var asmName = name;

            if (string.IsNullOrEmpty(path))
            {
                path = CoreWorkspace.CreateProjectDirectory(name, new FileInfo(CurrentSolution.FilePath).Directory.FullName);
            }

            // C# is the only supported language as this time, so it's a const variable
            // var lang = "C#"

            // The output file. Since there is no debugging functionality there is no need for both a debug AND release folder
            var outputFilePath = Path.Combine(new string[] { path, "bin", string.Format("{0}.{1}", name, kind == OutputKind.ConsoleApplication ? "exe" : "dll") });

            // The path to the project file
            var filePath = Path.Combine(path, string.Format("{0}.duproj", name));

            var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), name, name, LANGUAGE, path)
                .WithOutputFilePath(outputFilePath)
                .WithFilePath(filePath)
                .WithCompilationOptions(new CSharpCompilationOptions(kind))
                .WithMetadataReferences(new[] { MetadataReference.CreateFromAssembly(typeof(object).Assembly) });

            var project = AddProject(projectInfo);
            var doc = string.Format("{0}.cs", name);

            // Add an initial document to the project
            AddDocument(
                project.Id,
                doc,
                Generator.Class(name).NormalizeWhitespace().GetText(),
                Path.Combine(path, doc));
            
            WriteSolutionFile(project.Solution);

            return project;
        }

        /// <summary>
        /// Add an existing project to the current solution
        /// </summary>
        /// <param name="projectFilePath"></param>
        /// <returns></returns>
        public Project AddProject(string projectFilePath)
        {
            var dir = new FileInfo(CurrentSolution.FilePath).Directory;            
            var doc = new XPathDocument(Path.Combine(dir.FullName, projectFilePath));
            var nav = doc.CreateNavigator();

            nav.MoveToRoot();

            var name = nav.SelectSingleNode("Project/Name").Value;
            var asmName = nav.SelectSingleNode("Project/AssemblyName").Value;
            var lang = "C#";
            var outputFilePath = nav.SelectSingleNode("Project/CompilerOptions/OutputFilePath").Value;
            var outputType = nav.SelectSingleNode("Project/CompilerOptions/OutputType").Value;
            var documentFiles = nav.Select("Project/Document[@File]/@File");

            OutputKind kind;

            if (!Enum.TryParse(outputType, out kind))
            {
                throw new ArgumentOutOfRangeException("projectFilePath");
            }

            var options = new CSharpCompilationOptions(kind);

            var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), name, asmName, lang)
                .WithOutputFilePath(Path.Combine(new[] { dir.FullName, name, outputFilePath }))
                .WithFilePath(projectFilePath)
                .WithCompilationOptions(options)
                .WithMetadataReferences(new[] { MetadataReference.CreateFromAssembly(typeof(object).Assembly) });

            var project = AddProject(projectInfo);

            if (documentFiles.Count > 0)
            {
                // Add Documents to Project
                while (documentFiles.MoveNext())
                {
                    var fileInfo = new FileInfo(Path.Combine(new string[] { dir.FullName, name, documentFiles.Current.Value }));

                    // Create a logical document and specifying file path and a text loader
                    var docInfo = DocumentInfo.Create(DocumentId.CreateNewId(project.Id), fileInfo.Name)
                        .WithFilePath(fileInfo.FullName)
                        .WithTextLoader(
                            FileTextLoader.From(
                                TextAndVersion.Create(
                                    SourceText.From(File.Open(fileInfo.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)),
                                    VersionStamp.Create()
                                )
                            )
                        );

                    AddDocument(docInfo);
                }
            }

            return project;
        }

        // http://source.roslyn.codeplex.com/#Microsoft.CodeAnalysis.Workspaces/Workspace/CustomWorkspace.cs,cff074c052848f09
        /// <summary>
        /// Adds a document to the workspace.
        /// </summary>
        public Document AddDocument(DocumentInfo documentInfo)
        {
            if (documentInfo == null)
            {
                throw new ArgumentNullException("documentInfo");
            }

            this.OnDocumentAdded(documentInfo);

            return this.CurrentSolution.GetDocument(documentInfo.Id);
        }

        /// <summary>
        /// Adds a document to the project
        /// </summary>
        public Document AddDocument(ProjectId projectId, string name, SourceText text = null, string filePath = "")
        {
            if (projectId == null || string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException();
            }

            var file = new FileInfo(filePath);

            // Determine the location of the source code (e.g. read from file, or write memory to file)
            if (file.Exists && text == null)
            {
                Debug.WriteLine("Reading source code from file");

                // Read from file
                text = SourceText.From(file.OpenRead());
            }
            else if (!file.Exists && text != null)
            {
                Debug.WriteLine("Writing source code to file");

                // Write SourceText to file
                using (var writer = new StreamWriter(file.OpenWrite()))
                {
                    text.Write(writer);
                }
            }
            else if (!file.Exists) // both are empty/null
            {
                // empty file
                text = SourceText.From(string.Empty);
            }

            var id = DocumentId.CreateNewId(projectId);
            var loader = TextLoader.From(TextAndVersion.Create(text, VersionStamp.Create()));

            var doc = this.AddDocument(DocumentInfo.Create(id, name, loader: loader, filePath: name));

            // Update project file
            WriteProjectFile(doc.Project);

            return doc;
        }

        /// <summary>
        /// Reads an IDE solution file and load according to its parameters
        /// </summary>
        /// <param name="solutionFilePath"></param>
        /// <returns></returns>
        public Solution OpenSolution(string solutionFilePath)
        {
            CloseSolution();

            var doc = new XPathDocument(solutionFilePath);
            var nav = doc.CreateNavigator();

            nav.MoveToRoot();

            // Note: If the format isn't correct, the next few lines MIGHT throw an exception.

            // TODO: validate XML against XSD instead of letting an exception be raised

            var path = nav.SelectSingleNode("Solution/@Path").Value;
            var filePath = nav.SelectSingleNode("Solution/Path").Value;
            var projectFiles = nav.Select("Solution/Project[@File]/@File");

            var solInfo = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), Path.Combine(new string[] { path, filePath }));
            var sol = AddSolution(solInfo);

            // Add Projects to Solution
            if (projectFiles.Count > 0)
            {
                while (projectFiles.MoveNext())
                {
                    AddProject(projectFiles.Current.Value);
                }
            }

            return sol;
        }

        public Project OpenProject(string projectFilePath)
        {
            throw new NotImplementedException();
        }

        public override void OpenDocument(DocumentId documentId, bool activate = true)
        {
            Document doc = CurrentSolution.GetDocument(documentId) as Document;

            if (doc != null)
            {
                CurrentDocument = doc;               

                try
                {
                    OnDocumentOpened(documentId, doc.GetTextAsync().Result.Container);

                    if (DocumentOpened != null)
                    {
                        DocumentOpened(this, new DocumentEventArgs(doc));
                    }
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        void WriteSolutionFile(Solution solution)
        {
            Debug.WriteLine("Updating Solution File");

            // Get directory info from the file path
            var path = new FileInfo(solution.FilePath).Directory;

            // Directory must exist
            if (!path.Exists)
            {
                try
                {
                    // Only allow if path is specified within project directory
                    if (path.Parent.FullName != CoreWorkspace.ProjectDirectory)
                    {
                        throw new Exception("Unable to create directory outside of project folder");
                    }

                    // Attempt to create solution directory
                    path.Create();

                    // Attempt to create project directories (only if they are present)
                    foreach(var p in solution.Projects)
                    {
                        path.CreateSubdirectory(p.Name);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("ERROR: Unable to continue: {0}", ex.Message));
                    throw;
                }
            }

            try
            {
                using (var writer = XmlWriter.Create(solution.FilePath, new XmlWriterSettings() { Indent = true }))
                {
                    var doc = new XDocument(
                        new XElement("Solution",
                            new XAttribute("Path", new FileInfo(solution.FilePath).Directory.FullName),
                            new XElement("Id", solution.Id.Id.ToString()),
                            new XElement("Version", solution.Version.ToString()),
                            new XElement("Path", solution.FilePath.Remove(0, path.FullName.Length + 1))
                        ));

                    foreach (var project in solution.Projects)
                    {
                        doc.Root.Add(
                            new XElement("Project",
                                new XAttribute("File", project.FilePath)
                                ));
                    }

                    doc.WriteTo(writer);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        void WriteProjectFile(Project project, Solution solution = null)
        {
            Debug.WriteLine("Updating Project File");

            solution = solution ?? CurrentSolution;

            var file = new FileInfo(project.FilePath);

            using (var writer = XmlWriter.Create(project.FilePath, new XmlWriterSettings() { Indent = true }))
            {
                var doc = new XDocument(
                    new XElement("Project",
                        new XElement("Id", project.Id.Id.ToString()),
                        new XElement("Name", project.Name),
                        new XElement("AssemblyName", project.AssemblyName),
                        new XElement("Language", project.Language),
                        new XElement("Version", project.Version.ToString()),

                        new XElement("CompilerOptions",
                            new XElement("OutputType", project.CompilationOptions.OutputKind),
                            new XElement("OutputFilePath", project.OutputFilePath)
                        )
                    )
                );

                // Add references
                foreach (var r in project.MetadataReferences)
                {
                    //Debug.WriteLine(r.ToString());
                }

                // Add documents
                foreach (var d in project.Documents)
                {
                    doc.Root.Add(new XElement("Document",
                        new XAttribute("File", d.FilePath != string.Empty ? d.FilePath : d.Name)));
                }

                doc.WriteTo(writer);
            }
        }

        // http://source.roslyn.codeplex.com/#Microsoft.CodeAnalysis.Workspaces/Workspace/CustomWorkspace.cs,a893a29ad7c13150
        /// <summary>
        /// Clears all projects and documents from the workspace
        /// </summary>
        /*public new void ClearSolution()
        {
            base.ClearSolution();
        }*/

        public override void CloseDocument(DocumentId documentId)
        {
            var doc = CurrentSolution.GetDocument(documentId) as Document;

            if (doc != null)
            {
                var e = new DocumentEventArgs(doc);

                if (DocumentClosed != null)
                {
                    DocumentClosed(this, e);
                }
            }
        }

        public void CloseSolution()
        {
            base.OnSolutionRemoved();
        }

        public Document CurrentDocument { get; set; }

        public void Save()
        {
            //
        }

        public void Save(ProjectId projectId)
        {

        }

        public void Save(DocumentId documentId)
        {
            throw new NotImplementedException();
        }

        public void Build()
        {
            CurrentSolution.Emit();
        }

        public void Run()
        {
            Build();

            // TODO: add startup project attribute to solution file, so this method knows which project to run

            var file = CurrentSolution.Projects.First().OutputFilePath;

            Process proc = Process.Start(file);
        }
    }
}