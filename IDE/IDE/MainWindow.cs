using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Core;

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
            // Calls NormalizeWhitespace on the root node of the SyntaxTree
            TextEditor.Text = Parser.NormalizeWhitespace(TextEditor.Text).ToString();
        }
    }
}
