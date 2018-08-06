using System;

namespace Aikixd.CodeGeneration.Test.CSharp.Bridge
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class TypeTestAttribute : Attribute
    {
        private readonly string str;
        private readonly int i;
        private readonly Type type;

        public TypeTestAttribute()
        {

        }

        public TypeTestAttribute(Type type)
        {
            this.type = type;
        }
        
        public TypeTestAttribute(string str)
        {
            this.str = str;
        }

        public TypeTestAttribute(int i) {
            this.i = i;
        }
    }
}
