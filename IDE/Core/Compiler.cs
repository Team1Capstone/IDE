using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Core
{
    public class Compiler
    {
        public OutputKind Output { get; set; }

        public void Compile()
        {
            if(Output == OutputKind.ConsoleApplication || Output == OutputKind.DynamicallyLinkedLibrary)
            {
                var result = Compile(Output);
            }
        }

        private CSharpCompilation Compile(OutputKind kind)
        {
            // Files and references must be passed before compilation

            SyntaxTree[] sourceTrees = new SyntaxTree[] { };
            MetadataReference[] references = new MetadataReference[] { };

            return CSharpCompilation.Create("Test", sourceTrees, references, new CSharpCompilationOptions(kind));
        }
    }
}
