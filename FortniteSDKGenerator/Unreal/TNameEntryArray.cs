using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{
    class TNameEntryArray : MemoryObject
    {
        public TNameEntryArray(ulong InAddress) : base(InAddress)
        {
            Chunks = new UInt64[ChunkTableSize];
            for (int i = 0; i < ChunkTableSize; i++)
                Chunks[i] = Memory.Read<UInt64>(InAddress + (UInt64)(i * 8));
        }

        public int Num() => ElementCount;

        public bool IsValidIndex(Int32 index)
	    {
	    	return index < Num() && index >= 0;
	    }

        public FNameEntry GetByIndex(Int32 Index)
        {
            Int32 ChunkIndex = Index / ElementsPerChunk;
            Int32 WithinChunkIndex = Index % ElementsPerChunk;

            var Chunk = Chunks[ChunkIndex];
            return new FNameEntry(Memory.Read<UInt64>(Chunk + (UInt64)WithinChunkIndex * 8));
        }

        static readonly int MaxTotalElements = 2 * 1024 * 1024;
        static readonly int ElementsPerChunk = 16384;
        static readonly int ChunkTableSize = (MaxTotalElements + ElementsPerChunk - 1) / ElementsPerChunk;
        static readonly UInt64 TableSizeInBytes = (UInt64)((int)ChunkTableSize * (int)IntPtr.Size);

        UInt64[] Chunks { get; set; }
        public int ElementCount => Read<Int32>(TableSizeInBytes);
        public int ChunkCount => Read<Int32>(TableSizeInBytes + 4);
    }
}
