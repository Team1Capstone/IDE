using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Core.Workspace
{
    public static class ProjectExtension
    {
        public static CSharpCompilation Compile(this Project project, string assemblyName, IEnumerable<SyntaxTree> trees = null, IEnumerable<MetadataReference> references = null, CSharpCompilationOptions options = null)
        {
            var Compilation = CSharpCompilation
                .Create(assemblyName)
                .AddSyntaxTrees(trees)
                .AddReferences(MetadataReference.CreateFromAssembly(typeof(object).Assembly))
                .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            return Compilation;
        }

        public static EmitResult Emit(this Project project, Compilation compilation, string outputFile)
        {
            return compilation.Emit(outputFile);
        }
    }
}