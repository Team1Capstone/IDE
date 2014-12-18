using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.CodeAnalysis;

using Core.Workspace;

namespace IDE
{
    public partial class NewProjectDialog : Form
    {
        public event EventHandler SolutionDirectoryCreated;
        public event EventHandler ProjectDirectoryCreated;
        public event EventHandler SolutionCreateFailed;
        public event EventHandler ProjectCreateFailed;

        public string SolutionPath { get; private set; }
        public string ProjectPath { get; private set; }
        public string ProjectName { get; private set; }
        public OutputKind Kind { get; private set; }

        public bool IsValid { get; private set; }

        public NewProjectDialog()
        {
            InitializeComponent();

            IsValid = false;
            ProjectPathBox.Text = CoreWorkspace.ProjectDirectory;
        }

        private void ProjectName_TextChanged(object sender, EventArgs e)
        {
            ProjectPathBox.Text = CoreWorkspace.ProjectDirectory + @"\" + ProjectNameBox.Text;

            var message = string.Empty;

            if (string.IsNullOrEmpty(ProjectNameBox.Text))
            {
                message = "Name cannot be empty";
            }
            else if (ProjectNameBox.Text.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
            {
                message = "Invalid File Name Characters";
            }
            else if (ProjectNameBox.Text.IndexOfAny(Path.GetInvalidPathChars()) > 0)
            {
                message = "Invalid Path Characters";
            }
            else
            {
                var di = new DirectoryInfo(CoreWorkspace.ProjectDirectory).EnumerateDirectories("*", SearchOption.TopDirectoryOnly);

                if (di.Any(d => d.FullName.ToUpperInvariant().Equals(ProjectPathBox.Text.ToUpperInvariant())))
                {
                    message = "Project already exists";
                }
            }

            if (string.IsNullOrEmpty(message))
            {
                DialogErrorProvider.Clear();
                CreateButton.Enabled = true;
                IsValid = true;
                ProjectName = ProjectNameBox.Text;
            }
            else
            {
                DialogErrorProvider.SetError(ProjectNameBox, message);
                CreateButton.Enabled = false;
                IsValid = false;
                ProjectName = string.Empty;
            }
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            if (IsValid)
            {
                var dirInfo = new DirectoryInfo(ProjectPathBox.Text);

                if (!dirInfo.Exists)
                {
                    try
                    {
                        // Create solution directory
                        dirInfo.Create();

                        SolutionPath = ProjectPathBox.Text;

                        if (SolutionDirectoryCreated != null)
                        {
                            SolutionDirectoryCreated(this, new EventArgs());
                        }

                        // Create project directory
                        dirInfo.CreateSubdirectory(ProjectNameBox.Text);

                        ProjectPath = Path.Combine(SolutionPath, ProjectNameBox.Text);

                        if (ProjectDirectoryCreated != null)
                        {
                            ProjectDirectoryCreated(this, new EventArgs());
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        throw;
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Hide();
                }
                else
                {
                    throw new Exception("Solution already exists");
                }
            }
        }

        private void LibraryRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (LibraryRadioButton.Checked)
            {
                Kind = OutputKind.DynamicallyLinkedLibrary;
            }
        }

        private void ConsoleRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ConsoleRadioButton.Checked)
            {
                Kind = OutputKind.ConsoleApplication;
            }
        }
    }
}
