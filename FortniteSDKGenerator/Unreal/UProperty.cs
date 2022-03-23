using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{
    class UProperty : UField
    {
        public UProperty(ulong InAddress) : base(InAddress)
        {
        }

        public Int32 Offset => Read<Int32>(0x44);
    }
}
