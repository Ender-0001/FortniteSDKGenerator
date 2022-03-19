using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{
    class FString : MemoryObject
    {
        public FString(ulong InAddress) : base(InAddress)
        {
        }
        public Int32 Length => Read<Int32>(8) * 2;
        public UInt64 ArrayData => Read<UInt64>(0);


        public string Get()
        {
            var Bytes = Memory.ReadMemory(ArrayData, Length);
            return Encoding.Unicode.GetString(Bytes);
        }


    }
}
