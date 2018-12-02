using Aikixd.CodeGeneration.Core;
using Aikixd.CodeGeneration.CSharp;
using Aikixd.CodeGeneration.CSharp.TypeInfo;
using Aikixd.CodeGeneration.Test.CSharp.Bridge;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    new AttributeFeature<SerializableAttribute>("serialized", TestSymbol),
                    new AttributeFeature<TypeTestAttribute>("type.test", TestSymbol),
                    new AttributeFeature<ArrayArgAttribute>("arrayarg", ArrayArgTest)
                });

            //analyzers.ProjectFilter = x => x.Name == "Aikixd.CodeGeneration.Test.CSharp.Target.Standard";

            generator.Generate(analyzers.GenerateInfo());
        }

        static string TestSymbol(INamedTypeSymbol x)
        {
            var classNfo = ClassInfo.FromSymbol(x);

            return $"Generated for {x.Name}";
        }


        static string ArrayArgTest(INamedTypeSymbol x)
        {
            var nfo = ClassInfo.FromSymbol(x);

            Debug.Assert(nfo.Attributes.Count() == 1);

            var attr = nfo.Attributes.First();

            Debug.Assert(attr.PassedArguments.Count() == 1);
            Debug.Assert(attr.PassedArguments.First().GetType() == typeof(object[]));

            Debug.Assert((attr.PassedArguments.First() as object[]).SequenceEqual(new[] { "one", "two" }));

            return nfo.Name;
        }

    }
}
