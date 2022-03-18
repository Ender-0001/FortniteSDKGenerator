using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FortniteSDKGenerator.Globals;

namespace FortniteSDKGenerator
{
    class UObject : MemoryObject
    {
        public UObject(ulong InAddress) : base(InAddress)
        {
        }

        public static implicit operator UObject(UInt64 Address)
        {
            return new UObject(Address);
        }

        public static implicit operator UObject(IntPtr Address)
        {
            return new UObject((UInt64)Address);
        }

        public IntPtr VTable => Read<IntPtr>(0);
        public int ObjectFlags => Read<Int32>(8);
        public int InternalIndex => Read<Int32>(12);
        public UClass Class => Read<UClass>(16);
        public FName Name => new FName(Address + 24);
        public UObject Outer => Read<UObject>(0x20);

        public String GetName()
        {
            return Name.ToString();
        }

        public String GetFullName()
        {
            String Res = String.Empty;
            UObject Current = this;
            while (true)
            {
                Current = Current.Outer;
                if (Current.Address == 0)
                    break;
                Res = Current.GetName() + '.' + Res;
            }
            Res = Class.GetName() + ' ' + Res + GetName();
            return Res;
        }

        public bool IsA(string ClassName)
        {
            for (var Super = (UStruct)Class; Super.Address != 0; Super = Super.Super)
            {
                var Name = Super.GetName();

                if (Name == ClassName)
                    return true;

                if (Name.Contains("CoreUObject"))
                    return false;
            }

            return false;
        }

        public T Cast<T>() where T : UObject, new()
        {
            return (T)Activator.CreateInstance(typeof(T), Address);
        }
    }
}
