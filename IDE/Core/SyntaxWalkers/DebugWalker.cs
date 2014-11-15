using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Core.SyntaxWalkers
{
    public class DebugWalker : CSharpSyntaxWalker
    {
        public override void Visit(SyntaxNode node)
        {
            var location = node.GetLocation();
            var children = node.ChildNodes();
            var nodes = node.DescendantNodes();
            var tokens = node.DescendantTokens();
            //node.

            Debug.WriteLine("Kind: {0}", node.CSharpKind());
            Debug.WriteLine("Child Nodes: {0}", children.Count());
            Debug.WriteLine("Descendant Nodes: {0}", nodes.Count());
            Debug.WriteLine("Descendant Tokens: {0}", tokens.Count());


            Debug.WriteLine("FullSpan Start {0}", node.FullSpan.Start);
            Debug.WriteLine("FullSpan End {0}", node.FullSpan.End);
            Debug.WriteLine("FullSpan Length {0}", node.FullSpan.Length);
            Debug.WriteLine("Span Start {0}", node.Span.Start);
            Debug.WriteLine("Span End {0}", node.Span.End);
            Debug.WriteLine("Span Length {0}", node.Span.Length);

            //base.Visit(node);
        }
    }
}
