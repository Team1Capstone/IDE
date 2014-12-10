using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Core;
using Core.Workspace;
using Core.Text;
using System.IO;

namespace IDE
{
    public partial class MainWindow : Form
    {
        private IWorkspace Workspace;
        private Parser parser;
        private NewProjectDialog newProjectDialog;
              
        public MainWindow()
        {
            InitializeComponent();

            MainFlowPanel_SizeChanged(this, new EventArgs());
            MainWindowMenu.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            MainWindowMenu.Renderer = new CustomRenderer();

            parser = new Parser();
            Workspace = new DUWorkspace();
            newProjectDialog = new NewProjectDialog();

            Workspace.WorkspaceFailed += Workspace_WorkspaceFailed;

            Workspace.DocumentOpened += Workspace_DocumentOpened;
            Workspace.DocumentClosed += Workspace_DocumentClosed;

            Workspace.DocumentAdded += Workspace_DocumentAdded;
            Workspace.DocumentChanged += Workspace_DocumentChanged;
            Workspace.DocumentReloaded += Workspace_DocumentReloaded;
            Workspace.DocumentRemoved += Workspace_DocumentRemoved;

            Workspace.SolutionAdded += Workspace_SolutionAdded;
            Workspace.SolutionChanged += Workspace_SolutionChanged;
            Workspace.SolutionCleared += Workspace_SolutionCleared;
            Workspace.SolutionReloaded += Workspace_SolutionReloaded;
            Workspace.SolutionRemoved += Workspace_SolutionRemoved;

            Workspace.ProjectAdded += Workspace_ProjectAdded;
            Workspace.ProjectChanged += Workspace_ProjectChanged;
            Workspace.ProjectReloaded += Workspace_ProjectReloaded;
            Workspace.ProjectRemoved += Workspace_ProjectRemoved;

            TextEditor.SelectionChanged += parser.SelectionChanged;
            TextEditor.TextChanged += Editor_TextChanged;

            parser.HighlighterUpdated += Parser_HighlighterUpdated;

            packageMenuItem.Click += packageToolStripMenuItem_Click;

            newProjectDialog.SolutionCreated += NewProjectDialog_SolutionCreated;
            newProjectDialog.ProjectCreated += NewProjectDialog_ProjectCreated;
        }

        private void NewProjectDialog_SolutionCreated(object sender, EventArgs e)
        {

        }

        private void NewProjectDialog_ProjectCreated(object sender, EventArgs e)
        {
            //newProjectDialog.
            //throw new NotImplementedException();
        }

