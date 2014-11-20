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

using Core.SyntaxWalkers;
using Core.SyntaxRewriters;

namespace Core
{
    public class Parser
    {
        public delegate void HighlighterUpdatedHandler(object sender, HighlighterEventArgs e);

        private Highlighter highlighter;
        public SyntaxTree tree;

        public Parser()
        {
            highlighter = new Highlighter();
            EnableHighlighting = true;
        }

        public TextSpan Span { get; set; }

        public bool EnableHighlighting { get; set; }

        public event EventHandler TreeChanged;

        public event HighlighterUpdatedHandler HighlighterUpdated;

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
                highlighter.Visit(tree.GetRoot()); // this should changed to the node that contains the tokens being changed instead of the entire tree

                if (highlighter.Changes.Count > 0)
                {
                    // Trigger Event
                    OnHighlighterUpdated(new HighlighterEventArgs(highlighter.Changes));

                    highlighter.Changes.Clear();
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
