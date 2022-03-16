using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FortniteSDKGenerator.Globals;

namespace FortniteSDKGenerator
{
    class FName : MemoryObject
    {
        public FName(ulong InAddress) : base(InAddress)
        {
            ComparisonIndex = Read<Int32>(0);
            Index = Read<Int32>(4);
        }

        public Int32 ComparisonIndex;
        public Int32 Index;

        public override String ToString()
        {
            if (GNameCache.ContainsKey(ComparisonIndex))
                return GNameCache[ComparisonIndex];

            var Name = GNames.GetByIndex(ComparisonIndex).GetAnsiName();
            GNameCache.Add(ComparisonIndex, Name);

            return Name;
        }
    }
}
