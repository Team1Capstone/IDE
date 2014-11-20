using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Core.Workspace
{
    public class Document
    {
        // Identifier
        public int DocumentId { get; private set; }

        // Path of Document
        public string path;

        SyntaxTree Tree;
        
        void LoadTree() {
            var ms = new MemoryStream();
            var fs = File.OpenRead(path);

            fs.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(ms);

            Tree = SyntaxFactory.ParseSyntaxTree(reader.ReadToEnd());
        }

        void Save() { }
        void Save(string path) { }
        void Load() { }
        void Rename() { }
        void Delete() { }

    }
}
