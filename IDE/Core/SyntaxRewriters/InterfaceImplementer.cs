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
    /// Writes out the syntax required for an interface. Only classes implement interfaces, but interfaces can inherit from other interfaces
    /// </summary>
    public class InterfaceImplementer : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // TODO: determine the interfaces implemented and what they require

            return base.VisitClassDeclaration(node);
        }
    }
}
