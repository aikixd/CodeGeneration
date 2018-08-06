using Aikixd.CodeGeneration.Core;
using Aikixd.CodeGeneration.CSharp.TypeInfo;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp
{
    public interface IFeatureOccurence
    {
        FileGenerationInfo CreateGenerationInfo();
    }

    sealed class ClassFeatureOccurence : IFeatureOccurence
    {
        private readonly IFeature feature;
        private readonly INamedTypeSymbol symbol;
        private readonly ClassDeclarationSyntax syntax;
        private readonly Func<INamedTypeSymbol, string> generateCodeFn;


        public ClassFeatureOccurence(IFeature feature, INamedTypeSymbol symbol, ClassDeclarationSyntax syntax, Func<INamedTypeSymbol, string> generateCodeFn)
        {
            this.feature = feature;
            this.symbol  = symbol ?? throw new ArgumentNullException(nameof(symbol));
            this.syntax  = syntax ?? throw new ArgumentNullException(nameof(syntax));
            this.generateCodeFn = generateCodeFn ?? throw new ArgumentNullException(nameof(generateCodeFn));
        }

        public FileGenerationInfo CreateGenerationInfo()
        {
            //var classInfo = ClassInfo.FromSymbol(this.symbol);

            return new FileGenerationInfo(
                this.symbol.Name,
                this.symbol.ContainingNamespace.ToDisplayString(),
                this.feature.Modifier,
                ".cs",
                this.generateCodeFn(this.symbol));
        }
    }

    public interface IFeature
    {
        string Modifier { get; }

        IEnumerable<IFeatureOccurence> FindOccurences(SemanticModel semanticModel);
    }

    public sealed class ProgressReporter : IProgress<ProjectLoadProgress>
    {
        public void Report(ProjectLoadProgress value)
        {
            if (value.Operation == ProjectLoadOperation.Resolve)
                Console.WriteLine($"Project {value.Operation}:\r\n\t{value.FilePath}\r\n\tframework: {value.TargetFramework}");

            else
                Console.WriteLine($"Project {value.Operation}:\r\n\t{value.FilePath}");
        }
    }

    public sealed class FeatureInfoSource : IGenerationInfoSource
    {
        private readonly MSBuildWorkspace workspace;
        private readonly Solution solution;

        private Func<Project, bool> projectFilter;

        public FeatureInfoSource(string solutionPath, IEnumerable<IFeature> features)
        {
            this.projectFilter = _ => true;

            this.Features      = features;

            var properties = new Dictionary<string, string>
            {
                // This property ensures that XAML files will be compiled in the current AppDomain
                // rather than a separate one. Any tasks isolated in AppDomains or tasks that create
                // AppDomains will likely not work due to https://github.com/Microsoft/MSBuildLocator/issues/16.
                { "AlwaysCompileMarkupFilesInSeparateDomain", bool.FalseString }
            };

            this.workspace = MSBuildWorkspace.Create(properties);
            this.workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
            this.solution  = this.workspace.OpenSolutionAsync(solutionPath, new ProgressReporter()).Result;
        }

        private void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            Console.WriteLine($"Error: {e.Diagnostic.Message}");
        }

        public IEnumerable<IFeature> Features { get; }

        public Func<Project, bool> ProjectFilter
        {
            get => this.projectFilter;
            set => this.projectFilter = value ?? throw new ArgumentNullException("Function");
        }

        public IEnumerable<ProjectGenerationInfo> GenerateInfo()
        {
            var occurs =
                this.solution
                    .Projects
                    .Where(this.projectFilter)
                    .Select(prj => new { prj, cmp = prj.GetCompilationAsync().Result, nfos = prj.Documents.SelectMany(processDoc) });

            return occurs.Select(x => {
                var refs = x.cmp.References.ToArray();

                return new ProjectGenerationInfo(x.prj.FilePath, x.nfos.ToArray()); }).ToArray();

            IEnumerable<FileGenerationInfo> processDoc(Document doc)
            {
                return this.Features
                    .SelectMany(feat =>
                        {
                            var c = doc.Project.GetCompilationAsync().Result;
                            return feat.FindOccurences(c.GetSemanticModel(doc.GetSyntaxTreeAsync().Result));
                        })
                    .Select(x => x.CreateGenerationInfo());
            }
        }
    }
}
