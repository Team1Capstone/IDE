using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core.SyntaxRewriters
{
    /// <summary>
    /// This rewriter browses the syntax tree and corrects indentation, and remove unnecessary trailing whitespace
    /// </summary>
    public class FormattingWriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel Model;

        public uint TabSize { get; set; }

        public FormattingWriter(SemanticModel model)
        {
            TabSize = 2;
            Model = model;
        }

        // TODO: identify where indentation should be added/removed

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            return base.VisitTrivia(trivia);
        }

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            return base.VisitNamespaceDeclaration(node);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            return base.VisitClassDeclaration(node);
        }
    }
}
