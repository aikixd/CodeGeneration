using Aikixd.CodeGeneration.Test.CSharp.Bridge;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aikixd.CodeGeneration.Test.CSharp.Target.Standard
{
    [Test]
    class ClassMembers
    {
        private int fldInt;
        public int FldInt;

        private readonly string fldStr;
        public readonly string FldStr;

        public int PropInt_auto_get_set { get; set; }
        public int PropInt_auto_get_pSet { get; private set; }
        public int PropInt_auto_get { get; }
        public int PropInt_auto_get_assigned { get; } = 1;

        public int PropInt_exr => this.fldInt;
        public int PropInt_get
        {
            get
            {
                return this.fldInt;
            }
        }
        public int PropInt_get_set
        {
            get
            {
                return this.fldInt;
            }
            set
            {
                this.fldInt = value;
            }
        }
        public int PropInt_get_expr
        {
            get => this.fldInt;
        }
        public int PropInt_get_set_expr
        {
            get => this.fldInt;
            set => this.fldInt = value;
        }

        private static int sFldInt;
        public static int SFldInt;

        public static int SPropInt_auto_get_set { get; set; }
    }

    [Test]
    struct StructMembers
    {
        private int fldInt;
        public int FldInt;

        private readonly string fldStr;
        public readonly string FldStr;

        public int PropInt_auto_get_set { get; set; }
        public int PropInt_auto_get_pSet { get; private set; }
        public int PropInt_auto_get { get; }

        public int PropInt_exr => this.fldInt;
        public int PropInt_get
        {
            get
            {
                return this.fldInt;
            }
        }
        public int PropInt_get_set
        {
            get
            {
                return this.fldInt;
            }
            set
            {
                this.fldInt = value;
            }
        }
        public int PropInt_get_expr
        {
            get => this.fldInt;
        }
        public int PropInt_get_set_expr
        {
            get => this.fldInt;
            set => this.fldInt = value;
        }

        private static int sFldInt;
        public static int SFldInt;

        public static int SPropInt_auto_get_set { get; set; }
    }
}
