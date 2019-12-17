using Aikixd.CodeGeneration.CSharp.TypeInfo;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Aikixd.CodeGeneration.Test.CSharp.AnalysisTests
{
    public class Decorations
    {
        public static void ParamsAttributeDecoration(INamedTypeSymbol symbol)
        {
            var nfo = ClassInfo.FromSymbol(symbol);

            Test.Assert(nfo.Attributes.Count() == 2, "Wrong number of attributes on a class.");

            var attr = nfo.Attributes.Single(x => x.Type.Name == "ArrayArgAttribute");

            Test.Assert(attr.PassedArguments.Count() == 1, "Wrong number of params arguments in an attribute.");
            Test.Assert(attr.PassedArguments.First().GetType() == typeof(object[]), "Wrong type of attribute parameter.");

            Test.Assert((attr.PassedArguments.First() as object[]).SequenceEqual(new[] { "one", "two" }), "Attribute params argument has wrong values.");
        }

        public static void ArrayAttributeDecoration(INamedTypeSymbol symbol)
        {
            var nfo = ClassInfo.FromSymbol(symbol);

            Test.Assert(nfo.Attributes.Count() == 2, "Wrong number of attributes on a class.");

            var attr = nfo.Attributes.Single(x => x.Type.Name == "ArrayArgAttribute");

            Test.Assert(attr.PassedArguments.Count() == 1, "Wrong number of params arguments in an attribute.");
            Test.Assert(attr.PassedArguments.First().GetType() == typeof(object[]), "Wrong type of attribute parameter.");

            Test.Assert((attr.PassedArguments.First() as object[]).SequenceEqual(new[] { "one", "two" }), "Attribute params argument has wrong values.");
        }

        public static void NamedParamsAttributeDecoration(INamedTypeSymbol symbol)
        {
            var nfo = ClassInfo.FromSymbol(symbol);

            Test.Assert(nfo.Attributes.Count() == 2, "Wrong number of attributes on a class.");

            var attr = nfo.Attributes.Single(x => x.Type.Name == "ArrayArgAttribute");

            Test.Assert(attr.PassedArguments.Count() == 1, "Wrong number of params arguments in an attribute.");
            Test.Assert(attr.NamedArguments.Count == 1, "Wrong number of named arguments in an attribute");

            Test.Assert((attr.NamedArguments["NamedArray"] as object[]).SequenceEqual(new[] { "one", "two" }), "Attribute params argument has wrong values.");
        }
    }
}
