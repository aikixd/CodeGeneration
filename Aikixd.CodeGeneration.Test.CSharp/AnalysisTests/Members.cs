using Aikixd.CodeGeneration.CSharp.TypeInfo;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.Test.CSharp.AnalysisTests
{
    public static class Members
    {
        private class FieldAssertions
        {
            public bool IsStatic { get; set; }
            public bool IsReadOnly { get; set; }
            public string TypeFullName { get; set; }
        }

        private class PropertyAssertions
        {
            public bool IsAutoProperty { get; internal set; }
            public bool IsStatic { get; internal set; }
            public string TypeFullName { get; internal set; }
        }

        public static void ClassMembers(INamedTypeSymbol symbol) => dataTypeMembers(symbol);
        public static void StructMembers(INamedTypeSymbol symbol) => dataTypeMembers(symbol);

        private static void dataTypeMembers(INamedTypeSymbol symbol)
        {
            var nfo = DataTypeInfo.FromSymbol(symbol);

            var fieldAssertions = new Dictionary<string, FieldAssertions>
            {
                { "fldInt", new FieldAssertions {
                  IsReadOnly = false,
                  IsStatic   = false,
                  TypeFullName = "System.Int32" } },

                { "FldInt", new FieldAssertions {
                  IsReadOnly = false,
                  IsStatic = false,
                  TypeFullName = "System.Int32" } },

                { "fldStr", new FieldAssertions {
                  IsReadOnly = true,
                  IsStatic = false,
                  TypeFullName = "System.String" } },

                { "FldStr", new FieldAssertions {
                  IsReadOnly = true,
                  IsStatic = false,
                  TypeFullName = "System.String" } },

                { "sFldInt", new FieldAssertions {
                  IsReadOnly = false,
                  IsStatic = true,
                  TypeFullName = "System.Int32" } },

                { "SFldInt", new FieldAssertions {
                  IsReadOnly = false,
                  IsStatic = true,
                  TypeFullName = "System.Int32" } }
            };

            var propertiesAssertions = new Dictionary<string, PropertyAssertions>
            {
                { "PropInt_auto_get_set", new PropertyAssertions {
                  IsAutoProperty = true,
                  IsStatic = false,
                  TypeFullName = "System.Int32"} },

                { "PropInt_auto_get_pSet", new PropertyAssertions {
                  IsAutoProperty = true,
                  IsStatic = false,
                  TypeFullName = "System.Int32" } },

                { "PropInt_auto_get", new PropertyAssertions {
                  IsAutoProperty = true,
                  IsStatic = false,
                  TypeFullName = "System.Int32" } },

                { "PropInt_auto_get_assigned", new PropertyAssertions {
                  IsAutoProperty = true,
                  IsStatic = false,
                  TypeFullName = "System.Int32" } },

                { "PropInt_exr", new PropertyAssertions {
                  IsAutoProperty = false,
                  IsStatic = false,
                  TypeFullName = "System.Int32" } },

                { "PropInt_get", new PropertyAssertions {
                  IsAutoProperty = false,
                  IsStatic = false,
                  TypeFullName = "System.Int32" } },

                { "PropInt_get_set", new PropertyAssertions {
                  IsAutoProperty = false,
                  IsStatic = false,
                  TypeFullName = "System.Int32" } },

                { "PropInt_get_expr", new PropertyAssertions {
                  IsAutoProperty = false,
                  IsStatic = false,
                  TypeFullName = "System.Int32" } },

                { "PropInt_get_set_expr", new PropertyAssertions {
                  IsAutoProperty = false,
                  IsStatic = false,
                  TypeFullName = "System.Int32" } },

                { "SPropInt_auto_get_set", new PropertyAssertions {
                  IsAutoProperty = true,
                  IsStatic = true,
                  TypeFullName = "System.Int32" } },
            };

            // Structs can't have assigned proerties
            if (symbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.Struct)
                propertiesAssertions.Remove("PropInt_auto_get_assigned");

            foreach (var fld in nfo.Fields)
                asserFieldInfo(fld, fieldAssertions[fld.Name]);

            foreach (var prop in nfo.Properties)
                assertPropertyInfo(prop, propertiesAssertions[prop.Name]);

            void asserFieldInfo(FieldMemberInfo field, FieldAssertions assertions)
            {
                Test.Assert(field.IsStatic == assertions.IsStatic, "Field IsStatic assertion failed.");
                Test.Assert(field.IsReadOnly == assertions.IsReadOnly, "Field IsReadOnly assertion failed.");
                Test.Assert(field.Type.FullName == assertions.TypeFullName, "Field type name assertion failed.");
            }

            void assertPropertyInfo(PropertyMemberInfo property, PropertyAssertions assertions)
            {
                Test.Assert(property.IsAutoProperty == assertions.IsAutoProperty, "Property IsAutoProperty assertion failed.");
                Test.Assert(property.IsStatic == assertions.IsStatic, "Property IsStatic assertion failed.");
                Test.Assert(property.Type.FullName == assertions.TypeFullName, "Property type name assertion failed.");
            }
        }
    }
}
