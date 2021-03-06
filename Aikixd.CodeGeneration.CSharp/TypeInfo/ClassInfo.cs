﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public sealed partial class ClassInfo
    {
        private IOrigin origin;

        public TypeInfo TypeInfo { get; }
        public DataTypeInfo DataTypeInfo { get; }

        public string Name      => this.TypeInfo.Name;
        public string Namespace => this.TypeInfo.Namespace;
        public string FullName  => this.TypeInfo.FullName;


        public Accessibility Accessabilty => this.TypeInfo.Accessibility;
        public bool          IsStatic     => this.origin.IsStatic;
        public bool          IsSealed     => this.origin.IsSealed;

        public IEnumerable<AttributeInfo>      Attributes => this.TypeInfo.Attributes;
        public IEnumerable<FieldMemberInfo>    Fields     => this.DataTypeInfo.Fields;
        public IEnumerable<MethodMemberInfo>   Methods    => this.origin.Methods;
        public IEnumerable<PropertyMemberInfo> Properties => this.DataTypeInfo.Properties;

        public IEnumerable<MemberInfo> Members => 
            this.Properties
                .Cast<MemberInfo>()
                .Union(this.Methods)
                .Union(this.Fields);


        private ClassInfo(
            DataTypeInfo typeDataInfo,
            IOrigin origin)
        {
            var typeInfo = typeDataInfo.TypeInfo;
            
            if (typeInfo.Kind != TypeKind.Class)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(typeInfo),
                    typeInfo,
                    "Provided type info is not a class.");
            }

            this.TypeInfo = typeInfo;
            this.DataTypeInfo = typeDataInfo;
            this.origin = origin;
        }

        public static ClassInfo FromSymbol(INamedTypeSymbol symbol)
        {
            if (symbol.DeclaringSyntaxReferences.Length == 0)
                // Declared in referenced assemblies.
                return null;

            if (symbol.TypeKind != Microsoft.CodeAnalysis.TypeKind.Class)
                return null;

            return new ClassInfo(
                DataTypeInfo.FromSymbol(symbol),
                new RoslynOrigin(symbol));
        }
    }
}
