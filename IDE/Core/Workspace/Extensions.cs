using System;
using System.Diagnostics;
using System.IO;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Core.Workspace
{
    public static class Extensions
    {
        public static void Evaluate(this Project project)
        {
            var compilation = project.GetCompilationAsync().Result;

            if (compilation != null)
            {
                var results = compilation.GetDiagnostics();

                Debug.WriteLine("Messages: {0}", results.Length);

                foreach (var result in results)
                {
                    Debug.WriteLine(string.Format("Category: {0}", result.Category));
                    Debug.WriteLine(string.Format("Location: {0}", result.Location.GetLineSpan().Path));
                    Debug.WriteLine(result.Severity);
                    Debug.WriteLine(string.Format("Message: {0}", result.GetMessage()));
                }
            }
            else
            {
                Debug.WriteLine("Compilation not available");
            }
        }

        public static void Evaluate(this Solution solution)
        {
            foreach(var id in solution.ProjectIds)
            {
                solution.GetProject(id).Evaluate();
            }
        }

        public static void Emit(this Solution solution)
        {
            foreach(var id in solution.ProjectIds)
            {
                var project = solution.GetProject(id);
                var compile = project.GetCompilationAsync().Result;
                var dirInfo = new FileInfo(project.OutputFilePath).Directory;

                // Make sure output directory exists
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }

                var result = compile.Emit(project.OutputFilePath);

                Debug.WriteLine("" + (result.Success ? "Success" : "Failure"));
                Debug.WriteLine(string.Format("Errors: {0}", result.Diagnostics.Length));
            }
        }
    }
}