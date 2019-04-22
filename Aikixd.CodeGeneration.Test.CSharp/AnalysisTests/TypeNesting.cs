using Aikixd.CodeGeneration.Test.CSharp.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.Test.CSharp.AnalysisTests
{
    [Test]
    public class NestedTypeArray
    {
        public OuterClass.InnerClass[] field;
    }

    public class OuterClass
    {
        public class InnerClass { }
    }
}
