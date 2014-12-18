namespace IDE
{
    partial class NewProjectDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.CreateButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ProjectPathBox = new System.Windows.Forms.TextBox();
            this.ProjectNameBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.ConsoleRadioButton = new System.Windows.Forms.RadioButton();
            this.LibraryRadioButton = new System.Windows.Forms.RadioButton();
            this.DialogErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.AddToCB = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.DialogErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // CreateButton
            // 
            this.CreateButton.Enabled = false;
            this.CreateButton.Location = new System.Drawing.Point(155, 321);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(75, 23);
            this.CreateButton.TabIndex = 0;
            this.CreateButton.Text = "Create";
            this.CreateButton.UseVisualStyleBackColor = true;
            this.CreateButton.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 254);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Project Name:";
            // 
            // ProjectPathBox
            // 
            this.ProjectPathBox.Location = new System.Drawing.Point(105, 276);
            this.ProjectPathBox.Name = "ProjectPathBox";
            this.ProjectPathBox.ReadOnly = true;
            this.ProjectPathBox.Size = new System.Drawing.Size(300, 20);
            this.ProjectPathBox.TabIndex = 2;
            // 
            // ProjectNameBox
            // 
            this.ProjectNameBox.Location = new System.Drawing.Point(105, 250);
            this.ProjectNameBox.Name = "ProjectNameBox";
            this.ProjectNameBox.Size = new System.Drawing.Size(300, 20);
            this.ProjectNameBox.TabIndex = 4;
            this.ProjectNameBox.TextChanged += new System.EventHandler(this.ProjectName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(73, 276);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Path:";
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Location = new System.Drawing.Point(249, 321);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_Button.TabIndex = 6;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            // 
            // ConsoleRadioButton
            // 
            this.ConsoleRadioButton.AutoSize = true;
            this.ConsoleRadioButton.Checked = true;
            this.ConsoleRadioButton.Location = new System.Drawing.Point(35, 13);
            this.ConsoleRadioButton.Name = "ConsoleRadioButton";
            this.ConsoleRadioButton.Size = new System.Drawing.Size(118, 17);
            this.ConsoleRadioButton.TabIndex = 7;
            this.ConsoleRadioButton.TabStop = true;
            this.ConsoleRadioButton.Text = "Console Application";
            this.ConsoleRadioButton.UseVisualStyleBackColor = true;
            this.ConsoleRadioButton.CheckedChanged += new System.EventHandler(this.ConsoleRadioButton_CheckedChanged);
            // 
            // LibraryRadioButton
            // 
            this.LibraryRadioButton.AutoSize = true;
            this.LibraryRadioButton.Location = new System.Drawing.Point(35, 50);
            this.LibraryRadioButton.Name = "LibraryRadioButton";
            this.LibraryRadioButton.Size = new System.Drawing.Size(84, 17);
            this.LibraryRadioButton.TabIndex = 8;
            this.LibraryRadioButton.Text = "Class Library";
            this.LibraryRadioButton.UseVisualStyleBackColor = true;
            this.LibraryRadioButton.CheckedChanged += new System.EventHandler(this.LibraryRadioButton_CheckedChanged);
            // 
            // DialogErrorProvider
            // 
            this.DialogErrorProvider.ContainerControl = this;
            // 
            // AddToCB
            // 
            this.AddToCB.AutoSize = true;
            this.AddToCB.Location = new System.Drawing.Point(35, 218);
            this.AddToCB.Name = "AddToCB";
            this.AddToCB.Size = new System.Drawing.Size(135, 17);
            this.AddToCB.TabIndex = 9;
            this.AddToCB.Text = "Add to Current Solution";
            this.AddToCB.UseVisualStyleBackColor = true;
            // 
            // NewProjectDialog
            // 
            this.AcceptButton = this.CreateButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(580, 408);
            this.Controls.Add(this.AddToCB);
            this.Controls.Add(this.LibraryRadioButton);
            this.Controls.Add(this.ConsoleRadioButton);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ProjectNameBox);
            this.Controls.Add(this.ProjectPathBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CreateButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProjectDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Project";
            ((System.ComponentModel.ISupportInitialize)(this.DialogErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CreateButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox ProjectPathBox;
        private System.Windows.Forms.TextBox ProjectNameBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.RadioButton ConsoleRadioButton;
        private System.Windows.Forms.RadioButton LibraryRadioButton;
        private System.Windows.Forms.ErrorProvider DialogErrorProvider;
        private System.Windows.Forms.CheckBox AddToCB;
    }
}