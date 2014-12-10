using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Core.Workspace
{
    public interface IWorkspace
    {
        // Workspace Changes
        event EventHandler<WorkspaceChangeEventArgs> SolutionAdded;
        event EventHandler<WorkspaceChangeEventArgs> SolutionChanged;
        event EventHandler<WorkspaceChangeEventArgs> SolutionCleared;
        event EventHandler<WorkspaceChangeEventArgs> SolutionReloaded;
        event EventHandler<WorkspaceChangeEventArgs> SolutionRemoved;
        event EventHandler<WorkspaceChangeEventArgs> DocumentAdded;
        event EventHandler<WorkspaceChangeEventArgs> DocumentChanged;
        event EventHandler<WorkspaceChangeEventArgs> DocumentReloaded;
        event EventHandler<WorkspaceChangeEventArgs> DocumentRemoved;
        event EventHandler<WorkspaceChangeEventArgs> ProjectAdded;
        event EventHandler<WorkspaceChangeEventArgs> ProjectChanged;
        event EventHandler<WorkspaceChangeEventArgs> ProjectReloaded;
        event EventHandler<WorkspaceChangeEventArgs> ProjectRemoved;
        event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentAdded;
        event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentChanged;
        event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentReloaded;
        event EventHandler<WorkspaceChangeEventArgs> AdditionalDocumentRemoved;
        
        // Document Changes
        event EventHandler<DocumentEventArgs> DocumentOpened;
        event EventHandler<DocumentEventArgs> DocumentClosed;

        // Workspace Diagnostic Changes
        event EventHandler<WorkspaceDiagnosticEventArgs> WorkspaceFailed;

        bool TryApplyChanges(Solution solution);

        Solution AddSolution(string name);
        Solution AddSolution(SolutionInfo solutionInfo);
        Project AddProject(ProjectInfo projectInfo);

        Solution OpenSolution(string solutionFilePath);
        Project OpenProject(string projectFilePath);
        void OpenDocument(DocumentId documentId, bool activate = true);

        Document CurrentDocument { get; }


        Solution CurrentSolution { get; }

        void Build();
        void Run();
    }
}