using Aikixd.CodeGeneration.Core;
using Aikixd.CodeGeneration.CSharp;
using Aikixd.CodeGeneration.CSharp.TypeInfo;
using Aikixd.CodeGeneration.Test.CSharp.Bridge;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aikixd.CodeGeneration.Test.CSharp
{
    

    internal sealed class DefinitionAnalyzer : IAnalyser
    {
        public IEnumerable<ProjectGenerationInfo> GenerateInfo()
        {
            return Enumerable.Empty<ProjectGenerationInfo>();
        }
    }

    
    [Serializable]
    class Program
    {
        static void Main(string[] args)
        {
            if (InitMsBuild() == false)
                return;
            Run();
        }

        private static void Run()
        {
            var generator = new Generator("AutoGen", new CSharpSolutionExplorer());

            var analyzers = new FeatureAnalyzer(
                @"C:\Dev\Aikixd.CodeGeneration\Aikixd.CodeGeneration.TestSolution.sln",
                new IFeature[] {
                    //new AttributeFeature<SerializableAttribute>("serialized", x => $"generated {x.Name}."),
                    new AttributeFeature<TypeTestAttribute>("type.test", TestSymbol)
                });

            //analyzers.ProjectFilter = x => x.Name == "Aikixd.CodeGeneration.Test.CSharp.Target.Standard";

            generator.Generate(Enumerable.Repeat(analyzers, 1));
        }

        static string TestSymbol(INamedTypeSymbol x)
        {
            var classNfo = ClassInfo.FromSymbol(x);

            return $"Generated for {x.Name}";
        }

        static bool InitMsBuild()
        {
            //MSBuildLocator.RegisterDefaults();

            var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            if (instances.Length == 0)
            {
                Console.WriteLine("No MSBuild instances found.");
                return false;
            }

            Console.WriteLine("The following MSBuild instances have benen discovered:");
            Console.WriteLine(string.Empty);

            for (int i = 0; i < instances.Length; i++)
            {
                var inst = instances[i];
                Console.WriteLine($"    {i + 1}. {inst.Name} ({inst.Version})");
            }

            Console.WriteLine(string.Empty);

            var instance = instances[0];
            

            MSBuildLocator.RegisterInstance(instance);

            Console.WriteLine("Registered first MSBuild instance:");
            Console.WriteLine(string.Empty);
            Console.WriteLine($"    Name: {instance.Name}");
            Console.WriteLine($"    Version: {instance.Version}");
            Console.WriteLine($"    VisualStudioRootPath: {instance.VisualStudioRootPath}");
            Console.WriteLine($"    MSBuildPath: {instance.MSBuildPath}");
            Console.WriteLine(string.Empty);
            

            return true;
        }
    }
}
