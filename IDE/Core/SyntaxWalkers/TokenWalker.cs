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
    /// Traverses through a node and returns a list of descendant tokens
    /// </summary>
    public class TokenWalker : CSharpSyntaxWalker
    {
        public readonly List<SyntaxToken> tokens = new List<SyntaxToken>();

        public override void Visit(SyntaxNode node)
        {
            tokens.AddRange(node.ChildTokens());

            foreach (var _node in node.ChildNodes())
            {
                Visit(_node);
            }
        }
    }
}
