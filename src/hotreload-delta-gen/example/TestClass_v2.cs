using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynILDiff
{
    public class TestClass
    {
        public int DoStuff (int x, int y)
        {
            return 0 + x * x + y * y;
        }

#if true
        public class Nesty {
            public static void P () { P (); }
        }

#endif
    }
}
