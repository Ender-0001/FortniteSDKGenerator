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

        static List<IntPtr> CachedList;

        public void Initialize()
        {
            var List = new List<IntPtr>();
            for (int i = 0; i < NumElements; i++)
                List.Add(GetByIndex(i));

            CachedList = List;
        }

        public static implicit operator List<IntPtr>(TUObjectArray GObjects)
        {
            if (CachedList != null)
                return CachedList;

            GObjects.Initialize();
            return CachedList;
        }

        public IntPtr GetByIndex(int Index)
        {
            return Memory.Read<IntPtr>((UInt64)Objects + (UInt64)(Index * 24));
        }
    }
}
