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
        static string GetSpacing(int Idx)
        {
            string Spacing = "";

            for (int i = 0; i != Idx; i++)    
                Spacing += "    ";           

            return Spacing;
        }

        public static void ProcessPackage(UObject Package, List<UClass> Classes)
        {
            if (Classes.Count == 0)
                return; // Dont dump empty packages

            var PackageName = Package.GetSplitName();
            Log.Information("Dumping Package {PackageName}", PackageName);

            var PackageFile = new StringBuilder();
            PackageFile.AppendLine("using System;");
            PackageFile.AppendLine("using FortniteSDKGenerator;\n");
            PackageFile.AppendLine("namespace SDK;\n");

            foreach (var Class in Classes)
            {
                if (Class.GetName().Contains("Default__"))
                    continue;

                if (Class.IsA("Class"))
                {
                    var Super = Class.Super;

                    if (Super.IsValid())
                    {
                        var SuperName = Super.GetTypeName();

                        if (SuperName == "None")
                            continue;

                        PackageFile.Append("class ");
                        PackageFile.AppendLine(Class.GetPrefix() + Class.GetName() + " : " + SuperName);
                    }
                    else
                    {
                        PackageFile.Append("class ");
                        PackageFile.AppendLine(Class.GetPrefix() + Class.GetName());
                    }

                    PackageFile.AppendLine("{");

                    PackageFile.Append('\n' + GetSpacing(1) + $"public {Class.GetPrefix() + Class.GetName()}(ulong InAddress) : base(InAddress)");
                    PackageFile.AppendLine(" {}\n");

                    for (var Next = Class.Children; Next.IsValid(); Next = Next.Next)
                    {
                        if (Next.IsA("Property"))
                        {
                            var TypeName = Next.GetTypeName();

                            if (TypeName == "None")
                                continue;

                            var Name = Next.GetName();

                            PackageFile.Append(GetSpacing(1));
                            PackageFile.Append("public ");
                            PackageFile.Append(TypeName);
                            PackageFile.Append(' ');
                            PackageFile.Append(Name);
                            PackageFile.Append(" => GetMember<");
                            PackageFile.Append(TypeName);
                            PackageFile.Append(">(\"");
                            PackageFile.Append(Name);
                            PackageFile.AppendLine("\");");
                        }
                    }

                    PackageFile.AppendLine("}\n");
                }
            }

            File.WriteAllText($".\\SDK\\{PackageName}.cs", PackageFile.ToString());
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

            var Packages = new Dictionary<UInt64, List<UClass>>();

            for (int i = 0; i < GObjects.NumElements; i++)
            {
                var Object = GObjects.GetByIndex(i);
                UObject Package = Object;

                while (true)
                {
                    var TempOuter = Package.Outer;
                    if (TempOuter.Address == 0)
                        break;
                    Package = TempOuter;
                }

                if (!Packages.ContainsKey(Package.Address))
                    Packages.Add(Package.Address, new List<UClass>());
                Packages[Package.Address].Add(Object.Cast<UClass>());
            }

            foreach (var Package in Packages)
                ProcessPackage(Package.Key, Package.Value);
        }
    }
}
