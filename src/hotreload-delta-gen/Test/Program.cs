using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using RoslynILDiff;

namespace Test
{
    class Program
    {
        private static List<Action<Type[]>> _beforeUpdates;
        private static List<Action<Type[]>> _afterUpdates;

        static void Main(string[] args)
        {
            (_beforeUpdates, _afterUpdates) = GetMetadataUpdateHandlerActions();

            Console.WriteLine("Press key to continue");
            Console.ReadLine();

            Type type = typeof(TestClass);
            Array.ForEach(type.GetFields(), (field) => Console.WriteLine($"{field.Name} {field.MetadataToken:X8}"));
            Array.ForEach(type.GetMethods(), (method) => Console.WriteLine($"{method.Name} {method.MetadataToken:X8}"));
            Console.WriteLine();

            Console.WriteLine("ApplyUpdate 1");
            ApplyUpdate(
                type,
                @"C:\ssd\hotreload-utils\artifacts\bin\TestClass\Debug\net6.0\TestClass.dll.1.dmeta",
                @"C:\ssd\hotreload-utils\artifacts\bin\TestClass\Debug\net6.0\TestClass.dll.1.dil",
                @"C:\ssd\hotreload-utils\artifacts\bin\TestClass\Debug\net6.0\TestClass.dll.1.dpdb");

            Console.WriteLine("ApplyUpdate 2");
            ApplyUpdate(
                type,
                @"C:\ssd\hotreload-utils\artifacts\bin\TestClass\Debug\net6.0\TestClass.dll.2.dmeta",
                @"C:\ssd\hotreload-utils\artifacts\bin\TestClass\Debug\net6.0\TestClass.dll.2.dil",
                @"C:\ssd\hotreload-utils\artifacts\bin\TestClass\Debug\net6.0\TestClass.dll.2.dpdb");
        }

        private static void ApplyUpdate(Type type, string metadataDeltaFile, string ilDeltaFile, string pdbDeltaFile)
        {
            Assembly assembly = type.Assembly;

            _beforeUpdates.ForEach((handler) => handler(null));

            ReadOnlySpan<byte> metadataDelta = File.ReadAllBytes(metadataDeltaFile);
            ReadOnlySpan<byte> ilDelta = File.ReadAllBytes(ilDeltaFile);
            ReadOnlySpan<byte> pdbDelta = File.ReadAllBytes(pdbDeltaFile);
            System.Reflection.Metadata.AssemblyExtensions.ApplyUpdate(assembly, metadataDelta, ilDelta, pdbDelta);
            Console.WriteLine("ApplyUpdate finished");

            _afterUpdates.ForEach((handler) => handler(null));

            Array.ForEach(type.GetFields(), (field) => Console.WriteLine($"{field.Name} {field.MetadataToken:X8}"));
            Array.ForEach(type.GetMethods(), (method) => Console.WriteLine($"{method.Name} {method.MetadataToken:X8}"));
            Console.WriteLine();
        }

        private static (List<Action<Type[]>> BeforeUpdates, List<Action<Type[]>> AfterUpdates) GetMetadataUpdateHandlerActions()
        {
            var beforeUpdates = new List<Action<Type[]>>();
            var afterUpdates = new List<Action<Type[]>>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (CustomAttributeData attr in assembly.GetCustomAttributesData())
                {
                    if (attr.AttributeType.FullName != "System.Reflection.Metadata.MetadataUpdateHandlerAttribute")
                    {
                        continue;
                    }

                    IList<CustomAttributeTypedArgument> ctorArgs = attr.ConstructorArguments;
                    if (ctorArgs.Count != 1 ||
                        ctorArgs[0].Value is not Type handlerType)
                    {
                        Console.WriteLine($"'{attr}' found with invalid arguments.");
                        continue;
                    }

                    bool methodFound = false;

                    if (GetUpdateMethod(handlerType, "BeforeUpdate") is MethodInfo beforeUpdate)
                    {
                        Console.WriteLine($"Before handler {handlerType} {beforeUpdate} found");
                        beforeUpdates.Add(CreateAction(beforeUpdate));
                        methodFound = true;
                    }

                    if (GetUpdateMethod(handlerType, "AfterUpdate") is MethodInfo afterUpdate)
                    {
                        Console.WriteLine($"After handler {handlerType} {afterUpdate} found");
                        afterUpdates.Add(CreateAction(afterUpdate));
                        methodFound = true;
                    }

                    if (!methodFound)
                    {
                        Console.WriteLine($"No BeforeUpdate or AfterUpdate method found on '{handlerType}'.");
                    }

                    static Action<Type[]> CreateAction(MethodInfo update)
                    {
                        Action<Type[]> action = update.CreateDelegate<Action<Type[]>>();
                        return types =>
                        {
                            try
                            {
                                action(types);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Exception from '{action}': {ex}");
                            }
                        };
                    }

                    static MethodInfo GetUpdateMethod(Type handlerType, string name)
                    {
                        if (handlerType.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, new[] { typeof(Type[]) }) is MethodInfo updateMethod &&
                            updateMethod.ReturnType == typeof(void))
                        {
                            return updateMethod;
                        }

                        foreach (MethodInfo method in handlerType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                        {
                            if (method.Name == name)
                            {
                                Console.WriteLine($"Type '{handlerType}' has method '{method}' that does not match the required signature.");
                                break;
                            }
                        }

                        return null;
                    }
                }
            }

            return (beforeUpdates, afterUpdates);
        }
    }
}
