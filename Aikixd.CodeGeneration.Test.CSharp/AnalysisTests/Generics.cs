using Aikixd.CodeGeneration.CSharp.TypeInfo;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.Test.CSharp.AnalysisTests
{
    public class Generics
    {
        public static void ClosedGenericField(INamedTypeSymbol symbol)
        {
            var nfo = ClassInfo.FromSymbol(symbol);

            var fld = nfo.Fields.Single();

            Test.Assert(
                fld.Type.FullName == "System.Collections.Generic.List<System.Int32>",
                "Wrong closed generic field type.");
        }

        public static void OpenGenericField(INamedTypeSymbol symbol)
        {
            var nfo = ClassInfo.FromSymbol(symbol);

            var fld = nfo.Fields.Single();

            Test.Assert(
                fld.Type.Name == "T",
                "Wrong generic type argument name.");

            Test.Assert(
                fld.Type.FullName == "Aikixd.CodeGeneration.Test.CSharp.Target.Standard.OpenGenericField+T",
                "Wrong generic type argument name.");
        }

        public static void GenericInterfaceImplementation(INamedTypeSymbol symbol)
        {
            var nfo = ClassInfo.FromSymbol(symbol);

            var mtd = nfo.Methods.Single();

            Test.Assert(
                mtd.ReturnType.Name == "T",
                "Wrong type name of generic type argument");
        }
    }
}
