using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Core;
using Core.Workspace;
using Core.Text;

namespace IDE
{
    public partial class MainWindow : Form
    {        
        private BaseWorkspace Workspace;
        private Parser parser;

        public MainWindow()
        {
            InitializeComponent();

            parser = new Parser();
            Workspace = new DUWorkspace();

            /* setup workspace events */
            Workspace.WorkspaceFailed += Workspace_WorkspaceFailed;

            Workspace.DocumentAdded += Workspace_DocumentAdded;
            Workspace.DocumentChanged += Workspace_DocumentChanged;
            Workspace.DocumentReloaded += Workspace_DocumentReloaded;
            Workspace.DocumentRemoved += Workspace_DocumentRemoved;
            Workspace.DocumentOpened += Workspace_DocumentOpened;
            Workspace.DocumentClosed += Workspace_DocumentClosed;

            Workspace.SolutionAdded += Workspace_SolutionAdded;
            Workspace.SolutionChanged += Workspace_SolutionChanged;
            Workspace.SolutionCleared += Workspace_SolutionCleared;
            Workspace.SolutionReloaded += Workspace_SolutionReloaded;
            Workspace.SolutionRemoved += Workspace_SolutionRemoved;

            // When text is selected the parser needs to know what text is selected
            TextEditor.SelectionChanged += parser.SelectionChanged;

            // Parser maintains a SyntaxTree and needs to be updated when the text changes
            TextEditor.TextChanged += parser.TextChanged;

            /* setup parser events */
            parser.EnableHighlighting = false; // new highlighter isnt done yet :(
            parser.HighlighterUpdated += Parser_HighlighterUpdate;
            parser.TreeChanged += Parser_TreeChanged;

            packageMenuItem.Click += packageToolStripMenuItem_Click;
        }

