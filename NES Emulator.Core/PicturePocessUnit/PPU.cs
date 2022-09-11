using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core.PictureProcessUnit
{
    /// <summary>
    /// Represents the Picture Process Unit (PPU)
    /// </summary>
    public class PPU
    {
        public NES NES { get; set; }

        public Cartridge Cartridge => NES.Cartridge;

        /// <summary>
        /// Local VRAM of 2KB size to store table names
        /// </summary>
        private byte[] _VRAM = new byte[2048];

        /// <summary>
        /// Write from cpu bus
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void CPU_Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x0000:
                    break;
                case 0x0001:
                    break;
                case 0x0002:
                    break;
                case 0x0003:
                    break;
                case 0x0004:
                    break;
                case 0x0005:
                    break;
                case 0x0006:
                    break;
                case 0x0007:
                    break;
            }
        }

        /// <summary>
        /// Read from cpu bus
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public byte CPU_Read(ushort address)
        {
            switch (address)
            {
                case 0x0000:
                    break;
                case 0x0001:
                    break;
                case 0x0002:
                    break;
                case 0x0003:
                    break;
                case 0x0004:
                    break;
                case 0x0005:
                    break;
                case 0x0006:
                    break;
                case 0x0007:
                    break;
            }

            return 0x00;
        }
    }
}
