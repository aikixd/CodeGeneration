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


    internal sealed class DefinitionAnalyzer : IGenerationInfoSource
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
            Run();
        }

        private static void Run()
        {
            var generator = new Generator("AutoGen", new CSharpSolutionExplorer());

            var analyzers = new FeatureInfoSource(
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

    }
}
