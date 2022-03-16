using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{
    class FNameEntry : MemoryObject
    {
        public FNameEntry(ulong InAddress) : base(InAddress)
        {}

        public Int32 Index => Read<Int32>(0);
        public FNameEntry HashNext => Read<FNameEntry>(8);

        public Int32 GetIndex()
	    {
            return Index >> 1;
	    }

        public bool IsWide()
        {
            return (Index & 0x1) != 0;
        }

        public String GetAnsiName()
        {
            return Read<String>(16);
        }
    }
}
