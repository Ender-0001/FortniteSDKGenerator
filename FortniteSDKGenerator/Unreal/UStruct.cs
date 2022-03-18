
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{

    class UStruct : UField
    {
        public UStruct(ulong InAddress) : base(InAddress)
        {
        }

        public UStruct Super => Read<UStruct>(0x30);
        public UField Children => Read<UField>(0x38);
        public int PropertySize => Read<Int32>(0x40);
        public int MinAlignmnet => Read<Int32>(0x44);
    }
}
