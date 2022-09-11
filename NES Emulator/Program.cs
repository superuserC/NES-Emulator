using NES_Emulator.Core;
using System;

namespace NES_Emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var cartridge = new Cartridge(@"C:\Users\KevinDANTHINE\Documents\MarioROM\Mario.nes");
            Console.WriteLine($"MapperId : {cartridge.MapperId}");
            Console.WriteLine($"PRG ROM size : {cartridge.PRGROMSize * 16}KB");
            Console.WriteLine($"CHR ROM size : {cartridge.CHRROMSize * 8}KB");
            Console.WriteLine($"Mirroring direction : {cartridge.MirroringDirection}");
            Console.WriteLine($"Trainer present : {cartridge.IsTrainerPresent}");
            Console.WriteLine($"Battery backed : {cartridge.IsBatteryBacked}");
        }
    }
}
