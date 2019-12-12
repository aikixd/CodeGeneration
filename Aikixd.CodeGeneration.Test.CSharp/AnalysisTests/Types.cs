using Aikixd.CodeGeneration.CSharp.TypeInfo;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.Test.CSharp.AnalysisTests
{
    public class Types
    {
        public static void ClassType(INamedTypeSymbol symbol)
        {
            var nfo = ClassInfo.FromSymbol(symbol);

            Test.Assert(nfo.TypeInfo.AsClass() != null, "Type should be convertable to class.");
            Test.Assert(nfo.TypeInfo.Kind == CodeGeneration.CSharp.TypeInfo.TypeKind.Class, "Type kind should be 'Class'");
            Test.Assert(nfo.DataTypeInfo != null, "DataTypeInfo must not be null.");
            Test.Assert(nfo.DataTypeInfo.TypeInfo.Name == "ClassType", "Class name was wrongly detected.");
        }

        public static void StructType(INamedTypeSymbol symbol)
        {
            var nfo = StructInfo.FromSymbol(symbol);

            Test.Assert(nfo.TypeInfo.AsStruct() != null, "Type should be convertable to struct.");
            Test.Assert(nfo.TypeInfo.Kind == CodeGeneration.CSharp.TypeInfo.TypeKind.Struct, "Type kind should be 'Struct'");
            Test.Assert(nfo.DataTypeInfo != null, "DataTypeInfo must not be null.");
            Test.Assert(nfo.DataTypeInfo.TypeInfo.Name == "StructType", "Struct name was wrongly detected.");
        }
    }
}