        private void Workspace_DocumentRemoved(object sender, WorkspaceChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Workspace_DocumentReloaded(object sender, WorkspaceChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Workspace_DocumentChanged(object sender, WorkspaceChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Workspace_SolutionRemoved(object sender, WorkspaceChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Workspace_SolutionReloaded(object sender, WorkspaceChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Workspace_SolutionCleared(object sender, WorkspaceChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Workspace_SolutionChanged(object sender, WorkspaceChangeEventArgs e)
        {
            Debug.WriteLine("Solution Changed");
            Debug.WriteLine(string.Format("Old Solution {0}", e.NewSolution.Id.Id));
            Debug.WriteLine(string.Format("New Solution {0}", e.OldSolution.Id.Id));
        }

        private void Workspace_SolutionAdded(object sender, WorkspaceChangeEventArgs e)
        {
            Debug.WriteLine("Solution Added");

            if(e.OldSolution != null)
            {
                Debug.WriteLine("Old Solution:");
                Debug.WriteLine(e.OldSolution.Id.Id);
                Debug.WriteLine(string.IsNullOrWhiteSpace(e.OldSolution.FilePath) ? @"n\a" : e.OldSolution.FilePath);
            }

            if(e.NewSolution != null)
            {
                Debug.WriteLine("New Solution:");
                Debug.WriteLine(e.NewSolution.Id.Id);
                Debug.WriteLine(string.IsNullOrWhiteSpace(e.NewSolution.FilePath) ? @"n\a" : e.NewSolution.FilePath);
            }

            UpdateWorkspaceTreeNodes();
        }

        private void Workspace_DocumentAdded(object sender, WorkspaceChangeEventArgs e)
        {
            Debug.WriteLine("Document Added");

            if (e.DocumentId != null)
            {
                var doc = Workspace.CurrentSolution.GetDocument(e.DocumentId);

                // Update Solution Tree View

                Debug.WriteLine(doc.FilePath);
            }

            if(e.ProjectId != null)
            {
                var project = Workspace.CurrentSolution.GetProject(e.ProjectId);

                // Update Solution Tree View

                Debug.WriteLine(project.FilePath);
            }

            UpdateWorkspaceTreeNodes();
        }

        private void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            Debug.WriteLine(string.Format("{0} | {1}", e.Diagnostic.Kind, e.Diagnostic.Message));
        }
        /*
        private void Workspace_WorkspaceChanged(object sender, WorkspaceChangeEventArgs e)
        {
            if (e.NewSolution != null && e.OldSolution != null && e.NewSolution.Id.Id != e.OldSolution.Id.Id)
            {
                Debug.WriteLine("Old Solution: " + e.OldSolution.FilePath);
                Debug.WriteLine("New Solution: " + e.NewSolution.FilePath);
                Debug.WriteLine("Current Solution: " + Workspace.CurrentSolution.FilePath);
                Debug.WriteLine(string.Format("\t\t{0} Projects in New Solution", Workspace.CurrentSolution.ProjectIds.Count));

                var TreeNode = new TreeNode(string.Format("{0} ({1} Project{2})", e.NewSolution.Id.Id, Workspace.CurrentSolution.ProjectIds.Count, "s"));

                foreach (var id in Workspace.CurrentSolution.ProjectIds)
                {
                    Debug.WriteLine(string.Format("\t\t\t-- Project Id {0}", id.Id));

                    var project = Workspace.CurrentSolution.GetProject(id);
                    var ProjectNode = new TreeNode(string.Format("{0} ({1} Documents)", project.Name, project.DocumentIds.Count));

                    foreach (var did in project.DocumentIds)
                    {
                        ProjectNode.Nodes.Add(string.Format("Document {0}", did.Id));
                    }

                    TreeNode.Nodes.Add(ProjectNode);
                }

                WorkspaceTreeView.Nodes.Clear();
                WorkspaceTreeView.Nodes.Add(TreeNode);
            }

            if (e.ProjectId != null)
            {
                Debug.WriteLine(string.Format("Project {0}", e.ProjectId.Id));

                var TreeNode = new TreeNode(string.Format("{0} ({1} Project{2})", e.NewSolution.Id.Id, Workspace.CurrentSolution.ProjectIds.Count, "s"));

                foreach (var id in Workspace.CurrentSolution.ProjectIds)
                {
                    Debug.WriteLine(string.Format("\t\t\t-- Project Id {0}", id.Id));

                    var project = Workspace.CurrentSolution.GetProject(id);
                    var ProjectNode = new TreeNode(string.Format("{0} ({1} Documents)", project.Name, project.DocumentIds.Count));

                    foreach (var did in project.DocumentIds)
                    {
                        var document = project.GetDocument(did);

                        ProjectNode.Nodes.Add(string.Format("{0}", document.Name));
                    }

                    TreeNode.Nodes.Add(ProjectNode);
                }

                WorkspaceTreeView.Nodes.Clear();
                WorkspaceTreeView.Nodes.Add(TreeNode);
            }

            if (e.DocumentId != null)
                Debug.WriteLine(string.Format("Document {0}", e.DocumentId.Id));
        }
        */
        private void Workspace_DocumentClosed(object sender, DocumentEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Workspace_DocumentOpened(object sender, DocumentEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdateWorkspaceTreeNodes()
        {
            TreeNode root = new TreeNode("Solution");
            root.Tag = Workspace.CurrentSolution.Id;

            TreeNode node = null;
            foreach (var project in Workspace.CurrentSolution.Projects)
            {
                node = new TreeNode(project.Name);
                node.Tag = project.Id;
                node.Nodes.AddRange(project.Documents.Select(d => new TreeNode(d.Name)).ToArray());
                root.Nodes.Add(node);
            }

            WorkspaceTreeView.Nodes.Clear();
            WorkspaceTreeView.Nodes.Add(root);
        }
        

        /// <summary>
        /// Event handler for storing the tree in the correct document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Parser_TreeChanged(object sender, EventArgs e)
        {   
            // Update the Document object (not the file) with the changed SyntaxTree
        }

        private void NormalizeWhitespaceMenuItem_Click(object sender, EventArgs e)
        {
            TextEditor.Text = Parser.NormalizeWhitespace(TextEditor.Text).ToString();

            // TODO: position the cursor correctly, instead of letting it go to line 0, char 0
        }

        private void Parser_HighlighterUpdate(object sender, HighlighterEventArgs e)
        {
            // Use a temporary RichTextBox to prevent most of the flickering that occurs when using TextEditor.Select
            RichTextBox temp = new RichTextBox();
            temp.Font = TextEditor.Font;
            temp.Rtf = TextEditor.Rtf;
            temp.SelectionStart = TextEditor.SelectionStart;

            // Store the position of the caret
            int pos = TextEditor.SelectionStart;

            foreach (var change in e.Changes)
            {
                // Select the text that will be highlighted
                temp.Select(change.Span.Start, change.Span.Length);

                // Change the color of the text
                temp.SelectionColor = change.Color;
            }

            // Push changed text with styling
            TextEditor.Rtf = temp.Rtf;

            // Restore the position of the caret, and use default text color
            TextEditor.Select(pos, 0);
            TextEditor.SelectionColor = Color.White;
        }

        private void packageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region Highlighter Test
            //var highlight = new ClassificationHighlighter();

            //highlight.Format(true, "name.cs", SourceText.From(TextEditor.Text));
            #endregion
        }

        private void NewSolutionMenuItem_Click(object sender, EventArgs e)
        {
            // Is there is a solution open, close it (don't forget to provide save prompts!)

            // TODO: Prompt user for a name

            Workspace.CreateSolution("name");
        }

        private void consoleApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var window = new NewProject();
            
            // TODO Prompt user for information
            //var result = window.ShowDialog();

            Workspace.CreateConsoleApplicationProject(Workspace.CurrentSolution, "HelloWorld", "HelloWorld");
        }

        private void classLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Prompt user for information

            Workspace.CreateClassLibraryProject(Workspace.CurrentSolution, "HelloWorldLib", "HelloWorldLib");
        }

        private void visualStudioSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog();

            fd.Filter = "Visual Studio Solutions | *.sln";
            fd.ShowDialog();
        }

        private void visualStudioProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog();

            fd.Filter = "C# Project Files | *.csproj";
            fd.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO save prompts
            Close();
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Unsaved changes will be lost. Are you sure?", "Confirm", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                Workspace.CloseSolution();
            }
        }
    }
}
