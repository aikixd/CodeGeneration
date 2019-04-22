using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.Test.CSharp
{
    [Serializable]
    public class TestAssertionException : Exception
    {
        public TestAssertionException() { }
        public TestAssertionException(string message) : base(message) { }
        public TestAssertionException(string message, Exception inner) : base(message, inner) { }
        protected TestAssertionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    static class Test
    {
        [DebuggerNonUserCode]
        public static void Assert(bool condition)
        {
            Assert(condition, "Assertion failed.");
        }

        [DebuggerNonUserCode]
        public static void Assert(bool condition, string message)
        {
            Debug.Assert(condition, message);

            if (condition == false)
            {
                throw new TestAssertionException(message);
            }
        }
    }
}
