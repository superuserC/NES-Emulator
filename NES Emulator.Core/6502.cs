using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core
{
    /// <summary>
    /// Flags for processor status registry
    /// </summary>
    public enum Flags6502
    {
        Negative = 1 << 7,
        Overflow = 1 << 6,
        Unused = 1 << 5,
        Break = 1 << 4,
        DecimalMode = 1 << 3,
        IRQDisable = 1 << 2,
        Zero = 1 << 1,
        Carry = 1 << 0
    }

    /// <summary>
    /// 6502 processor.
    /// </summary>
    public class _6502
    {
        /// <summary>
        /// Accumulator
        /// </summary>
        public byte Acc_Register { get; set; } = 0x00;
        public byte X_Register { get; set; } = 0x00;
        public byte Y_Register { get; set; } = 0x00;

        /// <summary>
        /// Stack pointer
        /// </summary>
        public byte SP_Register { get; set; } = 0x00;

        /// <summary>
        /// Program counter
        /// </summary>
        public ushort PC_Register { get; set; } = 0x0000;

        /// <summary>
        /// Flags
        /// </summary>
        public byte Status_Register { get; set; } = 0x00;

        #region events
        /// <summary>
        /// See processor sheet.
        /// </summary>
        public void Clock() { }

        /// <summary>
        /// See processor sheet.
        /// </summary>
        public void IRQ() { }

        /// <summary>
        /// See processor sheet.
        /// </summary>
        public void NMI() { }

        /// <summary>
        /// See processor sheet.
        /// </summary>
        public void Reset() { }

        #endregion

        /// <summary>
        /// Write data to the bus.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        public void Write(ushort address, byte data)
        {

        }

        /// <summary>
        /// Read data from the bus.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public byte Read(ushort address)
        {
            return 0x00;
        }

    }
}
