using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{

    class TArray<T> : MemoryObject
    {
        public TArray(ulong InAddress, bool InPtr = true, int InSize = 8) : base(InAddress)
        {
            Ptr = InPtr;
            Size = InSize;
        }

        public readonly bool Ptr;
        public readonly int Size;

        public Int32 Count => Read<Int32>(8);
        public Int32 Max => Read<Int32>(12);
        public UInt64 ArrayData => Read<UInt64>(0);

        public T this[int Index]
        {
            get
            {
                if (Ptr)
                {
                    return (T)Activator.CreateInstance(typeof(T), Memory.Read<UInt64>(ArrayData, (8 * Index)));
                }
                else
                {
                    return (T)Activator.CreateInstance(typeof(T), ArrayData, (Size * Index));
                }
            }
        }

        public int Num() => Count;
    }
}
