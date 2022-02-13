using System;
using FortniteSDKGenerator;

namespace FortniteSDKGenerator
{
    class Program
    {
        static void Main()
        {
            Memory.Initialize("FortniteClient-Win64-Shipping");

            var GetEngineVersionAddr = Memory.FindPattern("48 85 C9 74 1D 4C 8B 05 ? ? ? ? 4D 85 C0");
            Console.WriteLine(GetEngineVersionAddr);
        }
    }
}