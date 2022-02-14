using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{
    class TUObjectArray : MemoryObject
    {
        public TUObjectArray(ulong InAddress) : base(InAddress)
        {}

        public IntPtr Objects => Read<IntPtr>(0);
        public Int32 MaxElements => Read<Int32>(8);
        public Int32 NumElements => Read<Int32>(12);

        public IntPtr GetByIndex(int Index)
        {
            return Memory.Read<IntPtr>((UInt64)Objects + (UInt64)(Index * 24));
        }
    }
}
