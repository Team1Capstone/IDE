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
using Core.SyntaxWalkers;

namespace IDE
{
    public partial class MainWindow : Form
    {
        private Parser parser;
        private Generator generator;
        private Compiler compiler;

        public MainWindow()
        {
            InitializeComponent();

            parser = new Parser();
            generator = new Generator();
            compiler = new Compiler();

            parser.HighlighterUpdated += Parser_HighlighterUpdate;
        }

        private void Run()
        {
            // Due to the lack of a debugger the program is launched as a normal process, and debugging symbols aren't provided YET
            compiler.Run();
        }

        private void Build()
        {
            // Save files that are part of the solution/project before building

            // These properties should be pulled from a project file, not hard-coded
            compiler.SourceTrees = new[] { parser.tree };
            compiler.Output = OutputKind.ConsoleApplication;
            compiler.ExecutableName = "test.exe";

            compiler.Compile();
        }

        private void Package()
        {

        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create new a file buffer
        }

        private void normalizeWhitespaceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TextEditor.Text = Parser.NormalizeWhitespace(TextEditor.Text).ToString();
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            // Update the Syntax Tree in the parser so it can determine highlighting and any automatic formatting
            parser.UpdateTree(TextEditor.Text);
        }

        private void Parser_HighlighterUpdate(object sender, HighlighterEventArgs e)
        {
            // Store the position of the caret
            int pos = TextEditor.SelectionStart;

            foreach(var change in e.Changes)
            {
                // Select the text that will be highlighted
                TextEditor.Select(change.Span.Start, change.Span.Length);

                // Change the color of the text
                TextEditor.SelectionColor = change.Color;
            }

            // Restore the position of the caret
            TextEditor.Select(pos, 0);
            TextEditor.SelectionColor = Color.White;
        }

        private void TextEditor_SelectionChanged(object sender, EventArgs e)
        {
            parser.Span = new TextSpan(TextEditor.SelectionStart, TextEditor.SelectionLength);
        }

        private void buildToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Build();
        }

        private void runToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void packageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Package();
        }
    }
}
