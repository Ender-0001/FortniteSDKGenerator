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
            Name = new FName(Address + 24);
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
        public UObject Class => Read<UObject>(16);
        public FName Name { get; set; }
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
    }
}
