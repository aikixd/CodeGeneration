using Aikixd.CodeGeneration.Test.CSharp.Bridge;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aikixd.CodeGeneration.Test.CSharp.Target.Standard
{
    [Detect]
    public class OuterGenericClass<T> : GenericClass<T>
    {
    }

    [Detect]
    public class GenericClassContainer
    {
        public OuterGenericClass<int> field;
    }
}
