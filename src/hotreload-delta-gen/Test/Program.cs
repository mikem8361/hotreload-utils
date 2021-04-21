using System;
using System.IO;
using System.Reflection;
using RoslynILDiff;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ApplyUpdate 1");
            Assembly assembly = typeof(TestClass).Assembly;
            ReadOnlySpan<byte> metadataDelta = File.ReadAllBytes(@"C:\ssd\hotreload-utils\artifacts\bin\TestClass\Debug\net6.0\TestClass.dll.1.dmeta");
            ReadOnlySpan<byte> ilDelta = File.ReadAllBytes(@"C:\ssd\hotreload-utils\artifacts\bin\TestClass\Debug\net6.0\TestClass.dll.1.dil");
            ReadOnlySpan<byte> pdbDelta = File.ReadAllBytes(@"C:\ssd\hotreload-utils\artifacts\bin\TestClass\Debug\net6.0\TestClass.dll.1.dpdb");
            System.Reflection.Metadata.AssemblyExtensions.ApplyUpdate(assembly, metadataDelta, ilDelta, pdbDelta);
            Console.WriteLine("ApplyUpdate 1 finished");
        }
    }
}
