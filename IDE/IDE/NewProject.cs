using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Core.Workspace;

namespace IDE
{
    public partial class NewProjectDialog : Form
    {
        public event EventHandler SolutionCreated;
        public event EventHandler ProjectCreated;

        public new string Name { get; private set; }
        public string SolutionPath { get; set; }
        public bool IsValid { get; private set; }

        public NewProjectDialog()
        {
            InitializeComponent();

            IsValid = false;
        }

        private void ProjectName_TextChanged(object sender, EventArgs e)
        {
            ProjectPath.Text = CoreWorkspace.ProjectDirectory + @"\" + ProjectName.Text;

            var message = string.Empty;

            if (string.IsNullOrEmpty(ProjectName.Text))
            {
                message = "Name cannot be empty";
            }
            else if (ProjectName.Text.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
            {
                message = "Invalid File Name Characters";
            }
            else if (ProjectName.Text.IndexOfAny(Path.GetInvalidPathChars()) > 0)
            {
                message = "Invalid Path Characters";
            }
            else
            {
                var di = new DirectoryInfo(CoreWorkspace.ProjectDirectory).EnumerateDirectories("*", SearchOption.TopDirectoryOnly);

                if (di.Any(d => d.FullName.ToUpperInvariant().Equals(ProjectPath.Text.ToUpperInvariant())))
                {
                    message = "Project already exists";
                }
            }

            if (string.IsNullOrEmpty(message))
            {
                DialogErrorProvider.Clear();
                CreateButton.Enabled = true;
                IsValid = true;
            }
            else
            {
                DialogErrorProvider.SetError(ProjectName, message);
                CreateButton.Enabled = false;
                IsValid = false;
            }
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                if (ProjectCreated != null)
                {
                    ProjectCreated(this, new EventArgs());
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
