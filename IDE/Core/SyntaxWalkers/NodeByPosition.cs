using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Core.SyntaxWalkers
{
    /// <summary>
    /// Returns the node in which the caret is positioned in
    /// </summary>
    public class NodeByTextPosition : CSharpSyntaxWalker
    {
        public int Position { get; set; }
        public SyntaxNode Node { get; private set; }

        public override void Visit(SyntaxNode node)
        {
            // Warning: Untested

            // Note: FullSpan contains all of the additional trivia such as leading/trailing whitespace. Span doesn't
            if (node.FullSpan.Start <= Position && node.FullSpan.End >= Position)
            {
                Node = node;

                foreach (var _node in node.ChildNodes())
                {
                    Visit(_node);
                }
            }
        }
    }
}
