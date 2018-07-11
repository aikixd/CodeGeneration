using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{

    public sealed class ParameterInfo : IEquatable<ParameterInfo>
    {
        public string Name { get; }
        public TypeInfo Type { get; }

        public ParameterInfo(string name, TypeInfo type)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public bool Equals(ParameterInfo other)
        {
            return this.Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is ParameterInfo x)
                return this.Equals(x);
            return false;
        }

        public static bool operator ==(ParameterInfo left, ParameterInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ParameterInfo left, ParameterInfo right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        internal static ParameterInfo FromSymbol(IParameterSymbol x)
        {
            return new ParameterInfo(
                x.Name,
                TypeInfo.FromSymbol(x.Type));
        }
    }
}
