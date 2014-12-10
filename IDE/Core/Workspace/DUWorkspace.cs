using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

        public DUWorkspace() : base(MefHostServices.DefaultHost, "DU")
        {
            DocumentOpened += Workspace_DocumentOpened;
            DocumentClosed += Workspace_DocumentClosed;
            WorkspaceChanged += Workspace_WorkspaceChanged;
            WorkspaceFailed += Workspace_WorkspaceFailed;
        }

        public override bool CanApplyChange(ApplyChangesKind feature)
        {
            return true;
        }

        #region Event Handlers
        /* event handlers */

        private void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            /*if(WorkspaceFailed != null)
            {
                WorkspaceFailed(this, e);
            }*/
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
            throw new NotImplementedException();
            /*if(DocumentClosed != null)
            {
                DocumentClosed(this, e);
            }*/
        }

        private void Workspace_DocumentOpened(object sender, DocumentEventArgs e)
        {
            /*if(DocumentOpened != null)
            {
                DocumentOpened(this, e);
            }*/
        }
        #endregion

        #region Properties

        public override bool CanOpenDocuments { get { return true; } }

        public override Solution CurrentSolution { get { return base.CurrentSolution; } }
        
        #endregion

        public override bool TryApplyChanges(Solution solution)
        {
            var result = base.TryApplyChanges(solution);

            Debug.WriteLine("Workspace Apply Changes: " + (result ? "Solution Updated" : "Update Failed"));

            return result;
        }

        #region open-source code
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

            return this.CurrentSolution.GetProject(projectInfo.Id);
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

            // TODO: parse enum to get correct value for output kind
            OutputKind kind;
            if (!Enum.TryParse(outputType, out kind))
            {
                throw new ArgumentOutOfRangeException();
            }

            var options = new CSharpCompilationOptions(kind);

            var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), name, asmName, lang)
                .WithOutputFilePath(Path.Combine(new[] { dir.FullName, name, outputFilePath }))
                .WithFilePath(projectFilePath)
                .WithCompilationOptions(options)
                .WithMetadataReferences(new[] { MetadataReference.CreateFromAssembly(typeof(object).Assembly) });

            var project = AddProject(projectInfo);

            Debug.WriteLine("Project has {0} document(s)", documentFiles.Count);

            // Add Documents to Project
            while (documentFiles.MoveNext())
            {
                Debug.WriteLine(documentFiles.Current.Value);

                var fileInfo = new FileInfo(Path.Combine(new string[] { dir.FullName, name, documentFiles.Current.Value }));
                var fs = new FileStream(fileInfo.FullName, FileMode.Open);
                var ms = new MemoryStream();
                var sb = new StringBuilder();

                fs.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(ms))
                {
                    while (!reader.EndOfStream)
                    {
                        sb.Append(reader.ReadLine());
                    }
                }

                ms.Close();
                fs.Close();

                var docInfo = DocumentInfo.Create(DocumentId.CreateNewId(project.Id), fileInfo.Name)
                    .WithFilePath(fileInfo.FullName)
                    .WithTextLoader(
                        TextLoader.From(
                            TextAndVersion.Create(
                                SourceText.From(File.Open(fileInfo.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)),
                                VersionStamp.Create()
                            )
                        )
                    );

                AddDocument(docInfo);
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

        // http://source.roslyn.codeplex.com/#Microsoft.CodeAnalysis.Workspaces/Workspace/CustomWorkspace.cs,cff074c052848f09
        /// <summary>
        /// Adds a document to the workspace.
        /// </summary>
        public Document AddDocument(ProjectId projectId, string name, SourceText text)
        {
            if (projectId == null)
            {
                throw new ArgumentNullException("projectId");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            var id = DocumentId.CreateNewId(projectId);
            var loader = TextLoader.From(TextAndVersion.Create(text, VersionStamp.Create()));

            return this.AddDocument(DocumentInfo.Create(id, name, loader: loader));
        }
        #endregion

        /// <summary>
        /// Creates a new solution
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Solution AddSolution(string name)
        {
            // Create and obtain the path of the newly created solution directory
            var path = CoreWorkspace.CreateSolutionDirectory(name);

            // Create a logical solution representation
            var si = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), path);

            // Add it to the workspace
            var solution = AddSolution(si);

            TryApplyChanges(CurrentSolution);

            WriteSolution(CurrentSolution);

            return solution;
        }

        /// <summary>
        /// Reads an IDE solution file and load according to its parameters
        /// </summary>
        /// <param name="solutionFilePath"></param>
        /// <returns></returns>
        public Solution OpenSolution(string solutionFilePath)
        {
            var doc = new XPathDocument(solutionFilePath);
            var nav = doc.CreateNavigator();

            nav.MoveToRoot();

            // Note: If the format isn't correct, the next few lines MIGHT throw an exception

            var path = nav.SelectSingleNode("Solution/@Path").Value;
            //var id = nav.SelectSingleNode("Solution/SolutionInfo/Id").Value;
            //var version = nav.SelectSingleNode("Solution/SolutionInfo/Version").Value;
            var filePath = nav.SelectSingleNode("Solution/SolutionInfo/Path").Value;
            var projectFiles = nav.Select("Solution/SolutionInfo/Project[@File]/@File");

            //VersionStamp.Create(DateTime.TryParse())

            var solInfo = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), Path.Combine(new string[] { path, filePath }));
            var sol = AddSolution(solInfo);

            // Add Projects to Solution
            Debug.WriteLine("Solution has {0} project(s)", projectFiles.Count);

            while (projectFiles.MoveNext())
            {
                Debug.WriteLine(projectFiles.Current.Value);

                AddProject(projectFiles.Current.Value);
            }

            return sol;
        }

        void WriteSolution(Solution solution)
        {
            Debug.WriteLine("Writing to: " + solution.FilePath);

            var path = new FileInfo(solution.FilePath).Directory;

            if (!path.Exists)
            {
                throw new DirectoryNotFoundException();
            }

            var writer = XmlWriter.Create(solution.FilePath, new XmlWriterSettings() { Indent = true });

            writer.WriteStartDocument();
            writer.WriteStartElement("Solution");
            writer.WriteAttributeString("Path", path.FullName);

            writer.WriteStartElement("SolutionInfo");
            writer.WriteElementString("Id", solution.Id.Id.ToString());
            writer.WriteElementString("Version", solution.Version.ToString());
            writer.WriteElementString("Path", solution.FilePath.Remove(0, path.FullName.Length));
            writer.WriteEndElement();

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
            writer.WriteEndDocument();

            writer.Close();
        }

        public void CloseSolution()
        {
            OnSolutionRemoved();
        }

        // http://source.roslyn.codeplex.com/#Microsoft.CodeAnalysis.Workspaces/Workspace/CustomWorkspace.cs,a893a29ad7c13150
        /// <summary>
        /// Clears all projects and documents from the workspace
        /// </summary>
        public new void ClearSolution()
        {
            base.ClearSolution();
        }


        public Project OpenProject(string projectFilePath)
        {
            throw new NotImplementedException();
        }
        /* document methods */

        public Document CurrentDocument { get; set; }

        public override void OpenDocument(DocumentId documentId, bool activate = true)
        {
            Document doc = CurrentSolution.GetDocument(documentId) as Document;           

            if (doc != null)
            {
                CurrentDocument = doc;

                DocumentEventArgs e = new DocumentEventArgs(doc);

                if (DocumentOpened != null)
                {
                    DocumentOpened(this, e);
                }
            }
        }

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

        public void Save()
        {
            //
        }
        public void Save(ProjectId projectId) { }
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
        }
    }
}