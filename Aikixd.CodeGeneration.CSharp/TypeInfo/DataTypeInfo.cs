using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public sealed partial class DataTypeInfo
    {
        public TypeInfo TypeInfo { get; }

        private IOrigin origin;
        public IEnumerable<FieldMemberInfo> Fields => this.origin.Fields;
        public IEnumerable<PropertyMemberInfo> Properties => this.origin.Properties;

        public IEnumerable<MemberInfo> DataMembers =>
            this.Properties
                .Cast<MemberInfo>()
                .Union(this.Fields);

        private DataTypeInfo(TypeInfo typeInfo, IOrigin origin)
        {
            this.TypeInfo = typeInfo;
            this.origin = origin;
        }

        public static DataTypeInfo FromSymbol(INamedTypeSymbol symbol)
        {
            return new DataTypeInfo(TypeInfo.FromSymbol(symbol), new RoslynOrigin(symbol));
        }
    }
}
