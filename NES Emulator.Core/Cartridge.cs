using NES_Emulator.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings;
using System.Threading.Tasks;

namespace NES_Emulator.Core
{
    public enum MirroringDirection
    {
        Vertical,
        Horizontal
    }

    public class Cartridge
    {
        /// <summary>
        /// Program
        /// </summary>
        public List<byte> PRGMemory { get; set; }

        /// <summary>
        /// Pattern (characters) memory
        /// </summary>
        public List<byte> CHRMemory { get; set; }

        /// <summary>
        /// In the INES format the header contains an 8-bit mapper id.
        /// This id identifies the mapper implemented in the cartridge.
        /// </summary>
        public byte MapperId { get; set; }

        public string HeaderConstant { get; set; }

        public int PRGROMSize { get; set; }

        public int CHRROMSize { get; set; }

        public MirroringDirection MirroringDirection { get; set; }

        public bool IsBatteryBacked { get; set; }

        public bool IsTrainerPresent { get; set; }


        public Cartridge(string ROMPath)
        {
            if (!File.Exists(ROMPath))
            {
                throw new CartridgeCreationException("The file specified does not exist");
            }
            
            LoadROM(ROMPath);
        }

        public void LoadROM(string path)
        {
            var file = File.ReadAllBytes(path);
            HeaderConstant = GetHeaderConstant(file);
            PRGROMSize = GetPRGROMSize(file);
            CHRROMSize = GetCHRROMSize(file);
            MirroringDirection = GetMirroringDirection(file);
            IsBatteryBacked = GetIsBatteryBacked(file);
            IsTrainerPresent = GetIsTrainerPresent(file);
            MapperId = GetMapperId(file);
        }

        /// <summary>
        /// Constant $4E $45 $53 $1A ("NES" followed by MS-DOS end-of-file)
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string GetHeaderConstant(byte[] stream) => Encoding.UTF8.GetString(stream, 0, 4);

        /// <summary>
        /// Size of PRG ROM in 16 KB units
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private int GetPRGROMSize(byte[] stream) => Convert.ToInt32(stream[4]);

        /// <summary>
        /// Size of CHR ROM in 8 KB units.
        /// Value 0 means the board uses CHR RAM
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private int GetCHRROMSize(byte[] stream) => Convert.ToInt32(stream[5]);

        /// <summary>
        /// Mapper, mirroring, battery, trainer
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private byte GetFlagSixHeader(byte[] stream) => stream[6];

        private MirroringDirection GetMirroringDirection(byte[] stream) =>
            (GetFlagSixHeader(stream) & 0b0000001) == 0b0000001
            ? MirroringDirection.Vertical
            : MirroringDirection.Horizontal;

        private bool GetIsBatteryBacked(byte[] stream) =>
            (GetFlagSixHeader(stream) & 0b0000010) == 0b0000010;

        private bool GetIsTrainerPresent(byte[] stream) =>
            (GetFlagSixHeader(stream) & 0b0000100) == 0b0000100;

        private byte GetMapperId(byte[] stream)
        {
            var lowerByte = (byte)((stream[6] & 0b11110000) >> 4);
            var upperByte = (byte)(stream[7] & 0b11110000);

            return (byte)(lowerByte | upperByte);
        }
    }
}
