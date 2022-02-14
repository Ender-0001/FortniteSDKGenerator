using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{
    class Dumper
    {
        static String GetObjectName(IntPtr Object)
        {
            var FStringAddr = Memory.VirtualAllocEx(Memory.GetHandle(), IntPtr.Zero, 16, 0x00001000, 0x0040);

            Memory.Execute(Memory.GetBaseAddress() + 0xC79B10, (UInt64)Object + 0x18, (UInt64)FStringAddr, 0, 0);

            var Str = Encoding.Unicode.GetString(Memory.ReadMemory(Memory.Read<UInt64>(FStringAddr), Memory.Read<Int32>(FStringAddr + 8) * 2));

            Memory.VirtualFreeEx(Memory.GetHandle(), (IntPtr)FStringAddr, 0, 0x8000);

            return Str.TrimEnd('\0');
        }

        public static void ProcessPackage(IntPtr Package, List<IntPtr> Classes)
        {
            var Name = GetObjectName(Package);
            Log.Information("Dumping Package {PackageName}", Name);

            File.AppendAllText("SDK.hpp", $"#include \"SDK/FN_{Name}.hpp\"\n");



            foreach (var Class in Classes)
            {
                
            }
        }

        public static void Dump(TUObjectArray GObjects)
        {
            if (File.Exists("SDK.hpp"))
                File.Delete("SDK.hpp");

            File.Create("SDK.hpp").Close();

            var Packages = new Dictionary<IntPtr, List<IntPtr>>();

            var NumElements = GObjects.NumElements;
            IntPtr LastPackage = IntPtr.Zero;
            for (int i = 0; i < NumElements; i++)
            {
                var Object = GObjects.GetByIndex(i);
                IntPtr Package = Object;

                while (true)
                {
                    var TempOuter = Memory.Read<IntPtr>((UInt64)Package + 0x20);
                    if (TempOuter == IntPtr.Zero)
                        break;
                    Package = TempOuter;
                }

                if (!Packages.ContainsKey(Package)) Packages.Add(Package, new List<IntPtr>());
                Packages[Package].Add(Object);

                if (i == 1000)
                    break;
            }

            foreach (var Package in Packages)
                ProcessPackage(Package.Key, Package.Value);
        }
    }
}
