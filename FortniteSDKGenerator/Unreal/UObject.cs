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

        public bool IsValid()
        {
            return Address != 0;
        }

        public string GetSplitName()
        {
            var SplitName = GetName().Split('/');
            return SplitName[SplitName.Length - 1];
        }

        public bool IsA(string ClassName)
        {
            for (var Super = (UStruct)Class; Super.IsValid(); Super = Super.Super)
            {
                var Name = Super.GetName();

                if (Name.Contains(ClassName))
                    return true;

                if (Name.Contains("CoreUObject"))
                    return false;
            }

            return false;
        }

        public T GetMember<T>(string Name)
        {
            for (var Super = (UStruct)Class; Super.IsValid(); Super = Super.Super) // For example only use casts if type is a parent of that class
            {
                for (var Next = Super.Children; Next.IsValid(); Next = Next.Next)
                {
                    if (Next.GetName() == Name)
                        return Read<T>(Next.Cast<UProperty>().Offset);
                }
            }

            return default(T);
        }

        public T Cast<T>() where T : UObject
        {
            return (T)Activator.CreateInstance(typeof(T), Address);
        }
    }
}
