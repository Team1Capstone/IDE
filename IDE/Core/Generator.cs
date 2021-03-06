﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core
{
    /// <summary>
    /// This class provides code generation capabilities
    /// </summary>
    public static class Generator
    {
        public static SyntaxNode Type(string name, TypeKind type)
        {
            switch (type)
            {
                case TypeKind.Class:
                    return Class(name);
                case TypeKind.Interface:
                    return Interface(name);
                case TypeKind.Struct:
                    return Struct(name);
                case TypeKind.Enum:
                    return Enum(name);
                default:
                    throw new NotSupportedException("Type: " + type + " is not supported");
            }
        }

        public static SyntaxNode Namespace(string name)
        {
            return SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(name));
        }

        /// <summary>
        /// Create a ClassDeclaration node with a specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SyntaxNode Class(string name)
        {
            return SyntaxFactory.ClassDeclaration(name)
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        new SyntaxToken[] {
                            SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                        }
                    ));
        }

        /// <summary>
        /// Create a ClassDeclaration node with a specified node, and type to inherit from
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static SyntaxNode Class(string name, string parent)
        {
            throw new NotSupportedException();

            /*return SyntaxFactory.ClassDeclaration(name)
                            .WithBaseList(SyntaxFactory.BaseList(SyntaxFactory.SeparatedList<TypeSyntax>()));*/
        }

        /// <summary>
        /// Creates a ClassDeclaration node with a specified node, a type to inherit from, and a list of interfaces to implement
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="interfaces"></param>
        /// <returns></returns>
        public static SyntaxNode Class(string name, string parent, IEnumerable<string> interfaces)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates an InterfaceDeclaration node with a specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SyntaxNode Interface(string name)
        {
            return SyntaxFactory.InterfaceDeclaration(name)
                .WithModifiers(
                    SyntaxFactory.TokenList(new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) }));
        }

        public static SyntaxNode Enum(string name)
        {
            return SyntaxFactory.EnumDeclaration(name);
        }

        public static SyntaxNode Enum(string name, IEnumerable<string> values)
        {
            throw new NotSupportedException();

            //return Enum(name);
        }
        
        public static SyntaxNode Struct(string name)
        {
            return SyntaxFactory.StructDeclaration(name);
        }

        public static SyntaxNode Delegate(string name)
        {
            // return SyntaxFactory.DelegateDeclaration
            throw new NotSupportedException();
        }

        public static SyntaxNode Delegate(string name, string type)
        {
            throw new NotSupportedException();
        }

        // class-level stuff

        public static SyntaxNode Parameters()
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


        public static void Parameters(IEnumerable<string> types) { }
        public static void Array(string type) { }
        // member-level stuff

        public static void If() { }
        public static void While() { }
        public static void For() { }
        public static void ForEach() { }
    }
}
