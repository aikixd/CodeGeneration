using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public abstract class MemberInfo
    {
        public abstract string                     Name       { get; }
        public abstract IEnumerable<AttributeInfo> Attributes { get; }

        protected MemberInfo() { }
    }

    public abstract class DataMemberInfo : MemberInfo
    {
        public abstract TypeInfo Type { get; }

        protected DataMemberInfo() : base() { }
    }

    public sealed partial class FieldMemberInfo : DataMemberInfo
    {
        private IOrigin origin;
        public override TypeInfo Type { get; }

        public          bool                       IsReadOnly => this.origin.IsReadOnly;
        public override string                     Name       => this.origin.Name;
        public override IEnumerable<AttributeInfo> Attributes => this.origin.Attributes;

        private FieldMemberInfo(IOrigin origin, TypeInfo type)
        {
            this.origin = origin ?? throw new ArgumentNullException(nameof(origin));
            this.Type   = type   ?? throw new ArgumentNullException(nameof(type));
        }

        public static FieldMemberInfo FromSymbol(IFieldSymbol symbol)
        {
            return new FieldMemberInfo(
                new RoslynOrigin(symbol), 
                TypeInfo.FromSymbol(symbol.Type));
        }
    }

    public sealed partial class PropertyMemberInfo : DataMemberInfo
    {
        private IOrigin origin;
        public override TypeInfo Type { get; }

        public          bool                       IsAutoProperty => this.origin.IsAutoProperty;
        public override string                     Name           => this.origin.Name;
        public override IEnumerable<AttributeInfo> Attributes     => this.origin.Attributes;

        private PropertyMemberInfo(IOrigin origin, TypeInfo type)
        {
            this.origin = origin;
            this.Type = type;
        }

        public static PropertyMemberInfo Generate(string name, TypeInfo type)
        {
            return new PropertyMemberInfo(
                new NullOrigin(
                    true, 
                    name, 
                    Enumerable.Empty<AttributeInfo>()), 
                type);
        }

        internal static PropertyMemberInfo FromSymbol(IPropertySymbol s)
        {
            return new PropertyMemberInfo(new RoslynOrigin(s), TypeInfo.FromSymbol(s.Type));
        }
    }

    public sealed partial class MethodMemberInfo : MemberInfo
    {
        private IOrigin origin;
        public TypeInfo ReturnType { get; }

        public          IEnumerable<ParameterInfo> Parameters => this.origin.Parameters;
        public override string                     Name       => this.origin.Name;
        public override IEnumerable<AttributeInfo> Attributes => this.origin.Attributes;

        private MethodMemberInfo(IOrigin origin, TypeInfo returnType)
        {
            this.ReturnType = returnType;
            this.origin     = origin;
        }

        internal static MethodMemberInfo FromSymbol(IMethodSymbol symbol)
        {
            try
            {
                return new MethodMemberInfo(new RoslynOrigin(symbol), TypeInfo.FromSymbol(symbol.ReturnType));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Could not create method info for {{{symbol}}}. See inner exception for details.", e);
            }
        }
    }
}
