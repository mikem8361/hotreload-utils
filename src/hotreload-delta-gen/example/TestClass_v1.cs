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

        public int DoStuff (int x, int y)
        {
            return x * x + y * y;
        }

        public int FooDog (int x, int y)
        {
            return DoStuff(x, y);
        }
    }
}
