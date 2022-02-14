using System;
using System.Text;
using FortniteSDKGenerator;
using Serilog;

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
            Memory.Initialize("FortniteClient-Win64-Shipping");

            var GObjectsAddr = Memory.GetBaseAddress() + 0x44E5CE0;
            var GObjects = new TUObjectArray(GObjectsAddr);

            Log.Information("Dumper Initialized...");

            Dumper.Dump(GObjects);
        }
    }
}