using Aikixd.CodeGeneration.Core;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aikixd.CodeGeneration.CSharp
{
    public sealed class CSharpSolutionExplorer : ISolutionExplorer
    {
        private readonly ProjectCollection projectCollection;

        static CSharpSolutionExplorer()
        {
            registerMsBuild();
        }

        private static void registerMsBuild()
        {
            var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            if (instances.Length == 0)
            {
                Console.WriteLine("No MSBuild instances found.");
                throw new CSharpSolutionExplorerInitializationFailedException("No MSBuild instances found.");
            }

            Console.WriteLine("The following MSBuild instances have benen discovered:");
            Console.WriteLine(string.Empty);

            for (int i = 0; i < instances.Length; i++)
            {
                var inst = instances[i];
                Console.WriteLine($"    {i + 1}. {inst.Name} ({inst.Version})");
            }

            Console.WriteLine(string.Empty);

            var instance = instances.First(x => x.Version.Major == 15);


            MSBuildLocator.RegisterInstance(instance);

            Console.WriteLine("Registered first MSBuild instance:");
            Console.WriteLine(string.Empty);
            Console.WriteLine($"    Name: {instance.Name}");
            Console.WriteLine($"    Version: {instance.Version}");
            Console.WriteLine($"    VisualStudioRootPath: {instance.VisualStudioRootPath}");
            Console.WriteLine($"    MSBuildPath: {instance.MSBuildPath}");
            Console.WriteLine(string.Empty);
        }

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
