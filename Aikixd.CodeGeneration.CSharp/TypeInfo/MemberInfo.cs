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
    public abstract partial class MemberInfo
    {
        protected abstract IOrigin Origin_MemberInfo { get; }

        public string Name => this.Origin_MemberInfo.Name;
        public IEnumerable<AttributeInfo> Attributes => this.Origin_MemberInfo.Attributes;
        public bool IsStatic => this.Origin_MemberInfo.IsStatic;

        protected MemberInfo() { }
    }

    public abstract partial class DataMemberInfo : MemberInfo
    {
        protected abstract IOrigin            Origin_DataMemberInfo { get; }
        protected override MemberInfo.IOrigin Origin_MemberInfo     => this.Origin_DataMemberInfo;

        public TypeInfo Type => this.Origin_DataMemberInfo.Type;

        protected DataMemberInfo() : base() { }
    }

    public sealed partial class FieldMemberInfo : DataMemberInfo
    {
        private IOrigin origin;

        protected override DataMemberInfo.IOrigin Origin_DataMemberInfo => this.origin;

        public bool IsReadOnly => this.origin.IsReadOnly;

        private FieldMemberInfo(IOrigin origin)
        {
            this.origin = origin ?? throw new ArgumentNullException(nameof(origin));
        }

        public static FieldMemberInfo FromSymbol(IFieldSymbol symbol)
        {
            return new FieldMemberInfo(new RoslynOrigin(symbol));
        }
    }

    public sealed partial class PropertyMemberInfo : DataMemberInfo
    {
        private IOrigin origin;
        protected override DataMemberInfo.IOrigin Origin_DataMemberInfo => this.origin;

        public          bool                       IsAutoProperty => this.origin.IsAutoProperty;

        

        private PropertyMemberInfo(IOrigin origin)
        {
            this.origin = origin;
        }

        public static PropertyMemberInfo Generate(string name, TypeInfo type)
        {
            return new PropertyMemberInfo(new NullOrigin(name, type));
        }

        internal static PropertyMemberInfo FromSymbol(IPropertySymbol s)
        {
            return new PropertyMemberInfo(new RoslynOrigin(s));
        }
    }

    public sealed partial class MethodMemberInfo : MemberInfo
    {
        private IOrigin origin;
        protected override MemberInfo.IOrigin Origin_MemberInfo => this.origin;

        public TypeInfo ReturnType { get; }

        public IEnumerable<ParameterInfo> Parameters => this.origin.Parameters;

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
