﻿using Aikixd.CodeGeneration.Test.CSharp.Bridge;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aikixd.CodeGeneration.Test.CSharp.Target.Standard
{
    [Test]
    [ArrayArg("one", "two")]
    class ParamsAttributeDecoration
    {
    }

    [Test]
    [ArrayArg(new[] { "one", "two" })]
    class ArrayAttributeDecoration
    {
    }

    [Test]
    [ArrayArg(NamedArray = new[] { "one", "two" })]
    class NamedParamsAttributeDecoration
    {
    }
}
