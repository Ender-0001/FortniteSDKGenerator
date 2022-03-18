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
        public static void ProcessPackage(UObject Package, List<UObject> Children)
        {
            var Name = Package.GetName();
            Log.Information("Dumping Package {PackageName}", Name);

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

            Log.Information("Building Packages...");

            var Packages = new Dictionary<UInt64, List<UObject>>();

            foreach (var Object in (List<UObject>)GObjects)
            {
                UObject Package = Object;

                while (true)
                {
                    var TempOuter = Package.Outer;
                    if (TempOuter.Address == 0)
                        break;
                    Package = TempOuter;
                }

                if (!Packages.ContainsKey(Package.Address))
                    Packages.Add(Package.Address, new List<UObject>());
                Packages[Package.Address].Add(Object);
            }

            foreach (var Package in Packages)
                ProcessPackage(Package.Key, Package.Value);
        }
    }
}
