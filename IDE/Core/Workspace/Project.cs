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
    [DataContract(Name = "Project", Namespace = "http://davenport.edu")]
    public class Project
    {
        public event EventHandler ParsingSucceeded;
        public event EventHandler ParsingFailed;
        public event EventHandler EmitSucceeded;
        public event EventHandler EmitFailed;

        public virtual void OnEmitSucceeded(EventArgs e)
        {
            if(EmitSucceeded != null)
            {
                EmitSucceeded(this, e);
            }
        }

        public virtual void OnEmitFailed(EventArgs e)
        {
            if(EmitFailed != null)
            {
                EmitFailed(this, e);
            }
        }

        [DataMember]
        internal ProjectId Id;

        [DataMember]
        internal OutputKind Type;

        [DataMember(Name = "Directory")]
        internal string directory;

        [DataMember]
        internal string AssemblyName;

        [DataMember]
        internal string ProjectName;

        [DataMember]
        internal string ProjectNamespace;

        [DataMember]
        internal string OutputFile;

        private Project() {
            //ProjectId = Guid.NewGuid();
        }

        /// <summary>
        /// Create a project of a specific type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="kind"></param>
        internal Project(string name, OutputKind kind) : this()
        {
            Type = kind;
            ProjectName = name;
            AssemblyName = name;
            ProjectNamespace = name;

            if(kind == OutputKind.ConsoleApplication)
            {
                OutputFile = name + ".exe";
            }else
            {
                OutputFile = name + ".dll";
            }
        }

        /// <summary>
        /// Create a project of a specific type at a specific location
        /// </summary>
        /// <param name="name"></param>
        /// <param name="kind"></param>
        /// <param name="directory"></param>
        internal Project(string name, OutputKind kind, string directory) : this(name, kind)
        {
            Directory = directory;
        }

        internal CSharpCompilation Compilation { get; set; }

        internal EmitResult EmitResult { get; set; }

        // The root directory of the project
        internal string Directory
        {
            get { return directory; }
            set { directory = value; }
        }

        // A list of the paths of each source file
        public List<Document> Files { get; protected set; }

        // A list of the pathes of each referenced file
        public List<MetadataReference> References { get; protected set; }

        // Temporary until class Document is finished
        public List<SyntaxTree> Trees { get; set; }

        /// <summary>
        /// Creates new source code file
        /// </summary>
        /// <param name="name">The name of the file</param>
        void New(string path)
        {
            var fileExists = File.Exists(path);
            var isProjectFile = Files.Any(f => f.Equals(path));

            // A new file must not exist in the project file, and it must not already be a file
            if (!fileExists && !isProjectFile)
            {
                // good
            }
            else if (fileExists)
            {

            }
            else if (isProjectFile)
            {
                //throw new Exception("File already exists");
            }
        }

        /// <summary>
        /// Opens an existing source code file
        /// </summary>
        void Open() { }

        /// <summary>
        /// Saves a source code file
        /// </summary>
        void Save() { }

        /// <summary>
        /// Saves a source code file at a new location
        /// </summary>
        /// <param name="path"></param>
        void Save(string path) { }

        /// <summary>
        /// Saves all opened source code files
        /// </summary>
        void SaveAll() { }

        /// <summary>
        /// Removes a source code file from the project and deletes the file
        /// </summary>
        void Delete() { }

        /// <summary>
        /// Removes a source code file from the project, but doesn't delete the file
        /// </summary>
        void Remove() { }

        /// <summary>
        /// Renames a project and any respective files and directory names
        /// </summary>
        void Rename() { }

        // Add an empty file to the project directory
        internal Stream AddFile(string name)
        {
            // Create a file. If the file already exists, then an exception will be thrown
            return new FileStream(string.Format("{0}.cs", name), FileMode.CreateNew);
        }

        internal void AddFile(string name, SyntaxNode node)
        {
            var fs = AddFile(name);

            using(var writer = new StreamWriter(fs))
            {
                writer.Write(node.NormalizeWhitespace().ToFullString());
            }

            fs.Close();
        }

        public void AddClass(string name) {
            // TODO: Add static methods to the Generator class, as that is more appriopriate here
            var gen = new Generator();

            AddFile(name, gen.Class(name));
        }

        public void AddInterface(string name) {
            var gen = new Generator();

            AddFile(name, gen.Interface(name));
        }

        public void AddEnum(string name)
        {
            var gen = new Generator();

            AddFile(name, gen.Enum(name));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Compile()
        {
            Compilation = CSharpCompilation.Create(AssemblyName)
                .AddReferences(new MetadataFileReference(typeof(object).Assembly.Location))
                .AddSyntaxTrees(Trees)
                .WithOptions(new CSharpCompilationOptions(Type));

            // TODO aggregate errors and warnings and send them back to the UI through a delegate (i.e. event handling)

            // temporary
            DisplayMessage(Compilation.GetParseDiagnostics());
        }

        /// <summary>
        /// 
        /// </summary>
        public void Emit()
        {
            EmitResult = Compilation.Emit(OutputFile);

            Debug.WriteLine(string.Format("Emitting IL {0}", EmitResult.Success ? "Succeeded" : "Failed"));

            if (EmitResult.Success)
            {
                OnEmitSucceeded(new EventArgs());
            } else
            {
                OnEmitFailed(new EventArgs());
            }

            // TODO aggregate errors and warnings and provide them back to the UI. This should use events and delegates

            // temporary
            DisplayMessage(EmitResult.Diagnostics);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagnostics"></param>
        void DisplayMessage(ImmutableArray<Diagnostic> diagnostics)
        {
            Debug.WriteLine("Diagnostic Results");

            foreach(var message in diagnostics)
            {
                Debug.WriteLine(message);

                Debug.WriteLine(string.Format("Location: {0}", message.Location));
            }
        }

        internal void WriteTo(string path)
        {
            var fs = new FileStream(path, FileMode.Create);
            var settings = new XmlWriterSettings()
            {
                Indent = true
            };

            var writer = XmlWriter.Create(fs, settings);
            var serializer = new DataContractSerializer(typeof(Project));

            serializer.WriteObject(writer, this);
            writer.Close();
            fs.Close();
        }
    }
}
