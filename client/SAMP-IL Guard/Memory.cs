using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
namespace SAMP_IL_Guard
{
    class Memory
    {
        public enum GTAMemoryAddresses
        {
            None = -1,
            Money = 0,
            Hour = 1,
            Minute = 2,
            RadioStation = 3,
            Gravity = 4,
            Fireproof = 5,
            Cheat_InvisibleCars = 6,
            Cheat_DriveOnWater = 7,
            Cheat_BoatsCanFly = 8,
            Cheat_CarsCanFly = 9,
            Cheat_NoReload = 10,
            Cheat_DriverDB = 11,
            Joypad = 12,
            MenuJoypad = 13,
            Crouch = 14,
            PlayerState = 15,
            JumpState = 16,
            RunningState = 17,
            Lock = 18,
            Menu = 19,
            MaxHealth = 20,
            Rain = 21,
            WaveLevel = 22,
            Town = 23,
            Camera1 = 24,
            Camera2 = 25,
            Camera3 = 26,
            JetpackMaxHeight = 27,
            DisableHUD = 28,
            InfiniteRun = 29,
            AFK = 30,
            Cheat_Bulletproof = 31,
            Cheat_Oxygen = 32,
            Vehicle = 33,
            VehicleSirens = 34,
            VehicleMass = 35,
            VehicleFlags = 36,
            VehicleNitro = 37,
            VehicleHorn = 38,
            VehicleExplodeTimer = 39,
            TrainSpeed = 40,
            RadioVolume = 41,
            SfxVolume = 42,
            MoonSize = 43,
            Cheat_StopGameClock = 44,
            NightVision = 45,
            ThermalVision = 46,
            HUD = 47,
            GameSpeed = 48,
            Blur = 49,
            Widescreen = 50,
            Stamina = 51,
            RotationSpeed = 52,
            PistolClip = 53,
            ShotgunClip = 54,
            UziClip = 55,
            AssaultClip = 56,
            VehicleCollision = 57
        }
        private const int CPed = 0xB6F5F0, CVehicle = 0xBA18FC;
        private static int handle = -1;
        public static object[,] MemoryData =
        {   // Addresses from http://www.gtamodding.com/index.php?title=Memory_Addresses_%28SA%29
            {0xB7CE50,  sizeof(uint),       0,          -1},        // Just for some tests: Money
            {0xB70153,  sizeof(byte),       0,          -1},        // Just for some tests: Hour
            {0xB70152,  sizeof(byte),       0,          -1},        // Just for some tests: Minute
            {0xBA679A,  sizeof(byte),       0,          -1},        // RadioStation
            {0x863984,  sizeof(double),     0,          -1},        // Gravity
            {0xB7CEE6,  sizeof(byte),       0,          -1},        // Fireproof
            {0x96914B,  sizeof(byte),       0,          -1},        // Cheat_InvisibleCars
            {0x969152,  sizeof(byte),       0,          -1},        // Cheat_DriveOnWater
            {0x969153,  sizeof(byte),       0,          -1},        // Cheat_BoatsCanFly
            {0x969160,  sizeof(byte),       0,          -1},        // Cheat_CarsCanFly
            {0x969178,  sizeof(byte),       0,          -1},        // Cheat_NoReload
            {0x969179,  sizeof(byte),       0,          -1},        // Cheat_DriverDB
            {0xB6EC2E,  sizeof(byte),       0,          -1},        // Joypad
            {0xBA6818,  sizeof(byte),       0,          -1},        // Menu Joypad
            {0x46F,     sizeof(byte),       CPed,       -1},        // Crouch
            {0x46C,     sizeof(byte),       CPed,       -1},        // Player State
            {0x46D,     sizeof(byte),       CPed,       -1},        // Jump State
            {0x534,     sizeof(byte),       CPed,       -1},        // RunningState
            {0x598,     sizeof(byte),       CPed,       -1},        // Lock
            {0xBA68A5,  sizeof(byte),       0,          -1},        // Menu
            {0xB793E0,  sizeof(double),     0,          -1},        // Max Health 
            {0xC81324,  sizeof(double),     0,          -1},        // Rain
            {0x8D38C8,  sizeof(double),     0,          -1},        // Wave Level
            {0xC81314,  sizeof(byte),       0,          -1},        // Town
            {0xB6F27C,  sizeof(double),     0,          -1},        // Camera 1
            {0xB6F280,  sizeof(double),     0,          -1},        // Camera 2
            {0xB6F274,  sizeof(double),     0,          -1},        // Camera 3
            {0x8703D8,  sizeof(double),     0,          -1},        // Jetpack Max Height
            {0xC8A7C1,  sizeof(byte),       0,          -1},        // Disable HUD
            {0xB7CEE4,  sizeof(byte),       0,          -1},        // Infinite Run
            {0x96916D,  sizeof(byte),       0,          -1},        // Cheat_Bulletproof
            {0x96916E,  sizeof(byte),       0,          -1},        // Cheat_Oxygen
            {0xB7CB49,  sizeof(byte),       0,          -1},        // AFK
            {0x58C,     sizeof(uint),       CPed,       -1},        // Vehicle
            {1069,      sizeof(byte),       CVehicle,   -1},        // Vehicle Sirens
            {140,       sizeof(double),     CVehicle,   -1},        // Vehicle Mass
            {66,        sizeof(byte),       CVehicle,   -1},        // Vehicle Flags
            {0x48A,     sizeof(double),     CVehicle,   -1},        // Vehicle Nitro
            {1300,      sizeof(byte),       CVehicle,   -1},        // Vehicle Horn
            {2276,      sizeof(double),     CVehicle,   -1},        // Vehicle Explode Timer
            {1444,      sizeof(double),     CVehicle,   -1},        // Train Speed
            {0xBA6798,  sizeof(byte),       0,          -1},        // Radio Volume
            {0xBA6797,  sizeof(byte),       0,          -1},        // SFX Volume
            {0x8D4B60,  sizeof(byte),       0,          -1},        // Moon Size
            {0x969168,  sizeof(byte),       0,          -1},        // Cheat_StopGameClock
            {0xC402B8,  sizeof(byte),       0,          -1},        // Night Vision
            {0xC402B9,  sizeof(byte),       0,          -1},        // Thermal Vision
            {0xA444A0,  sizeof(byte),       0,          -1},        // HUD
            {0xB7CB64,  sizeof(double),     0,          -1},        // Game Speed
            {0x8D5104,  sizeof(byte),       0,          -1},        // Blur Level
            {0xB6F065,  sizeof(byte),       0,          -1},        // Widescreen
            {0xB793D8,  sizeof(double),     0,          -1},        // Stamina
            {0x560,     sizeof(double),     CPed,       -1},        // Rotation Speed
            {0x5E0,     sizeof(byte),       CPed,       -1},        // Pistol Clip
            {0x5FC,     sizeof(byte),       CPed,       -1},        // Shotgun Clip
            {0x618,     sizeof(byte),       CPed,       -1},        // SMG Clip
            {0x634,     sizeof(byte),       CPed,       -1},        // Assault Clip
            {216,       sizeof(double),     CVehicle,   -1},        // VehicleCollision
        };
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, uint nSize, int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        static extern bool ReadProcessMemoryFloat(int hProcess, int lpBaseAddress, float lpBuffer, uint nSize, int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);
        [DllImport("kernel32.dll")]
        static extern int CloseHandle(int hObject);
        public static void OpenGTAProcess()
        {
            if (handle == -1)
                handle = OpenProcess(0x1F0FFF, 1, (uint)Variables.proc.Id);
        }
        public static void CloseGTAProcess()
        {
            CloseHandle(handle);
            handle = -1;
        }
        public static void WriteMemory(GTAMemoryAddresses ad, uint value)
        {
            byte i = Convert.ToByte(ad);
            UInt32 maxSize = Convert.ToUInt32(MemoryData[i, 1]);
            int bytesout = 0, offset = Convert.ToInt32(MemoryData[i, 2]), binpos = Convert.ToInt32(MemoryData[i, 3]);
            if (offset != 0) offset = Convert.ToInt32(ValueOf(ReadMemory(Convert.ToInt32(MemoryData[i, 2]), sizeof(uint))));
            if (binpos == -1)
                WriteProcessMemory(handle, offset + Convert.ToInt32(MemoryData[i, 0]), BitConverter.GetBytes(value), maxSize, out bytesout);
            else
            {
                byte[] array = Memory.ReadMemory(offset + Convert.ToInt32(MemoryData[i, 0]), maxSize);
                array[Convert.ToInt32(MemoryData[i, 3])] = Convert.ToByte(value);
                Memory.WriteMemory(offset + Convert.ToInt32(MemoryData[i, 0]), maxSize, BitConverter.ToUInt32(array, 0));
            }
        }
        public static void WriteMemory(GTAMemoryAddresses ad, float value)
        {
            byte i = Convert.ToByte(ad);
            UInt32 maxSize = Convert.ToUInt32(MemoryData[i, 1]);
            int bytesout = 0, offset = Convert.ToInt32(MemoryData[i, 2]), binpos = Convert.ToInt32(MemoryData[i, 3]);
            if (offset != 0) offset = Convert.ToInt32(ValueOf(ReadMemory(Convert.ToInt32(MemoryData[i, 2]), sizeof(uint))));
            if (binpos == -1)
                WriteProcessMemory(handle, offset + Convert.ToInt32(MemoryData[i, 0]), BitConverter.GetBytes(value), maxSize, out bytesout);
            else
            {
                byte[] array = Memory.ReadMemory(offset + Convert.ToInt32(MemoryData[i, 0]), maxSize);
                array[Convert.ToInt32(MemoryData[i, 3])] = Convert.ToByte(value);
                Memory.WriteMemory(offset + Convert.ToInt32(MemoryData[i, 0]), maxSize, BitConverter.ToUInt32(array, 0));
            }
        }
        public static byte[] ReadMemory(GTAMemoryAddresses ad)
        {
            byte i = Convert.ToByte(ad);
            UInt32 maxSize = Convert.ToUInt32(MemoryData[i, 1]);
            byte[] answer = new byte[maxSize];
            int offset = Convert.ToInt32(MemoryData[i, 2]), binpos = Convert.ToInt32(MemoryData[i, 3]);
            if (offset != 0) offset = Convert.ToInt32(ValueOf(ReadMemory(Convert.ToInt32(MemoryData[i, 2]), sizeof(uint))));
            if (binpos == -1)
                ReadProcessMemory(handle, offset + Convert.ToInt32(MemoryData[i, 0]), answer, maxSize, 0);
            else
            {
                answer = Memory.ReadMemory(offset + Convert.ToInt32(MemoryData[i, 0]), maxSize);
                return new byte[] { answer[binpos] };
            }
            return answer;
        }
        public static void WriteMemory(int ad, uint max, uint value)
        {
            int bytesout = 0;
            WriteProcessMemory(handle, ad, BitConverter.GetBytes(value), max, out bytesout);
        }
        public static void WriteMemory(int ad, uint max, float value)
        {
            int bytesout = 0;
            WriteProcessMemory(handle, ad, BitConverter.GetBytes(value), max, out bytesout);
        }
        public static byte[] ReadMemory(int ad, uint max)
        {
            byte[] answer = new byte[max];
            ReadProcessMemory(handle, ad, answer, max, 0);
            return answer;
        }
        public static int GetOffset(int mem)
        {
            return Convert.ToInt32(ValueOf(ReadMemory(Convert.ToInt32(mem), sizeof(uint))));
        }
        public static object ValueOf(byte[] answer)
        {
            switch (Convert.ToUInt32(answer.Length))
            {
                case sizeof(uint): return BitConverter.ToInt32(answer, 0);
                case sizeof(byte): return answer[0];
                case sizeof(double): return BitConverter.ToSingle(answer, 0);
            }
            return 0;
        }
        public static void NOP(int ad, uint bytes)
        {
            int bytesout = 0;
            byte[] nop = new byte[bytes];
            for (uint i = 0; i < bytes; i++) nop[i] = 0x90;
            WriteProcessMemory(ad, ad, nop, bytes, out bytesout);
        }
    }
}