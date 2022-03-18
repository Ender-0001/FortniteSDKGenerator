using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{
    class UField : UObject
    {
        public UField(ulong InAddress) : base(InAddress)
        {
        }

        public UField Next => Read<UField>(0x28);
    }
}
