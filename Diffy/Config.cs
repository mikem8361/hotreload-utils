using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Diffy
{
    public enum TfmType {
        Netcore,
        MonoMono
    }

    public class Config
    {

        public static ConfigBuilder Builder () => new ConfigBuilder ();

        public class ConfigBuilder {
            internal ConfigBuilder () {}

            public List<string> Files {get; set; } = new List<string> ();
            public List<string> Libs {get; set; } = new List<String> ();

            public TfmType TfmType {get; set; } = TfmType.Netcore;

            public string? BclBase {get; set; } = default;

            public string OutputDir {get; set;} = ".";

            public Microsoft.CodeAnalysis.OutputKind OutputKind {get; set; } = Microsoft.CodeAnalysis.OutputKind.ConsoleApplication;
            public Config Bake () => new Config(this);
        }

        Config (ConfigBuilder builder) {
            Files = builder.Files;
            Libs = builder.Libs;
            TfmType = builder.TfmType;
            OutputKind = builder.OutputKind;
            BclBase = builder.BclBase;
            OutputDir = builder.OutputDir;
        }

        /// The libraries added to the project
        public IReadOnlyList<string> Libs { get; }
        /// The source file and its sequence of changes
        public IReadOnlyList<string> Files { get; }

        public TfmType TfmType { get; }

        public string? BclBase { get; }

        public Microsoft.CodeAnalysis.OutputKind OutputKind { get; }

        /// Destination path for baseline and deltas
        public string OutputDir { get; }

        /// The full path of the baseline source file
        internal string SourcePath { get => Files[0]; }
        /// Just the base file name of the baseline source file
        public string Filename { get => Path.GetFileName(SourcePath);}

        /// Just the files containing sequence of changes
        public IReadOnlyList<string> DeltaFiles { get => Files.Skip(1).ToList(); }


    }
}
