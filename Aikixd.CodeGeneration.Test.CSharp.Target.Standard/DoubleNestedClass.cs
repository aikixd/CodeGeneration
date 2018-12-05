using System;
using System.Collections.Generic;
using System.Text;

namespace Aikixd.CodeGeneration.Test.CSharp.Target.Standard
{
    class DoubleNestedClass
    {
        class Middle
        {
            [Serializable]
            class InnerClass
            {

            }
        }
    }
}
