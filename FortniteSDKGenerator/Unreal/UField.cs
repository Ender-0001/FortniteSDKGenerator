using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{
    class UField : UObject
    {
        public UField(ulong InAddress) : base(InAddress)
        {
        }

        public UField Next => Read<UField>(0x28);

        public char GetPrefix()
        {
            char Prefix = 'U';

            if (IsA("Actor"))
                Prefix = 'A';

            return Prefix;
        }

        public string GetTypeName()
        {
            var ClassName = Class.GetName();
            string TypeName = "None";

            if (ClassName == "BoolProperty")
                TypeName = "bool";
            else if (ClassName == "ByteProperty" || ClassName == "Int8Property")
                TypeName = "byte";
            else if (ClassName == "Int16Property")
                TypeName = "short";
            else if (ClassName == "UInt16Property")
                TypeName = "ushort";
            else if (ClassName == "IntProperty")
                TypeName = "int";
            else if (ClassName == "UInt32Property")
                TypeName = "uint";
            else if (ClassName == "Int64Property")
                TypeName = "long";
            else if (ClassName == "UInt64Property")
                TypeName = "ulong";
            else if (ClassName == "FloatProperty")
                TypeName = "float";
            else if (ClassName == "DoubleProperty")
                TypeName = "double";
            else if (ClassName == "StrProperty")
                TypeName = "FString";
            else if (ClassName == "ObjectProperty")
                TypeName = "UObject";
            else if (ClassName == "StructProperty")
                TypeName = "None"; // TODO
            else if (ClassName == "EnumProperty")
                TypeName = "None"; // TODO
            else if (ClassName == "NameProperty")
                TypeName = "FName";


            return TypeName;
        }
    }
}
