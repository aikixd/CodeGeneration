using Aikixd.CodeGeneration.Core;
using Aikixd.CodeGeneration.CSharp;
using Aikixd.CodeGeneration.CSharp.TypeInfo;
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
            MSBuildLocator.RegisterDefaults();

            var generator = new Generator("AutoGen", new CSharpSolutionExplorer());

            var analyzers = new FeatureAnalyzer(@"C:\Dev\Aikixd.CodeGeneration\Aikixd.CodeGeneration.sln", new IFeature[] { new AttributeFeature<SerializableAttribute>("serialized", x => $"generated {x.Name}.") });

            analyzers.ProjectFilter = x => x.Name == "Aikixd.CodeGeneration.Test.CSharp.Target.Standard";

            generator.Generate(Enumerable.Repeat(analyzers, 1));
        }
    }
}
