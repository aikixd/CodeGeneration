using Aikixd.CodeGeneration.Test.CSharp.Bridge;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aikixd.CodeGeneration.Test.CSharp.Target.Standard
{
    [TestProperties]
    class PropertyTests
    {
        public string Str => "str";

        public string Auto1 { get; }
        public string Auto2 { get; set; }
    }
}
