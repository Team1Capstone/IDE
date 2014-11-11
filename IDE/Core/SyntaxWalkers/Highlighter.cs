using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Core.SyntaxWalkers
{
    /// <summary>
    /// 
    /// </summary>
    public class Highlighter : CSharpSyntaxWalker
    {
        public readonly Dictionary<SyntaxKind, Color> Map;
        
        // TODO: return details about any changes that need to be highlighted. Strings are immutable, so it is important to do any string operations as fast as possible (i.e. StringBuilder)

        public Highlighter()
        {
            // TODO: Load the map from an external source, and not have the colors hard-coded since there is a lot of different tokens
            Map = new Dictionary<SyntaxKind, Color>();

            Action<IEnumerable<SyntaxKind>, Color> AddToMap = (kinds, color) =>
            {
                foreach (var kind in kinds)
                {
                    Map.Add(kind, color);
                }
            };

            // Note: Sorry I have such bad choices in colors!
            AddToMap(SyntaxFacts.GetKeywordKinds(), Color.Orange);
            AddToMap(SyntaxFacts.GetPunctuationKinds(), Color.White);

            // Comments
            Map.Add(SyntaxKind.MultiLineCommentTrivia, Color.DarkGreen);
            Map.Add(SyntaxKind.MultiLineDocumentationCommentTrivia, Color.LawnGreen);
            Map.Add(SyntaxKind.SingleLineCommentTrivia, Color.LawnGreen);
            Map.Add(SyntaxKind.SingleLineDocumentationCommentTrivia, Color.LawnGreen);

            Map.Add(SyntaxKind.IdentifierName, Color.White);

            /* Punctuation Kinds

            Map.Add(SyntaxKind.OpenBraceToken, Color.White);
            Map.Add(SyntaxKind.CloseBraceToken, Color.White);
            Map.Add(SyntaxKind.OpenParenToken, Color.White);
            Map.Add(SyntaxKind.CloseParenToken, Color.White);
            Map.Add(SyntaxKind.OpenBracketToken, Color.White);
            Map.Add(SyntaxKind.CloseBracketToken, Color.White);*/

            // Keywords
            /*            Map.Add(SyntaxKind.NamespaceKeyword, Color.Turquoise);
                        Map.Add(SyntaxKind.PropertyKeyword, Color.LawnGreen);
                        Map.Add(SyntaxKind.MethodKeyword, Color.LawnGreen);
                        Map.Add(SyntaxKind.ClassKeyword, Color.Turquoise);
                        Map.Add(SyntaxKind.PublicKeyword, Color.Orange);
                        Map.Add(SyntaxKind.ProtectedKeyword, Color.Orange);
                        Map.Add(SyntaxKind.PrivateKeyword, Color.Orange);*/
        }

        /// <summary>
        /// Traverses tokens and trivia of a node, and recursively calls child node
        /// </summary>
        /// <param name="node">The node being highlighted</param>
        public override void Visit(SyntaxNode node)
        {
            // Browse all tokens and determine which ones should be colored
            node.ChildTokens(); // incomplete

            // Recursively call child nodes (this would probably be better off as an iterative loop)
            foreach(var _node in node.ChildNodes())
            {
                Visit(_node);
            }
        }
    }
}
