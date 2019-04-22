using Aikixd.CodeGeneration.CSharp.TypeInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.Test.CSharp.FirstOrderTests
{
    internal class InfoGeneration
    {
        public static void TypeInfoGeneration()
        {
            var typ = TypeInfo.Generate("Name", "Namespace", TypeKind.Object, Accessibility.Private);

            Test.Assert(typ.Name == "Name");
            Test.Assert(typ.FullName == "Namespace.Name");
            Test.Assert(typ.Kind == TypeKind.Object);
            Test.Assert(typ.Accessibility == Accessibility.Private);

            Test.Assert(typ.ContainingType == null);
        }
    }
}
