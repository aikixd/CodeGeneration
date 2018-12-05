using Aikixd.CodeGeneration.Core;
using Aikixd.CodeGeneration.CSharp;
using Aikixd.CodeGeneration.CSharp.SearchPatterns;
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
                
                new IFeatureQuery[] {
                    new TypeQuery(
                        new TypeAttributeSearchPattern<SerializableAttribute>(),
                        new TypeGeneration("serialized", TestSerialized)),

                    new TypeQuery(
                        new TypeAttributeSearchPattern<DetectAttribute>(),
                        new TypeGeneration("detect", TestSymbol)),
                    
                    new TypeQuery(
                        new TypeAttributeSearchPattern<ArrayArgAttribute>(),
                        new TypeGeneration("arrayarg", TestArrayArg)),

                    new TypeQuery(
                        new TypeAttributeSearchPattern<TypeAttribute>(),
                        new TypeGeneration("type", TestType)),

                    new TypeQuery(
                        new TypeAttributeSearchPattern<CombinedAttribute>(),
                        new [] {
                            new TypeGeneration("combined.serializzed", TestSerialized),
                            new TypeGeneration("combined.arrayarg", TestArrayArg)
                        })
                });

            var nfos = analyzers.GenerateInfo();

            var g =
                nfos
                .SelectMany(x => x.FileGeneration)
                .GroupBy(x => x.Name + "." + x.Modifier);

            if (g.Any(x => x.Count() > 1))
            {
                var group = g.First(x => x.Count() > 1);
                var allGroups = g.Where(x => x.Count() > 1).ToArray();
                Debug.Fail($"More than one generation exist for file {group.Key}.");
            }

            generator.Generate(nfos);
        }

        static string TestSymbol(INamedTypeSymbol x)
        {
            var nfo = ClassInfo.FromSymbol(x);

            var members = string.Join(", ", ClassInfo.FromSymbol(x).Fields.Select(y => y.Name));

            return $"namespace {nfo.Namespace} {{" +
                $"partial class {x.Name} {{ string test; }}" +
                $"}}";
        }

        static string TestSerialized(INamedTypeSymbol x)
        {
            var nfo = ClassInfo.FromSymbol(x);

            if (nfo.Name != "DecorationWithSerializable")
                return getText(nfo);

            var attr = nfo.Attributes.First();

            Debug.Assert(attr.Type.Name == "SerializableAttribute");

            return getText(nfo);

            string getText(ClassInfo i)
            {
                return $"// Serialized test. Class name: {i.FullName}.";
            }
        }

        static string TestArrayArg(INamedTypeSymbol x)
        {
            var nfo = ClassInfo.FromSymbol(x);

            if (nfo.Name != "DecorationWithArrayType")
                return getText(nfo);

            Debug.Assert(nfo.Attributes.Count() == 1);

            var attr = nfo.Attributes.First();

            Debug.Assert(attr.PassedArguments.Count() == 1);
            Debug.Assert(attr.PassedArguments.First().GetType() == typeof(object[]));

            Debug.Assert((attr.PassedArguments.First() as object[]).SequenceEqual(new[] { "one", "two" }));

            return getText(nfo);

            string getText(ClassInfo i)
            {
                return $"// Attribute array argument test. Class name: {i.Name}.";
            }
        }

        static string TestType(Compilation c, INamedTypeSymbol x)
        {
            var nfo = ClassInfo.FromSymbol(x);
            var cNfo = CompilationInfo.FromCompilation(c);

            var attr = nfo.Attributes.First();

            Debug.Assert(
                ((CodeGeneration.CSharp.TypeInfo.TypeInfo)attr.PassedArguments[0]).FullName ==
                "System.IEquatable<Aikixd.CodeGeneration.Test.CSharp.Target.Standard.DecorationWithType>");

            var eqNfo = ((CodeGeneration.CSharp.TypeInfo.TypeInfo)attr.PassedArguments[0]).AsInterface();

            return getText(nfo);

            string getText(ClassInfo i)
            {
                return $"// Attribute array argument test. Class name: {i.Name}. TypeArg: {((CodeGeneration.CSharp.TypeInfo.TypeInfo)attr.PassedArguments[0]).FullName}";
            }
        }
    }
}
