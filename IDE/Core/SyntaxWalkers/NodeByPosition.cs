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
    /// <summary>
    /// Returns the node in which the caret is positioned in
    /// </summary>
    public class NodeByTextPosition : CSharpSyntaxWalker
    {
        public int Position { get; set; }
        public SyntaxNode Node { get; private set; }

        public override void Visit(SyntaxNode node)
        {
            //Debug.WriteLine("Searching for Position: " + this.Position);

            //Debug.WriteLine("Start: " + node.FullSpan.Start);
            //Debug.WriteLine("End: " + node.FullSpan.End);

            // Note: FullSpan contains all of the additional trivia such as leading/trailing whitespace. Span doesn't
            if (node.FullSpan.Start <= Position && node.FullSpan.End >= Position)
            {
                //Debug.WriteLine("Using Current Node");
                Node = node;

                foreach (var _node in node.ChildNodes())
                {
                    //Debug.WriteLine("Visiting Child");
                    Visit(_node);
                }
            }else
            {
                //Debug.WriteLine("Out of Range");
            }

            //Debug.WriteLine("Done");
        }
    }
}
