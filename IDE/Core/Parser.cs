using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
//using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace Core
{
    public class Parser
    {
        private SyntaxTree tree;
        private SourceText source;

        // Incomplete. Testing this out
        public void StateChange(string text)
        {
            bool IsTreeRefreshRequired = false;

            // Parse Text into a SyntaxTree
            var newTree = CSharpSyntaxTree.ParseText(text);

            if (tree != null)
            {
                // Get the changes between trees
                var changes = newTree.GetChanges(tree);
                var spans = newTree.GetChangedSpans(tree);

                // Where are the changes at? Do any nodes need to be passed over with the highlighter?

                Debug.WriteLine(string.Format("{0} Changes", changes.Count));
                Debug.WriteLine("{0} Spans", spans.Count);

                foreach (var change in changes)
                {
                    Debug.WriteLine(string.Format("Change [{0}:{1}] {2}", change.Span.Start, change.Span.End, change.NewText));
                }

                foreach (var span in spans)
                {
                    Debug.WriteLine(string.Format("{0}{1}", span.Start, span.End));
                }
            }
            else
            {
                // Always change if root tree is null. Only happens on the first write
                IsTreeRefreshRequired = true;
            }

            if (IsTreeRefreshRequired)
            {
                tree = newTree;
            }
        }

        public static SyntaxNode NormalizeWhitespace(string text)
        {
            return CSharpSyntaxTree.ParseText(text).GetRoot().NormalizeWhitespace();
        }


        /*public void ParseSyntax(RichTextBox rtb)
        {
            NodeByPositionWalker walker = new NodeByPositionWalker();

            rtb.Text = ParseSyntax(rtb.Text);

            walker.Visit(tree.GetRoot());

            //SyntaxHighlight(rtb);
        }*/

        public string ParseSyntax(string text)
        {
            tree = CSharpSyntaxTree.ParseText(text);
            source = SourceText.From(text);

            return tree.ToString();
        }

        /*private void SyntaxHighlight(RichTextBox rtb)
        {
            SyntaxTokenHighlight highlighter = new SyntaxTokenHighlight();
            TokenWalker collector = new TokenWalker();

            collector.Visit(tree.GetRoot());

            foreach (var token in collector.tokens)
            {
                if (highlighter.Map.ContainsKey(token.CSharpKind()))
                {
                    //Console.WriteLine("Coloring {2} Index: {0} Length: {1}", token.Span.Start, token.Span.Length, token.CSharpKind());

                    rtb.Select(token.Span.Start, token.Span.Length);
                    rtb.SelectionColor = highlighter.Map[token.CSharpKind()];
                }
            }
        }*/
    }
}
