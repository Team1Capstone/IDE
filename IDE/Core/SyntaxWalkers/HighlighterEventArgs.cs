using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Text;

namespace Core.SyntaxWalkers
{
    public class HighlighterEventArgs : EventArgs
    {
        public List<ColorTextSpan> Changes;

        public HighlighterEventArgs() {
            Changes = new List<ColorTextSpan>();
        }

        public HighlighterEventArgs(IEnumerable<ColorTextSpan> changes) : this()
        {
            Changes.AddRange(changes);
        }
    }
}
