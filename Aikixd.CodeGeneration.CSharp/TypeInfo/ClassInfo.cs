using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public sealed partial class ClassInfo
    {
        private IOrigin origin;

        public TypeInfo TypeInfo { get; }

        public string Name      => this.TypeInfo.Name;
        public string Namespace => this.TypeInfo.Namespace;
        public string FullName  => this.TypeInfo.FullName;


        public Accessibility Accessabilty => this.TypeInfo.Accessibility;
        public bool          IsStatic     => this.origin.IsStatic;
        public bool          IsSealed     => this.origin.IsSealed;

        public IEnumerable<AttributeInfo>      Attributes => this.TypeInfo.Attributes;
        public IEnumerable<FieldMemberInfo>    Fields     => this.origin.Fields;
        public IEnumerable<MethodMemberInfo>   Methods    => this.origin.Methods;
        public IEnumerable<PropertyMemberInfo> Properties => this.origin.Properties;

        public IEnumerable<MemberInfo> Members => 
            this.Properties
                .Cast<MemberInfo>()
                .Union(this.Methods)
                .Union(this.Fields);

        private ClassInfo(
            TypeInfo typeInfo,
            IOrigin origin)
        {
            this.TypeInfo = typeInfo;
            this.origin = origin;
        }

        public static ClassInfo FromSymbol(INamedTypeSymbol symbol)
        {
            if (symbol.DeclaringSyntaxReferences.Length == 0)
                // Declared in referenced assemblies.
                return null;

            return new ClassInfo(TypeInfo.FromSymbol(symbol), new RoslynOrigin(symbol));
        }
    }
}
