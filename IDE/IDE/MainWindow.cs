using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using Core;
using Core.Workspace;
using Core.SyntaxWalkers;

namespace IDE
{
    public partial class MainWindow : Form
    {
        private Parser parser;
        private Generator generator;
        private Core.Workspace.Workspace Workspace;

        public MainWindow()
        {
            InitializeComponent();

            parser = new Parser();
            generator = new Generator();
            Workspace = new Core.Workspace.Workspace();

            // Event handling methods have been created in specific classes to clear this file up

            // When text is selected the parser needs to know what text is selected
            TextEditor.SelectionChanged += parser.SelectionChanged;

            // Parser maintains a SyntaxTree and needs to be updated when the text changes
            TextEditor.TextChanged += parser.TextChanged;

            parser.HighlighterUpdated += Parser_HighlighterUpdate;

            // Build button
            buildMenuItem.Click += Workspace.CurrentSolution.Build;

            // Run button
            runMenuItem.Click += Workspace.CurrentSolution.Run;

            // Package button
            packageMenuItem.Click += Workspace.CurrentSolution.Package;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create new a file buffer

            // if current solution has multiple projects, ask where to place new file
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
            //Package();


            // Test out serialization of Workspace/Solution/Project

            // Add a solution to the workspace
            Workspace.AddNewSolution("ConsoleApp", OutputKind.ConsoleApplication);
            Workspace.AddNewSolution("TestClass", OutputKind.DynamicallyLinkedLibrary);

            Workspace.WriteTo("test.workspace.xml");
        }
    }
}
