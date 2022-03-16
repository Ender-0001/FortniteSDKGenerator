using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FortniteSDKGenerator.Globals;

namespace FortniteSDKGenerator
{
    class Dumper
    {
        public static String GetObjectName(IntPtr Object)
        {
            var ComparisonIndex = Memory.Read<Int32>((UInt64)Object + 0x18);

            if (GNameCache.ContainsKey(ComparisonIndex))
                return GNameCache[ComparisonIndex];

            var Name = GNames.GetByIndex(ComparisonIndex).GetAnsiName();
            GNameCache.Add(ComparisonIndex, Name);

            return Name;
        }

        public static void ProcessPackage(IntPtr Package, List<IntPtr> Children)
        {
            var Name = GetObjectName(Package);
            Log.Information("Dumping Package {PackageName}", Name.Split('/')[Name.Split('/').Length - 1]);

            File.AppendAllText("SDK.hpp", $"#include \"SDK/FN_{Name}.hpp\"\n");

            foreach (var Child in Children)
            {

            }
        }

        public static void Initialize(TUObjectArray InGObjects, TNameEntryArray InGNames)
        {
            GNames = InGNames;
            GObjects = InGObjects;
        }

        public static void Dump()
        {
            if (File.Exists("SDK.hpp"))
                File.Delete("SDK.hpp");

            File.Create("SDK.hpp").Close();

            var Packages = new Dictionary<IntPtr, List<IntPtr>>();

            foreach (var Object in (List<IntPtr>)GObjects)
            {
                IntPtr Package = Object;

                while (true)
                {
                    var TempOuter = Memory.Read<IntPtr>((UInt64)Package + 0x20);
                    if (TempOuter == IntPtr.Zero)
                        break;
                    Package = TempOuter;
                }

                if (!Packages.Keys.Contains(Package))
                    Packages.Add(Package, new List<IntPtr>());
                Packages[Package].Add(Object);
            }

            foreach (var Package in Packages)
                ProcessPackage(Package.Key, Package.Value);
        }
    }
}
