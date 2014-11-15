using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Core.Text;

namespace Core.SyntaxWalkers
{
    /// <summary>
    /// 
    /// </summary>
    public class Highlighter : CSharpSyntaxWalker
    {
        public Color DefaultColor { get; protected set; }
        public Color DefaultBackgroundColor { get; protected set; }
        public readonly Dictionary<SyntaxKind, Color> Map;
        public readonly List<ColorTextSpan> Changes;

        public Highlighter()
        {
            // TODO: Load the map from an external source, and not have the colors hard-coded since there is a lot of different tokens
            Map = new Dictionary<SyntaxKind, Color>();
            Changes = new List<ColorTextSpan>();

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

            // Predefined types
            var predefinedTypes = new[] {
                SyntaxKind.IntKeyword,
                SyntaxKind.UIntKeyword,
                SyntaxKind.ShortKeyword,
                SyntaxKind.UShortKeyword,
                SyntaxKind.LongKeyword,
                SyntaxKind.ULongKeyword,
                SyntaxKind.ByteKeyword,
                SyntaxKind.SByteKeyword,
                SyntaxKind.ObjectKeyword,
                SyntaxKind.NullKeyword
            };

            SetColor(predefinedTypes, Color.DodgerBlue);

            Map.Add(SyntaxKind.IdentifierToken, Color.CadetBlue);
        }
        
        public void SetColor(IEnumerable<SyntaxKind> kinds, Color color)
        {
            foreach(var kind in kinds)
            {
                if (Map.ContainsKey(kind))
                {
                    Map[kind] = color;
                }else
                {
                    Map.Add(kind, color);
                }
            }
        }

        public override void Visit(SyntaxNode node)
        {
            var tokens = node.DescendantTokens();
            SyntaxKind kind;

            foreach (var token in tokens)
            {
                kind = token.CSharpKind();

                if (!token.FullSpan.IsEmpty && Map.ContainsKey(kind))
                {
                    Debug.WriteLine(kind + " is " + Map[kind].ToString());

                    Changes.Add(new ColorTextSpan()
                    {
                        Color = Map[kind],
                        Span = token.FullSpan
                    });
                }
            }
        }
    }
}
