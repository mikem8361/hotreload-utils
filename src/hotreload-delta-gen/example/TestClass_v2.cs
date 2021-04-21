using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynILDiff
{
    public class TestClass
    {
        public int _field0 = 0;
        public int _field1 = 1;
        public int _field2 = 2;

        public int DoStuff (int x, int y)
        {
            return x * y;
        }
    }
}
