﻿using System;
using System.Text;
using FortniteSDKGenerator;
using Serilog;
using SDK;

namespace FortniteSDKGenerator
{
    class Program
    {

        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Dumper.log")
                .WriteTo.Debug()
                .MinimumLevel.Debug()
                .MinimumLevel.Verbose()
                .CreateLogger();

            Log.Information("Initializing Dumper...");

            var GObjectsAddr = Memory.GetBaseAddress() + 0x678E010;
            var GNamesAddr = Memory.Read<UInt64>(Memory.GetBaseAddress() + 0x6785448);
            var GObjects = new TUObjectArray(GObjectsAddr);
            var GNames = new TNameEntryArray(GNamesAddr);

            Dumper.Initialize(GObjects, GNames);

            Log.Information("Dumper Initialized...");

            Dumper.Dump();
        }
    }
}