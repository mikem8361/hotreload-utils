using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynILDiff
{
    public class TestClass
    {
        public nint _field0 = 0;
        
        public int DoStuff (int x, int y)
        {
            return x * y;
        }
    }
}
