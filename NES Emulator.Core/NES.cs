using NES_Emulator.Core.PictureProcessUnit;
using NES_Emulator.Core.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core
{
    public class NES
    {
        public _6502 CPU { get; }

        public PPU PPU { get; }

        public Cartridge Cartridge { get; set; }

        public Dictionary<ushort, byte> Ram { get; }

        public NES(_6502 cpu, PPU ppu)
        {
            Ram = new Dictionary<ushort, byte>();
        }

        /// <summary>
        /// Range definition :
        /// $0000-$07FF -> 2KB internal RAM
        /// $08FF-$1FFF -> Mirrors of $0000-$07FF
        /// $2000-$2007 -> PPU registers
        /// $2008-$3FFF -> Mirrors of $2000-$2007
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        public void CPU_Write(ushort address, byte data)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                Ram[GetRamAddressMirroring(address)] = data;
            }
            else if (address >= 0x2000 && address <= 0x3fff)
            {
                PPU.CPU_Write(GetPpuAddressMirroring(address), data);
            }
        }

        public byte CPU_Read(ushort address)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                return Ram[GetRamAddressMirroring(address)];
            }

            else if (address >= 0x2000 && address <= 0x3fff)
            {
                return PPU.CPU_Read(GetPpuAddressMirroring(address));
            }

            return 0;
        }

        /// <summary>
        /// Nes RAM is accessible in the range 0x0000 -> 0x1fff
        /// The actual RAM size is 2KB, which is in the range 0x0000 -> 0x07FF
        /// The RAM implements "merroring" that loop over the range 0x0000 -> 0x07ff
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private ushort GetRamAddressMirroring(ushort address) => (ushort)(address & 0x07FF);

        private ushort GetPpuAddressMirroring(ushort address) => (ushort)(address & 0x0007);

        public void LoadROM(string ROMPath)
        {
            var cartridge = new Cartridge(ROMPath);
        }

        public void Reset()
        {
            CPU.Reset();
        }

        public void Clock() => throw new NotImplementedException();
    }
}
