using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core
{
    /// <summary>
    /// This class provides code generation capabilities
    /// </summary>
    public class Generator
    {
        public SyntaxNode Type(TypeKind type)
        {
            switch (type)
            {
                case TypeKind.Class:
                    return Class();
                case TypeKind.Interface:
                    return Interface();
                case TypeKind.Struct:
                    return Struct();
                case TypeKind.Enum:
                    return Enum();
                default:
                    throw new NotSupportedException("Type: " + type + " is not supported");
            }
        }

        public SyntaxNode Namespace()
        {
            return SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("DefaultNamespace"));
        }

        /// <summary>
        /// Create a ClassDeclaration node with the default name
        /// </summary>
        /// <returns></returns>
        public SyntaxNode Class()
        {
            return Class("name");
        }

        /// <summary>
        /// Create a ClassDeclaration node with a specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SyntaxNode Class(string name)
        {
            return SyntaxFactory.ClassDeclaration(name)
                /*.WithOpenBraceToken(
                    SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(
                    SyntaxFactory.Token(SyntaxKind.CloseBraceToken))*/
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        new SyntaxToken[] {
                            SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                    }));
        }

        /// <summary>
        /// Create a ClassDeclaration node with a specified node, and type to inherit from
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public SyntaxNode Class(string name, string parent)
        {
            throw new NotSupportedException();

            return SyntaxFactory.ClassDeclaration(name)
                            .WithBaseList(SyntaxFactory.BaseList(SyntaxFactory.SeparatedList<TypeSyntax>()));
        }

        /// <summary>
        /// Creates a ClassDeclaration node with a specified node, a type to inherit from, and a list of interfaces to implement
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="interfaces"></param>
        /// <returns></returns>
        public SyntaxNode Class(string name, string parent, IEnumerable<string> interfaces)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates an InterfaceDeclaration node with the default name
        /// </summary>
        /// <returns></returns>
        public SyntaxNode Interface()
        {
            return Interface("name");
        }

        /// <summary>
        /// Creates an InterfaceDeclaration node with a specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SyntaxNode Interface(string name)
        {
            return SyntaxFactory.InterfaceDeclaration(name)
                .WithModifiers(
                    SyntaxFactory.TokenList(new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) }));
        }

        public SyntaxNode Enum()
        {
            return SyntaxFactory.EnumDeclaration("name");
        }

        public SyntaxNode Enum(string name)
        {
            return SyntaxFactory.EnumDeclaration(name);
        }
        
        public SyntaxNode Struct()
        {
            return SyntaxFactory.StructDeclaration("name");
        }


        // class-level stuff

        public SyntaxNode Parameters()
        {
            var list = new SeparatedSyntaxList<ParameterSyntax>();

            // Incomplete. Testing this out still
            list.Add(SyntaxFactory.Parameter(SyntaxFactory.Identifier("test")));
            /*            list.Add(SyntaxFactory.Parameter(new SyntaxList<AttributeListSyntax>(),
                            new SyntaxTokenList(),
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.PredefinedType))
                            ,
                            SyntaxFactory.EqualsValueClause(SyntaxFactory.DefaultExpression(
                              )
                        );*/

                // this returns an empty parameter: "()"
            return SyntaxFactory.ParameterList(
                SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                list,
                SyntaxFactory.Token(SyntaxKind.CloseParenToken));
        }

        // member-level stuff

        public void If() { }
        public void While() { }
        public void For() { }
        public void ForEach() { }
    }
}
