using Aikixd.CodeGeneration.Test.CSharp.Bridge;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aikixd.CodeGeneration.Test.CSharp.Target.Standard
{
    [Test]
    public class ClosedGenericField
    {
        public List<int> field;
    }

    [Test]
    public class OpenGenericField<T>
    {
        public T field;
    }

    public interface GenericInterface_1<T>
    {
        T Get();
    }

    [Test]
    public class GenericInterfaceImplementation<T> : GenericInterface_1<T>
    {
        public T Get()
        {
            return default(T);
        }
    }
}
