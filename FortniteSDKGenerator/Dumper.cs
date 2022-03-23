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
            var Name = Package.GetSplitName();
            Log.Information("Dumping Package {PackageName}", Name);

            var PackageFile = new StringBuilder();
            PackageFile.AppendLine("using System;");
            PackageFile.AppendLine("using FortniteSDKGenerator;\n");
            PackageFile.AppendLine("namespace SDK;\n");

            foreach (var Child in Children)
            {
                if (Child.IsA("Class"))
                {
                    var Properties = Children.FindAll(delegate (UObject obj) { return obj.Read<UInt64>(0x20) == Child.Address; } );

                    PackageFile.Append("class ");
                    PackageFile.AppendLine(Child.GetName());
                    PackageFile.Append("{\n");

                    foreach (var Property in Properties)
                    {
                        PackageFile.AppendLine(Property.GetName());
                    }

                    PackageFile.AppendLine("}\n");
                }
            }

            File.WriteAllText($".\\SDK\\{Name}.cs", PackageFile.ToString());
        }

        public static void Initialize(TUObjectArray InGObjects, TNameEntryArray InGNames)
        {
            GNames = InGNames;
            GObjects = InGObjects;
        }

        public static void Dump()
        {
            Log.Information("Building Packages...");

            if (Directory.Exists(".\\SDK"))
                Directory.Delete(".\\SDK");
            Directory.CreateDirectory(".\\SDK");

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
