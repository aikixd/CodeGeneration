using Aikixd.CodeGeneration.Test.CSharp.Bridge;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aikixd.CodeGeneration.Test.CSharp.Target.Standard
{
    [Recursive]
    class DecorationWithRecursive
    {
        public DecorationWithRecursiveMember member;
    }

    [Recursive]
    class DecorationWithRecursiveMember
    {
    }
}
