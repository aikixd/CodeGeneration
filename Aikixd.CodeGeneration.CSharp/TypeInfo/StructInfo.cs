using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public partial class StructInfo
    {
        private IOrigin origin;

        public TypeInfo TypeInfo { get; }
        public DataTypeInfo DataTypeInfo { get; }

        public string Name => this.TypeInfo.Name;
        public string Namespace => this.TypeInfo.Namespace;
        public string FullName => this.TypeInfo.FullName;


        public Accessibility Accessabilty => this.TypeInfo.Accessibility;
        public bool IsSealed => this.origin.IsSealed;

        public IEnumerable<AttributeInfo> Attributes => this.TypeInfo.Attributes;
        public IEnumerable<FieldMemberInfo> Fields => this.DataTypeInfo.Fields;
        public IEnumerable<MethodMemberInfo> Methods => this.origin.Methods;
        public IEnumerable<PropertyMemberInfo> Properties => this.DataTypeInfo.Properties;

        public IEnumerable<MemberInfo> Members =>
            this.Properties
                .Cast<MemberInfo>()
                .Union(this.Methods)
                .Union(this.Fields);

        private StructInfo(
            DataTypeInfo dataTypeInfo,
            IOrigin origin)
        {
            var typeInfo = dataTypeInfo.TypeInfo;
            
            if (typeInfo.Kind != TypeKind.Struct)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(typeInfo),
                    typeInfo,
                    "Provided type info is not a struct.");
            }

            this.TypeInfo = typeInfo;
            this.DataTypeInfo = dataTypeInfo;
            this.origin = origin;
        }

        public static StructInfo FromSymbol(INamedTypeSymbol symbol)
        {
            if (symbol.DeclaringSyntaxReferences.Length == 0)
                // Declared in referenced assemblies.
                return null;

            if (symbol.TypeKind != Microsoft.CodeAnalysis.TypeKind.Struct)
                return null;

            return new StructInfo(DataTypeInfo.FromSymbol(symbol), new RoslynOrigin(symbol));
        }
    }
}
