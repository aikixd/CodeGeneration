using Aikixd.CodeGeneration.Core;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aikixd.CodeGeneration.CSharp
{
    public sealed class CSharpSolutionExplorer : ISolutionExplorer
    {
        private readonly ProjectCollection projectCollection;

        public CSharpSolutionExplorer()
        {
            this.projectCollection = new ProjectCollection();
        }

        public IProjectExplorer GetProject(ProjectGenerationInfo projectInfo)
        {
            return new CSharpProjectExplorer(this.projectCollection.LoadProject(projectInfo.Path));
        }

        public void Save()
        {
            foreach (var p in this.projectCollection.LoadedProjects)
                p.Save();
        }
    }
}
