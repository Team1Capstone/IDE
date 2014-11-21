using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

using Core.Workspace;

namespace Core.Text
{
    public class ClassificationHighlighter
    {
        public Color DefaultColor { get; protected set; }
        public Color DefaultBackgroundColor { get; protected set; }
        public readonly Dictionary<string, Color> Map;
        public readonly List<ColorTextSpan> Changes;

        public ClassificationHighlighter()
        {
            Map = new Dictionary<string, Color>();
            DefaultColor = Color.White;
            DefaultBackgroundColor = Color.FromArgb(32, 32, 32);

            // Use reflection to get the string values of all the constants in ClassificationTypeNames
            var constants = typeof(ClassificationTypeNames).GetFields(
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();

            foreach(var str in constants)
            {
                Map.Add((string)str.GetValue(null), DefaultColor);
            }

            Map[ClassificationTypeNames.ClassName] = Color.DodgerBlue;
            Map[ClassificationTypeNames.InterfaceName] = Color.Olive;
            Map[ClassificationTypeNames.Comment] = Color.LawnGreen;
        }

        // Adapted from: https://roslyn.codeplex.com/SourceControl/latest#Src/Samples/CSharp/ConsoleClassifier/Program.cs
        #region Derivative Work
        public async Task Format(Document document, SourceText text)
        {
            if(document == null)
            {
                return;
            }

            var workspace = document.Project.Solution.Workspace;
            var solution = workspace.CurrentSolution;
            var project = document.Project;
            
            document = await Microsoft.CodeAnalysis.Formatting.Formatter.FormatAsync(document);

            var cSpans = await Classifier.GetClassifiedSpansAsync(document, TextSpan.FromBounds(0, text.Length));

            var ranges =  cSpans.Select(c => new Range(c, text.GetSubText(c.TextSpan).ToString()));

            ranges = FillGaps(text, ranges);

            foreach(var range in ranges)
            {
                switch (range.ClassificationType)
                {
                    case "class name":
                        break;
                    case "interface name":
                        break;
                    default:
                        break;
                }
            }
        }

        IEnumerable<Range> FillGaps(SourceText text, IEnumerable<Range> ranges)
        {
            const string wsc = null;
            int current = 0;
            Range previous = null;

            foreach (Range range in ranges)
            {
                int start = range.TextSpan.Start;

                if (start > current)
                {
                    yield return new Range(wsc, TextSpan.FromBounds(current, start), text);
                }

                if (previous == null || range.TextSpan != previous.TextSpan)
                {
                    yield return range;
                }

                previous = range;
                current = range.TextSpan.End;
            }

            if (current < text.Length)
            {
                yield return new Range(wsc, TextSpan.FromBounds(current, text.Length), text);
            }

        }
        #endregion
    }
}
