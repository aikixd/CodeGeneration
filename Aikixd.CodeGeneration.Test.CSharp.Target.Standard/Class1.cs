using System;

namespace Aikixd.CodeGeneration.Test.CSharp.Target.Standard
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class MyAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string positionalString;

        public MyAttribute()
        {

        }

        // This is a positional argument
        public MyAttribute(string positionalString)
        {
            this.positionalString = positionalString;

            // TODO: Implement code here

            throw new NotImplementedException();
        }

        public string PositionalString
        {
            get { return positionalString; }
        }

        // This is a named argument
        public int NamedInt { get; set; }
    }

    [Serializable]
    public class MyClass
    {
        //[My("str")]
        public bool boolean;
    }

    //[Serializable]
    public class anotherClass
    {

    }
}
