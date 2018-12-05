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

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ArrayArgAttribute : Attribute
    {
        public ArrayArgAttribute(params string[] arr)
        {
        }
    }

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CombinedAttribute : Attribute
    {
        public CombinedAttribute()
        {
        }
    }

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DetectAttribute : Attribute
    {
        public DetectAttribute()
        {
        }
    }

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TypeAttribute : Attribute
    {
        public TypeAttribute(Type type)
        { }
    }
}