        private void Workspace_ProjectRemoved(object sender, WorkspaceChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Workspace_ProjectReloaded(object sender, WorkspaceChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Workspace_ProjectChanged(object sender, WorkspaceChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Workspace_ProjectAdded(object sender, WorkspaceChangeEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Error(string message)
        {
            Message(string.Format("Error: {0}", message));
        }

        private void Debug(string message)
        {
            Message(string.Format("Debug: {0}", message));
        }

        private void Message(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);

            MessagesBox.Items.Add(message);
        }

        #region Workspace Event Handlers
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
            Message(string.Format("[Solution Changed] Old Solution: {0} | New Solution: {1}", e.OldSolution.Id.Id, e.NewSolution.Id.Id));
        }

        private void Workspace_SolutionAdded(object sender, WorkspaceChangeEventArgs e)
        {
            var sb = new StringBuilder("[Solution Added]");

            if (e != null && e.OldSolution != null)
            {
                sb.AppendLine();
                sb.Append('\t');
                sb.Append("Old Solution: ");
                sb.Append(e.OldSolution.Id.Id.ToString());
                sb.Append(string.IsNullOrWhiteSpace(e.OldSolution.FilePath) ? @"n\a" : e.OldSolution.FilePath);
            }

            if (e.NewSolution != null && e.NewSolution != null)
            {
                sb.AppendLine();
                sb.Append('\t');
                sb.Append("New Solution:");
                sb.Append(e.NewSolution.Id.Id.ToString());
                sb.Append(string.IsNullOrWhiteSpace(e.NewSolution.FilePath) ? @"n\a" : e.NewSolution.FilePath);
            }

            Message(sb.ToString());

            UpdateWorkspaceTreeNodes();
        }

        private void Workspace_DocumentAdded(object sender, WorkspaceChangeEventArgs e)
        {
            Message("Workspace Changed: Document Added");

            if (e.DocumentId != null)
            {
                var doc = Workspace.CurrentSolution.GetDocument(e.DocumentId);

                // Add button to the FileControlPanel
                var button = new Button();

                button.Text = doc.Name;
                button.Width = 75;
                button.Height = 30;
                button.Tag = doc.Id;
                button.Click += Control_Click;

                FileControlPanel.Controls.Add(button);
            }

            if (e.ProjectId != null)
            {
                var project = Workspace.CurrentSolution.GetProject(e.ProjectId);

                // Update Solution Tree View

                Message(project.FilePath);
            }

            UpdateWorkspaceTreeNodes();
        }

        private void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            Message(string.Format("{0} | {1}", e.Diagnostic.Kind, e.Diagnostic.Message));
        }

        private void Workspace_DocumentClosed(object sender, DocumentEventArgs e)
        {
            Debug("Workspace Changed: Document Closed");
        }

        private void Workspace_DocumentOpened(object sender, DocumentEventArgs e)
        {
            Debug("Workspace Changed: Document Opened");

            e.Document.GetTextAsync().ContinueWith(t =>
            {
                TextEditor.Text = t.Result.ToString();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        private void UpdateWorkspaceTreeNodes()
        {
            Message("Updating Solution Tree Nodes");

            TreeNode root = new TreeNode("Solution");
            root.Tag = Workspace.CurrentSolution.Id;

            TreeNode node = null;
            foreach (var project in Workspace.CurrentSolution.Projects)
            {
                node = new TreeNode(project.Name);
                node.Tag = project.Id;

                foreach (var doc in project.Documents)
                {
                    TreeNode child = new TreeNode(doc.Name);
                    child.Tag = doc.Id;
                    node.Nodes.Add(child);
                }

                root.Nodes.Add(node);
            }

            WorkspaceTreeView.Nodes.Clear();
            WorkspaceTreeView.Nodes.Add(root);
            WorkspaceTreeView.ExpandAll();
        }

        private void Editor_TextChanged(object sender, EventArgs e)
        {
            parser.Text = SourceText.From(TextEditor.Text);

            if (Workspace.CurrentDocument != null)
            {
                // Highlight syntax in document using text from the UI (not the file)
                parser.UpdateTree(Workspace.CurrentDocument.WithText(SourceText.From(TextEditor.Text)));

                // Evaluate solution for warnings and errors
                Task t = Task.Factory.StartNew(() => Workspace.CurrentSolution.Evaluate());
            }
            else
            {
                // Notify user that they are typing in an empty buffer that isn't associated with a file
            }
        }

        private void NormalizeWhitespace_Click(object sender, EventArgs e)
        {
            TextEditor.Text = Parser.NormalizeWhitespace(TextEditor.Text).ToString();

            // TODO: position the cursor correctly, instead of letting it go to line 0, char 0
        }

        private void Parser_HighlighterUpdated(object sender, HighlighterEventArgs e)
        {
            // Remove event handler, as this method will change the text, thus causing another highlight pass
            TextEditor.TextChanged -= Editor_TextChanged;

            Message(string.Format("Highlighting: {0} changes", e.Changes.Count));

            // Use a temporary RichTextBox to prevent most of the flickering that occurs when using TextEditor.Select on a visible object
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

            // Restore event handler
            TextEditor.TextChanged += Editor_TextChanged;
        }

        private void packageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Note: this will not create VS formats of a solution file and any project files. Converting to a VS format will have to be done by hand

            var dir = new FileInfo(Workspace.CurrentSolution.FilePath).Directory;

            // Only files specifically mentioned in the solution and project files will be included in the ZIP
        }

        private void NewSolutionMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                newProjectDialog.ShowDialog();

                // The rest is handled through events
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void OpenIDESolutionFile_Click(object sender, EventArgs e)
        {
            OpenFile("IDE Solutions | *.dusln", t => Workspace.OpenSolution(t));
        }

        private void OpenIDEProjectFile_Click(object sender, EventArgs e)
        {
            OpenFile("IDE Project Files | *.duproj", t => Workspace.OpenProject(t));
        }

        private void OpenVSSolutionFile_Click(object sender, EventArgs e)
        {
            OpenFile("Visual Studio Solutions | *.sln");
        }

        private void OpenVSProjectFile_Click(object sender, EventArgs e)
        {
            OpenFile("C# Project Files | *.csproj");
        }

        private DialogResult OpenFile(string filter, out string fileName)
        {
            fileName = string.Empty;

            var fd = new OpenFileDialog();
            fd.Filter = filter;

            var result = fd.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (fd.CheckPathExists && fd.CheckFileExists)
                {
                    fileName = fd.FileName;
                }
            }

            return result;
        }

        private void OpenFile(string filter, Action<string> func = null)
        {
            var fileName = string.Empty;
            var result = OpenFile(filter, out fileName);

            if (result == DialogResult.OK && func != null)
            {
                func(fileName);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            CloseAllDocuments_Click(sender, e);

            // TODO: prevent exit if cancel is pressed

            Close();
        }

        private void CloseAllDocuments_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Save changes?", "Confirm", MessageBoxButtons.YesNoCancel);

            if (result != DialogResult.Cancel)
            {
                if (result == DialogResult.Yes)
                {
                    // Save all open documents
                }
                else
                {
                    // Discard changes
                }
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var docId = e.Node.Tag as DocumentId;

            // Node is a document
            if (docId != null)
            {
                // Opens a document, and loads the contents of a document in the TextEditor via event handling
                Workspace.OpenDocument(docId);
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {
            var ctrl = sender as Control;

            if (ctrl != null)
            {
                var doc = ctrl.Tag as DocumentId;

                if(doc != null)
                {
                    Workspace.OpenDocument(doc);
                }
            }
        }

        private void WorkspaceTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                if (e.Node.Tag is ProjectId)
                {
                    Message("Adjusting context menu for project");
                }
                else if (e.Node.Tag is DocumentId)
                {
                    Message("Adjusting context menu for document");
                }
                else if (e.Node.Tag is SolutionId)
                {
                    Message("Adjusting context menu for solution");
                }
            }
        }

        private void Cut(object sender, EventArgs e)
        {
            TextEditor.Cut();
        }

        private void Copy(object sender, EventArgs e)
        {
            TextEditor.Copy();
        }

        private void Paste(object sender, EventArgs e)
        {
            TextEditor.Paste(DataFormats.GetFormat(DataFormats.Text));
        }

        private void SelectAll(object sender, EventArgs e)
        {
            TextEditor.SelectAll();
        }

        private void Undo(object sender, EventArgs e)
        {
            TextEditor.Undo();
        }

        private void Redo(object sender, EventArgs e)
        {
            TextEditor.Redo();
        }

        private void buildMenuItem_Click(object sender, EventArgs e)
        {
            Workspace.Build();
        }

        private void runMenuItem_Click(object sender, EventArgs e)
        {
            Workspace.Run();
        }

        private void MainFlowPanel_Resize(object sender, EventArgs e)
        {

        }

        private void MainFlowPanel_SizeChanged(object sender, EventArgs e)
        {
            FileControlPanel.Width = MainFlowPanel.Width - MainFlowPanel.Margin.Size.Width;
            TextEditor.Width = MainFlowPanel.Width - MainFlowPanel.Margin.Size.Width;
            MessagePanel.Width = MainFlowPanel.Width - MainFlowPanel.Margin.Size.Width;

            TextEditor.Height = MainFlowPanel.Height - FileControlPanel.Height - MessagePanel.Height;
        }

        private void splitContainer1_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {

        }

        private void TextEditor_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void TextEditor_MouseClick(object sender, MouseEventArgs e)
        {
            var c = TextEditor.GetFirstCharIndexOfCurrentLine();

            var d = TextEditor.GetCharIndexFromPosition(e.Location);

            Debug(c.ToString());
            Debug(d.ToString());
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
