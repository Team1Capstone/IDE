using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

using Core.Workspace;
using Core.Text;

namespace Core
{
    public class Parser
    {
        public delegate void HighlighterUpdatedHandler(object sender, HighlighterEventArgs e);

        public event EventHandler TreeChanged;
        public event HighlighterUpdatedHandler HighlighterUpdated;

        private ClassificationHighlighter newHighlighter;

        public SyntaxTree tree;

        public Parser()
        {
            newHighlighter = new ClassificationHighlighter();
            EnableHighlighting = true;
        }

        public TextSpan Span { get; set; }

        public bool EnableHighlighting { get; set; }

        protected virtual void OnTreeChanged(EventArgs e)
        {
            if(TreeChanged != null)
            {
                TreeChanged(this, e);
            }
        }

        protected virtual void OnHighlighterUpdated(HighlighterEventArgs e)
        {
            if(HighlighterUpdated != null)
            {
                HighlighterUpdated(this, e);
            }
        }

        public void TextChanged(object sender, EventArgs e)
        {
            var TextEditor = sender as RichTextBox;


            UpdateTree(TextEditor.Text);
        }

        public void SelectionChanged(object sender, EventArgs e)
        {
            var TextEditor = sender as RichTextBox;

            Span = new TextSpan(TextEditor.SelectionStart, TextEditor.SelectionLength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void UpdateTree(string text)
        {
            var newTree = CSharpSyntaxTree.ParseText(text);

            if (tree != null)
            {
                tree = newTree;

                // This would be much easier if there was a way to determine which node the caret is positioned in           
                // Find the descendant node in the new tree which needs to be highlighted

                // Some changes won't require a highlighter pass (e.g. spaces (outside of string literals), newlines, some deletions)
                // When a either a semicolon token is added, or when a closing brace is added that node should be normalized

                // Steps
                // 1. Determine the descendant that holds the selected text (1 character to many), as we don't want to highlight tokens that aren't changing
                // 2. Visit that node using the Highlight class
                // 2. - OR - compare the two trees to determine which tokens have changed
                // 3. Return changes (contains color and position information) to the MainWindow

                OnTreeChanged(new EventArgs());
            }
            else
            {
                tree = newTree;

                // Trigger Event
                OnTreeChanged(new EventArgs());
            }

            if (EnableHighlighting)
            {
                // TODO retrieve active document
                newHighlighter.Format(null, tree.GetText()).Wait();

                var e = new HighlighterEventArgs();

                if(newHighlighter.Changes.Count > 0)
                {
                    OnHighlighterUpdated(e);

                    newHighlighter.Changes.Clear();
                }
            }
        }

        /// <summary>
        /// Normalizes whitespace using predefined rules
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static SyntaxNode NormalizeWhitespace(string text)
        {
            return CSharpSyntaxTree.ParseText(text).GetRoot().NormalizeWhitespace();
        }
    }
}