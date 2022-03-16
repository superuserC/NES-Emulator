using NES_Emulator.Core.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core.Extensions
{
    public static class ByteExtensions
    {
        /// <summary>
        /// Check if MSB is equal to one.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNegative(this byte value) => (value & (byte)0x80) == 0x80;

        /// <summary>
        /// Check if value is equal to zero.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsZero(this byte value) => value == 0x00;

        /// <summary>
        /// Add operation.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static byte Add(this byte value, int value2) => (byte)(value + value2);

        /// <summary>
        /// Substract operation.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static byte Substract(this byte value, int value2) => (byte)(value - value2);

        /// <summary>
        /// Performs AND operator.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static byte AND(this byte value, int value2) => (byte)(value & value2);

        /// <summary>
        /// Performs AND operator.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static byte AND(this byte value, Flags6502 state) => (byte)(value & (byte)state);

        /// <summary>
        /// Performs OR operator.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static byte OR(this byte value, int value2) => (byte)(value | value2);

        /// <summary>
        /// Performs OR operator.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static byte OR(this byte value, Flags6502 state) => (byte)(value | (byte)state);

        /// <summary>
        /// Performs XOR operator.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static byte XOR(this byte value, int value2)  => (byte)(value ^ value2);

        /// <summary>
        /// Shift right one bit.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte SR(this byte value) => (byte) (value >> 1);

        /// <summary>
        /// Shift left one bit.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte SL(this byte value) => (byte)(value << 1);
    }
}
