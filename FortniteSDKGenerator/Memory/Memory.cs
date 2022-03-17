using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FortniteSDKGenerator
{
    class Memory
    {
        public const int PROCESS_CREATE_THREAD = 2;

        public const int PROCESS_VM_OPERATION = 8;
        public const int PROCESS_VM_WRITE = 0x0020;
        public const int PROCESS_VM_READ = 0x0010;

        public const int PROCESS_QUERY_INFORMATION = 0x0400;

        private static Process? ProcessInternal;
        private static IntPtr HandleInternal;
        private static SigScan? SigScanInternal;

        public delegate IntPtr OpenProcessDelegate(uint processAccess, bool bInheritHandle, int processId);
        public static OpenProcessDelegate OpenProcess;

        public delegate bool ReadProcessMemoryDelegate(IntPtr hProcess, UInt64 lpBaseAddress, byte[] lpBuffer, int dwSize, out UInt32 lpNumberOfBytesRead);
        public static ReadProcessMemoryDelegate ReadProcessMemory;

        public delegate bool WriteProcessMemoryDelegate(IntPtr hProcess, UInt64 lpBaseAddress, byte[] buffer, int nSize, out UInt32 lpNumberOfBytesWritten);
        public static WriteProcessMemoryDelegate WriteProcessMemory;

        public delegate UInt64 VirtualAllocExDelegate(IntPtr hProcess, IntPtr lpAddress, Int32 dwSize, Int32 flAllocationType, Int32 flProtect);
        public static VirtualAllocExDelegate VirtualAllocEx;

        public delegate bool VirtualFreeExDelegate(IntPtr hProcess, IntPtr lpAddress, Int32 dwSize, Int32 dwFreeType);
        public static VirtualFreeExDelegate VirtualFreeEx;

        public delegate UInt64 CreateRemoteThreadDelegate(IntPtr hProcess, IntPtr lpThreadAttributes, UInt32 dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, UInt32 dwCreationFlags, IntPtr lpThreadId);
        public static CreateRemoteThreadDelegate CreateRemoteThread;

        public delegate UInt32 WaitForSingleObjectDelegate(IntPtr hHandle, UInt32 dwMilliseconds);
        public static WaitForSingleObjectDelegate WaitForSingleObject;

        public delegate Int32 CloseHandleDelegate(IntPtr hHandle);
        public static CloseHandleDelegate CloseHandle;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string Name);

        public static IntPtr GetPtrToFunction(string Module, string Name)
        {
            return GetProcAddress(GetModuleHandle(Module), Name);
        }

        public static bool Initialize(String ProcName)
        {
            OpenProcess = Marshal.GetDelegateForFunctionPointer<OpenProcessDelegate>(GetPtrToFunction("kernel32.dll", "OpenProcess"));
            ReadProcessMemory = Marshal.GetDelegateForFunctionPointer<ReadProcessMemoryDelegate>(GetPtrToFunction("kernel32.dll", "ReadProcessMemory"));
            WriteProcessMemory = Marshal.GetDelegateForFunctionPointer<WriteProcessMemoryDelegate>(GetPtrToFunction("kernel32.dll", "WriteProcessMemory"));
            VirtualAllocEx = Marshal.GetDelegateForFunctionPointer<VirtualAllocExDelegate>(GetPtrToFunction("kernel32.dll", "VirtualAllocEx"));
            VirtualFreeEx = Marshal.GetDelegateForFunctionPointer<VirtualFreeExDelegate>(GetPtrToFunction("kernel32.dll", "VirtualFreeEx"));
            CreateRemoteThread = Marshal.GetDelegateForFunctionPointer<CreateRemoteThreadDelegate>(GetPtrToFunction("kernel32.dll", "CreateRemoteThread"));
            WaitForSingleObject = Marshal.GetDelegateForFunctionPointer<WaitForSingleObjectDelegate>(GetPtrToFunction("kernel32.dll", "WaitForSingleObject"));
            CloseHandle = Marshal.GetDelegateForFunctionPointer<CloseHandleDelegate>(GetPtrToFunction("kernel32.dll", "CloseHandle"));

            ProcessInternal = Process.GetProcessesByName(ProcName).FirstOrDefault();
            if (ProcessInternal == null)
                return false;

            HandleInternal = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, ProcessInternal.Id);
            SigScanInternal = new SigScan(ProcessInternal, (IntPtr)GetBaseAddress(), ProcessInternal.MainModule.ModuleMemorySize);

            return true;
        }

        public static byte[] ReadMemory(UInt64 Address, int Size)
        {
            byte[] Buffer = new Byte[Size];
            ReadProcessMemory(HandleInternal, Address, Buffer, Size, out UInt32 BytesWritten);
            return Buffer;
        }

        public static UInt64 GetBaseAddress() => 
            (UInt64)ProcessInternal.MainModule.BaseAddress;

        public static IntPtr GetHandle() => 
            HandleInternal;

        public static int StringReadLen = 1024;

        public static T Read<T>(UInt64 Address)
        {
            var type = typeof(T);

            if (type == typeof(String))
            {
                var Bytes = Memory.ReadMemory(Address, StringReadLen);
                String Res = String.Empty;
                foreach (var Byte in Bytes)
                {
                    if (Byte == 0)
                        break;
                    Res += (char)Byte;
                }

                return (T)(Object)Res;
            }

            if (type.IsAssignableTo(typeof(MemoryObject)))
            {
                return (T)(Object)Activator.CreateInstance(type, Read<UInt64>(Address));
            }

            var Buffer = ReadMemory(Address, Marshal.SizeOf<T>());
            var structPtr = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
            var obj = Marshal.PtrToStructure(structPtr.AddrOfPinnedObject(), type);
            structPtr.Free();
            return (T)obj;
        }

        public static UInt64 FindPattern(String pattern)
        {
            var arrayOfBytes = pattern.Split(' ').Select(b => b.Contains("?") ? (Byte)0 : (Byte)Convert.ToInt32(b, 16)).ToArray();
            var strMask = String.Join("", pattern.Split(' ').Select(b => b.Contains("?") ? '?' : 'x'));
            return (UInt64)SigScanInternal.FindPattern(arrayOfBytes, strMask, 0);
        }

        public static bool WriteMemory(UInt64 Address, byte[] Value)
        {
            return WriteProcessMemory(HandleInternal, Address, Value, Value.Length, out uint BytesWritten);
        }

        public static UInt64 Execute(UInt64 fPtr, UInt64 a1, UInt64 a2, UInt64 a3, UInt64 a4, params UInt64[] args)
        {
            var retValPtr = VirtualAllocEx(HandleInternal, IntPtr.Zero, 0x40, 0x1000, 0x40);
            WriteMemory((UInt64)retValPtr, BitConverter.GetBytes((UInt64)0xdeadbeefcafef00d));

            var asm = new List<Byte>();
            asm.AddRange(new Byte[] { 0x48, 0x83, 0xEC }); // sub rsp
            asm.Add(104);

            asm.AddRange(new Byte[] { 0x48, 0xB9 }); // mov rcx
            asm.AddRange(BitConverter.GetBytes(a1));

            asm.AddRange(new Byte[] { 0x48, 0xBA }); // mov rdx
            asm.AddRange(BitConverter.GetBytes(a2));

            asm.AddRange(new Byte[] { 0x49, 0xB8 }); // mov r8
            asm.AddRange(BitConverter.GetBytes(a3));

            asm.AddRange(new Byte[] { 0x49, 0xB9 }); // mov r9
            asm.AddRange(BitConverter.GetBytes(a4));

            var offset = 0u;
            foreach (var obj in args)
            {
                asm.AddRange(new Byte[] { 0x48, 0xB8 }); // mov rax
                asm.AddRange(BitConverter.GetBytes(obj));
                asm.AddRange(new Byte[] { 0x48, 0x89, 0x44, 0x24, (Byte)(0x28 + 8 * offset++) }); // mov rax to stack
            }
            asm.AddRange(new Byte[] { 0x48, 0xB8 }); // mov rax
            asm.AddRange(BitConverter.GetBytes(fPtr));

            asm.AddRange(new Byte[] { 0xFF, 0xD0 }); // call rax
            asm.AddRange(new Byte[] { 0x48, 0x83, 0xC4 }); // add rsp
            asm.Add(104);

            asm.AddRange(new Byte[] { 0x48, 0xA3 }); // mov rax to
            asm.AddRange(BitConverter.GetBytes((UInt64)retValPtr));
            asm.Add(0xC3); // ret
            var codePtr = VirtualAllocEx(HandleInternal, IntPtr.Zero, asm.Count, 0x1000, 0x40);
            WriteProcessMemory(HandleInternal, (UInt64)codePtr, asm.ToArray(), asm.Count, out uint bytesRead);

            var thread = CreateRemoteThread(HandleInternal, IntPtr.Zero, 0, (IntPtr)codePtr, IntPtr.Zero, 0, IntPtr.Zero);
            WaitForSingleObject((IntPtr)thread, 10000);
            var returnValue = Read<UInt64>((UInt64)retValPtr);
            VirtualFreeEx(HandleInternal, (IntPtr)codePtr, 0, 0x8000);
            VirtualFreeEx(HandleInternal, (IntPtr)retValPtr, 0, 0x8000);
            CloseHandle((IntPtr)thread);
            return returnValue;
        }
    }
}
