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
using System.Reflection;
using System.Text;
using System.Threading;

namespace Aikixd.CodeGeneration.Test.CSharp
{
    internal sealed class DefinitionAnalyzer
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

            var analyzers = new FeatureLookup(
                @"C:\Dev\Aikixd.CodeGeneration\Aikixd.CodeGeneration.TestSolution.sln",

                new IFeatureQuery[] {
                    /*new TypeQuery(
                        new TypeAttributeSearchPattern<SerializableAttribute>(),
                        new TypeGeneration("serialized", TestSerialized)),

                    new TypeQuery(
                        new TypeAttributeSearchPattern<DetectAttribute>(),
                        new TypeGeneration("detect", TestSymbol)),

                    new TypeQuery(
                        new TypeAttributeSearchPattern<TestPropertiesAttribute>(),
                        new TypeGeneration("testprops", TestProperties)),

                    new TypeQuery(
                        new TypeAttributeSearchPattern<RecursiveAttribute>(),
                        new TypeGeneration("rec", TestRecursive)),

                    new TypeQuery(
                        new TypeAttributeSearchPattern<ArrayArgAttribute>(),
                        new TypeGeneration("arrayarg", TestArrayArg)),

                    new TypeQuery(
                        new TypeAttributeSearchPattern<TypeAttribute>(),
                        new TypeGeneration("type", TestType)),*/

                    new TypeQuery(
                        new TypeAttributeSearchPattern<TestAttribute>(),
                        new TypeGeneration("test", RouteTest)),
                    /*
                    new TypeQuery(
                        new TypeAttributeSearchPattern<CombinedAttribute>(),
                        new [] {
                            new TypeGeneration("combined.serializzed", TestSerialized),
                            new TypeGeneration("combined.arrayarg", TestArrayArg)
                        })
                        */
                });

            var nfos = analyzers.GenerateInfo();
            RunFirstOrderTests();

#if DEBUG
            var g =
                nfos
                .SelectMany(x => x.FileGeneration)
                .GroupBy(x => x.Name + "." + x.Group.Modifier);

            if (g.Any(x => x.Count() > 1))
            {
                var group = g.First(x => x.Count() > 1);
                var allGroups = g.Where(x => x.Count() > 1).ToArray();
                Debug.Fail($"More than one generation exist for file {group.Key}.");
            }
#endif

            generator.Generate(nfos);

            // TODO: check trough reflection that all tests have run.
            Console.Write(testResults.ToString());
            Console.WriteLine(failedTests ? "Has failed tests." : "All tests passed.");

            Console.ReadKey();
        }


        private const string AnalysisTestsNamespace = "Aikixd.CodeGeneration.Test.CSharp.AnalysisTests";
        private const string FirstOrderTestsNamespace = "Aikixd.CodeGeneration.Test.CSharp.FirstOrderTests";
        private static Dictionary<string, System.Reflection.MethodInfo> tests =
            Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass && x.Namespace == AnalysisTestsNamespace)
                .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public))
                .ToDictionary(x => x.Name);

        private static bool failedTests = false;
        private static StringBuilder testResults = new StringBuilder();

        static string RouteTest(INamedTypeSymbol symbol)
        {
            /*if (symbol.Name != "GenericClass")
                return "";*/

            var nfo = ClassInfo.FromSymbol(symbol);

            return 
                string.Join(
                    "\r\n",
                    nfo
                    .Attributes
                    .Where(x => x.Type.Name == "TestAttribute")
                    .Select(
                        x => x.PassedArguments.Count == 0 
                        ? invokeTest(nfo.Name) 
                        : invokeTest((string)x.PassedArguments[0])));

            string invokeTest(string name)
            {
                var r = string.Empty;

                if (tests.TryGetValue(name, out var test))
                {
                    try
                    {
                        test.Invoke(null, new[] { symbol });

                        r = $"Test {name}: OK";
                    }

                    catch (TestAssertionException e)
                    {
                        failedTests = true;
                        r = $"Test {name}: Failed: {e.Message}";
                    }

                    catch (Exception e)
                    {
                        failedTests = true;
                        r = $"Test {name}: Unexpected exception: {e.Message}";
                    }
                }

                else
                {
                    failedTests = true;
                    Debug.Fail($"Test {name} not found.");
                    r = $"Test {name} not found.";
                }

                testResults.AppendLine(r);
                return r;
            }
        }

        private static void RunFirstOrderTests()
        {
            var firstOrderTests =
                Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass && x.Namespace == FirstOrderTestsNamespace)
                .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public));

            foreach (var t in firstOrderTests)
            {
                string r = string.Empty;

                try
                {
                    t.Invoke(null, new object[0]);

                    r = $"Test {t.Name}: OK";
                }

                catch (TestAssertionException e)
                {
                    failedTests = true;
                    r = $"Test {t.Name}: Failed: {e.Message}";
                }

                catch (Exception e)
                {
                    failedTests = true;
                    r = $"Test {t.Name}: Unexpected exception: {e.Message}";
                }

                testResults.AppendLine(r);
            }
        }

        static string TestSymbol(INamedTypeSymbol x)
        {
            var nfo = ClassInfo.FromSymbol(x);

            var members = string.Join(", ", nfo.Fields.Select(y => y.Name));

            return $"namespace {nfo.Namespace} {{" +
                $"partial class {x.Name} {{ string test; }}" +
                $"}}";
        }

        static string TestProperties(INamedTypeSymbol x)
        {
            var nfo = ClassInfo.FromSymbol(x);

            var expressionProp = nfo.Properties.Single(p => p.Name == "Str");
            var auto1 = nfo.Properties.Single(p => p.Name == "Auto1");
            var auto2 = nfo.Properties.Single(p => p.Name == "Auto2");

            Debug.Assert(expressionProp.IsAutoProperty == false);
            Debug.Assert(auto1.IsAutoProperty == true);
            Debug.Assert(auto2.IsAutoProperty == true);

            return $"namespace {nfo.Namespace} {{" +
                $"partial class {x.Name} {{ string test; }}" +
                $"}}";
        }

        static string TestRecursive(INamedTypeSymbol x)
        {
            var nfo = ClassInfo.FromSymbol(x);

            if (nfo.Name != "DecorationWithRecursive")
                return getText(nfo);

            Debug.Assert(nfo.Attributes.Single().Type.Name == nameof(RecursiveAttribute));
            Debug.Assert(nfo.Fields.Single().Type.AsClass().Attributes.Single().Type.Name == nameof(RecursiveAttribute));

            return getText(nfo);

            string getText(ClassInfo i)
            {
                return $"namespace {i.Namespace} {{" +
                    $"partial class {i.Name} {{ string test; }}" +
                    $"}}";
            }
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
