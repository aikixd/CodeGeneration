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
    /// <summary>
    /// Looks for features in the source code provided in the constructor
    /// and generates the <see cref="FileGenerationInfo"/> based on the found
    /// occurences.
    /// </summary>
    public sealed class FeatureInfoSource : IGenerationInfoSource
    {
        private readonly MSBuildWorkspace workspace;
        private readonly Solution solution;

        private Func<Project, bool> projectFilter;

        private readonly IEnumerable<IFeatureQuery> queries;

        public FeatureInfoSource(string solutionPath, IEnumerable<IFeatureQuery> queries)
        {
            this.projectFilter = _ => true;

            this.queries = queries;

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

        /// <summary>
        /// The filter that determines in which projects to look for features.
        /// </summary>
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
                    .Select(prj => new
                    {
                        prj,
                        cmp = prj.GetCompilationAsync().Result
                    })
                    .Select(x => new
                    {
                        x.prj,
                        nfos = this.queries.SelectMany(q =>
                            q.Execute(x.prj, x.cmp))
                    });
                        
                    //)
                    //.Select(prj => new {
                    //    prj,
                    //    cmp = prj.GetCompilationAsync().Result.GlobalNamespace.gett,
                    //    nfos = prj.Documents.SelectMany(processDoc) });

            return 
                occurs
                .Select(x => new ProjectGenerationInfo(x.prj.FilePath, x.nfos.ToArray()))
                .ToArray();
        }

        
    }
}
