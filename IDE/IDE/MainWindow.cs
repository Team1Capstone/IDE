using System;
using System.Collections.Immutable;
using System.Diagnostics;
//using System.Drawing;
using System.IO;
using System.Linq;
using System.Timers;
using System.Text;
using System.Threading.Tasks;
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
        private System.Timers.Timer timer;
        private IWorkspace Workspace;
        private Parser parser;
        private NewProjectDialog newProjectDialog;

        public MainWindow()
        {
            InitializeComponent();

            MainFlowPanel_SizeChanged(this, new EventArgs());
            MainWindowMenu.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            MainWindowMenu.Renderer = new CustomRenderer(); // Styling MenuItems in WinForms is cumbersome and annoying (should have used WPF!)

            parser = new Parser();
            Workspace = new DUWorkspace();
            newProjectDialog = new NewProjectDialog();
            timer = new System.Timers.Timer(2500)
            {
                AutoReset = false,
                Enabled = false
            };

            #region Event Listening

            // compiler results need to be delayed
            timer.Elapsed += Timer_Elapsed;

            // Workspace events             
            Workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
            Workspace.DocumentOpened += Workspace_DocumentOpened;
            Workspace.DocumentClosed += Workspace_DocumentClosed;

            Workspace.DocumentAdded += Workspace_Document;
            Workspace.DocumentChanged += Workspace_Document;
            Workspace.DocumentReloaded += Workspace_Document;
            Workspace.DocumentRemoved += Workspace_Document;

            Workspace.SolutionAdded += Workspace_Solution;
            Workspace.SolutionChanged += Workspace_Solution;
            Workspace.SolutionCleared += Workspace_Solution;
            Workspace.SolutionReloaded += Workspace_Solution;
            Workspace.SolutionRemoved += Workspace_Solution;

            Workspace.ProjectAdded += Workspace_Project;
            Workspace.ProjectChanged += Workspace_Project;
            Workspace.ProjectReloaded += Workspace_Project;
            Workspace.ProjectRemoved += Workspace_Project;

            // TextEditor (RichTextbox) events
            TextEditor.SelectionChanged += parser.SelectionChanged;
            TextEditor.TextChanged += Editor_TextChanged;

            parser.HighlighterUpdated += Parser_HighlighterUpdated;

            // NewProjectDialog events
            newProjectDialog.SolutionDirectoryCreated += (sender, e) => { Workspace.AddSolution(newProjectDialog.ProjectName, newProjectDialog.SolutionPath); };
            newProjectDialog.ProjectDirectoryCreated += (sender, e) => { Workspace.AddProject(newProjectDialog.ProjectName, newProjectDialog.ProjectPath, newProjectDialog.Kind); };

            // UI menu events (most have shortcut keys!)
            newToolStripMenuItem.Click += (sender, e) => { newProjectDialog.ShowDialog(); };
            openToolStripMenuItem.Click += (sender, e) => { OpenFile("IDE Solutions | *.dusln", t => Workspace.OpenSolution(t)); };
            runMenuItem.Click += (sender, e) => { Workspace.Run(); };
            buildMenuItem.Click += (sender, e) => { Workspace.Build(); };
            cutToolStripMenuItem.Click += (sender, e) => { TextEditor.Cut(); };
            copyToolStripMenuItem.Click += (sender, e) => { TextEditor.Copy(); };
            pasteToolStripMenuItem.Click += (sender, e) => { TextEditor.Paste(DataFormats.GetFormat(DataFormats.Text)); };
            selectAllToolStripMenuItem.Click += (sender, e) => { TextEditor.SelectAll(); };
            undoToolStripMenuItem.Click += (sender, e) => { TextEditor.Undo(); };
            redoToolStripMenuItem.Click += (sender, e) => { TextEditor.Redo(); };
            #endregion

            #region Compiler Results DataGrid
            CompilerResults.AutoGenerateColumns = false;

            AddColumn("Message");
            AddColumn("Id");
            AddColumn("Category");
            AddColumn("Kind");
            AddColumn("Start");
            AddColumn("End");
            AddColumn("Length");
            AddColumn("File");
            AddColumn("StartLine");
            AddColumn("StartChar");
            AddColumn("EndLine");
            AddColumn("EndChar");
            #endregion
        }

        private void AddColumn(string property, string text = "")
        {
            if (string.IsNullOrEmpty(text))
            {
                text = property;
            }

            var col = new DataGridViewTextBoxColumn()
            {
                DataPropertyName = property,
                HeaderText = text
            };

            CompilerResults.Columns.Add(col);          
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Invoke method on UI thread
            BeginInvoke(new MethodInvoker(delegate
            {
                DisplayCompilerResults();
            }));
        }

        #region Message/Debug/Error output functions
        private void LogError(string message)
        {
            LogMessage(string.Format("Error: {0}", message));
        }

        private void LogDebug(string message)
        {
            LogMessage(string.Format("Debug: {0}", message));
        }

        private void LogMessage(string message)
        {
            Debug.WriteLine(message);
        }
        #endregion

        private void LogEvent(WorkspaceChangeEventArgs e, Solution oldSolution = null, Solution newSolution = null)
        {
            if (oldSolution != null)
            {
                LogMessage(string.Format("Event: {1} | Solution: {0}", oldSolution.Id.ToString(), e.Kind));
            }
        }

        private void LogEvent(WorkspaceChangeEventArgs e, ProjectId id)
        {
            LogMessage(string.Format("Event: {1} | Project: {0}", id.Id.ToString(), e.Kind));
        }

        private void LogEvent(WorkspaceChangeEventArgs e, DocumentId id)
        {
            LogMessage(string.Format("Event: {1} | Document: {0}", id.Id.ToString(), e.Kind));
        }

        #region Workspace Event Handlers
        private void Workspace_Document(object sender, WorkspaceChangeEventArgs e)
        {
            LogEvent(e, e.DocumentId);
            UpdateWorkspaceTreeNodes();
        }

        private void Workspace_Project(object sender, WorkspaceChangeEventArgs e)
        {
            LogEvent(e, e.ProjectId);
            UpdateWorkspaceTreeNodes();
        }

        private void Workspace_Solution(object sender, WorkspaceChangeEventArgs e)
        {
            LogEvent(e, e.OldSolution, e.NewSolution);
            UpdateWorkspaceTreeNodes();
        }

        private void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            LogError(string.Format("{0} | {1}", e.Diagnostic.Kind, e.Diagnostic.Message));
        }

        private void Workspace_DocumentOpened(object sender, DocumentEventArgs e)
        {
            if (e != null && e.Document is Document)
            {
                var doc = e.Document;

                // Add button to the FileControlPanel
                var button = new Button();

                // TODO: don't hard code styling values here
                button.Text = doc.Name;
                button.Width = 75;
                button.Height = 30;
                button.Tag = doc.Id;
                //button.ContextMenu = 

                // TODO: Change OpenDocument to ActivateDocument (after it's created/implemented)
                button.Click += (_sender, _e) => { Workspace.OpenDocument(doc.Id); }; 

                FileControlPanel.Controls.Add(button);
            }

            // Obtain text from document asynchronously (there is no synchronous method), then update TextEditor box
            e.Document.GetTextAsync().ContinueWith(t =>
            {
                TextEditor.Text = t.Result.ToString();
            }, TaskScheduler.FromCurrentSynchronizationContext()); // Task must be run on the UI thread
        }

        private void Workspace_DocumentClosed(object sender, DocumentEventArgs e)
        {
            if (e != null && e.Document is Document)
            {
                // Remove button in FileControlPanel that correspondes to this document
                var result = FileControlPanel.Controls.OfType<Button>().Where(b => b.Tag as DocumentId == e.Document.Id).FirstOrDefault();

                if (!string.IsNullOrEmpty(result.Text))
                {
                    FileControlPanel.Controls.Remove(result);
                }

                // Clear TextEditor out (or activate a different document)
                TextEditor.Clear();
                TextEditor.ClearUndo();
            }
        }
        #endregion

        /// <summary>
        /// Update the Solution Explorer TreeView Control based on the current solution
        /// </summary>
        private void UpdateWorkspaceTreeNodes()
        {
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
                    child.ContextMenuStrip = DocumentContextMenu;
                    node.Nodes.Add(child);
                }

                root.Nodes.Add(node);
            }

            WorkspaceTreeView.Nodes.Clear();
            WorkspaceTreeView.Nodes.Add(root);
            WorkspaceTreeView.ExpandAll(); // this probably needs to changed for more complicated solutions
        }

        private void Editor_TextChanged(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Start();

            parser.Text = SourceText.From(TextEditor.Text);

            if (Workspace.CurrentDocument != null)
            {
//                Workspace.CurrentDocument.



                // Highlight syntax in document using text from the UI (not the file)
                parser.UpdateTree(Workspace.CurrentDocument.WithText(SourceText.From(TextEditor.Text)));
            }
            else
            {
                // TODO: Unintrusively notify user that they are typing in an empty buffer that isn't associated with a file
            }
        }

        private void DisplayCompilerResults()
        {
            DisplayCompilerResults(Workspace.Evaluate());
        }

        private void DisplayCompilerResults(ImmutableArray<Diagnostic> results)
        {
            var result = from r in results
                         select new
                         {
                             Message = r.GetMessage(),
                             Id = r.Id,
                             Category = r.Category,
                             Kind = r.Location.Kind,
                             Start = r.Location.SourceSpan.Start,
                             End = r.Location.SourceSpan.End,
                             Length = r.Location.SourceSpan.Length,
                             File = r.Location.SourceTree != null ? new FileInfo(r.Location.SourceTree.FilePath).Name : "",
                             StartLine = r.Location.SourceTree != null ? r.Location.SourceTree.GetLineSpan(r.Location.SourceSpan).StartLinePosition.Line : -1,
                             StartChar = r.Location.SourceTree != null ? r.Location.SourceTree.GetLineSpan(r.Location.SourceSpan).StartLinePosition.Character : -1,
                             EndLine = r.Location.SourceTree != null ? r.Location.SourceTree.GetLineSpan(r.Location.SourceSpan).EndLinePosition.Line : -1,
                             EndChar = r.Location.SourceTree != null ? r.Location.SourceTree.GetLineSpan(r.Location.SourceSpan).EndLinePosition.Character : -1,
                         };

            var bs = new BindingSource();
            bs.DataSource = result.ToArray();

            CompilerResults.DataSource = bs;
        }

        private void NormalizeWhitespace_Click(object sender, EventArgs e)
        {
            TextEditor.Text = Parser.NormalizeWhitespace(TextEditor.Text).ToString();

            // Update Document (but not the file it corresponds to)
            // Workspace.CurrentDocument.

            // TODO: position the cursor correctly, instead of letting it go to line 0, char 0
        }

        private void Parser_HighlighterUpdated(object sender, HighlighterEventArgs e)
        {
            // Remove event handler, as this method will change the text, thus causing another highlight pass
            TextEditor.TextChanged -= Editor_TextChanged;

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
            TextEditor.SelectionColor = System.Drawing.Color.White;

            // Restore event handler
            TextEditor.TextChanged += Editor_TextChanged;
        }

        private void OpenIDESolutionFile_Click(object sender, EventArgs e)
        {
            OpenFile("IDE Solutions | *.dusln", t => Workspace.OpenSolution(t));
        }

        private void OpenIDEProjectFile_Click(object sender, EventArgs e)
        {
            OpenFile("IDE Project Files | *.duproj", t => Workspace.OpenProject(t));
        }

        /* Visual Studio files aren't yet supported

        private void OpenVSSolutionFile_Click(object sender, EventArgs e)
        {
            OpenFile("Visual Studio Solutions | *.sln");
        }

        private void OpenVSProjectFile_Click(object sender, EventArgs e)
        {
            OpenFile("C# Project Files | *.csproj");
        }*/

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
            //Workspace.has

            Close();
        }

        private void CloseAllDocuments_Click(object sender, EventArgs e)
        {
            if (Workspace.CurrentSolution != null && Workspace.GetOpenDocumentIds().Count() > 0)
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
        }

        private void SolutionExplorer_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var docId = e.Node.Tag as DocumentId;

            // Node is a document
            if (docId != null)
            {
                // Opens a document, and loads the contents of a document in the TextEditor via event handling
                Workspace.OpenDocument(docId);
            }
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
    }
}
