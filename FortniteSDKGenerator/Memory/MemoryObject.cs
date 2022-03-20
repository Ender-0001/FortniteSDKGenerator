using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{
    class MemoryObject
    {
        public readonly UInt64 Address;

        public MemoryObject(UInt64 InAddress)
        {
            Address = InAddress;
        }

        public T Read<T>(UInt64 Offset)
        {
            return Memory.Read<T>(Address + Offset);
        }

        public void Write<T>(UInt64 Offset, T Value)
        {
            Memory.Write<T>(Address + Offset, Value);
        }

        public static bool operator ==(MemoryObject obj, MemoryObject obj2)
        {
            return obj.Address == obj2.Address;
        }

        public static bool operator !=(MemoryObject obj, MemoryObject obj2)
        {
            return obj.Address != obj2.Address;
        }
    }
}
