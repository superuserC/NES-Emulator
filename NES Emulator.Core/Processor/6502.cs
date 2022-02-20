using Dawn;
using NES_Emulator.Core.Extensions;
using NES_Emulator.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core.Processor
{
    /// <summary>
    /// Flags for processor status registry
    /// </summary>
    public enum Flags6502 : byte
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
    public partial class _6502
    {
        public _6502(IDataTransfer dataTransfer)
        {
            DataTransfer = Guard.Argument(dataTransfer, nameof(dataTransfer)).NotNull().Value;
            _instructionsMap = MapProcessorInstructions();
        }

        public IDataTransfer DataTransfer { get; set; }

        /// <summary>
        /// Accumulator
        /// </summary>
        public byte _acc_Register { get; private set; } = 0x00;

        public byte _x_Register { get; set; } = 0x00;
        public byte _y_Register { get; set; } = 0x00;

        /// <summary>
        /// Stack pointer
        /// Register addressing range from 0x0100 to 0x01ff.
        /// This register allocates his 256 bytes of memory from top to bottom.
        /// </summary>
        public byte _sp_Register { get; private set; } = 0xff;

        /// <summary>
        /// Program counter
        /// </summary>
        public ushort _pc_Register { get; private set; } = 0x0000;

        /// <summary>
        /// Flags
        /// </summary>
        public byte _status_Register { get; private set; } = 0x00;

        /// <summary>
        /// The mapping of the processor instruction set with the insttuction implementation.
        /// </summary>
        private readonly List<Instruction> _instructionsMap;

        /// <summary>
        /// Represents the number of cycle for the current instruction.
        /// </summary>
        public int _cycles { get; private set; } = 0;

        /// <summary>
        /// Represents the current processor instruction code.
        /// </summary>
        public byte _opcode { get; private set; } = 0x00;

        /// <summary>
        /// Represent the instruction set operand;
        /// </summary>
        public ushort _operand_Address { get; set; } = 0x0000;

        /// <summary>
        /// Represents the the actual value of the operand for the given <see cref="_operand_Address"/>.
        /// </summary>
        public byte _operand_Value { get; set; } = 0x00;

        /// <summary>
        /// Defines if addressing mode is implied.
        /// </summary>
        public bool _isAMImplied { get; private set; } = false;

        /// <summary>
        /// Write data to the bus.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        public void Write(ushort address, byte data)
        {
            DataTransfer.Write(address, data);
        }

        /// <summary>
        /// Read data from the bus.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public byte Read(ushort address)
        {
            return DataTransfer.Read(address);
        }

        /// <summary>
        /// Set the flag register to 1.
        /// </summary>
        /// <param name="state"></param>
        public void SetFlag(Flags6502 state)
        {
            _status_Register = _status_Register.OR(state);
        }

        /// <summary>
        /// Update the flag register.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activated"></param>
        public void SetFlag(Flags6502 state, bool activated)
        {
            if (activated)
            {
                _status_Register = _status_Register.OR((int)state);
            }
            else
            {
                _status_Register = _status_Register.AND(~(byte)state);
            }
        }


        /// <summary>
        /// Set the register flag to 0.
        /// </summary>
        /// <param name="state"></param>
        public void ClearFlag(Flags6502 state)
        {
            _status_Register = _status_Register.AND(~(byte)state);
        }


        /// <summary>
        /// Read the value of status registry for the given flag.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public byte ReadStatusRegister(Flags6502 state)
        {
            return (_status_Register & (byte)state) == 0 ? (byte)0 : (byte)1;
        }

        /// <summary>
        /// Push value to the stack.
        /// </summary>
        /// <param name="value"></param>
        public void PushToStack(byte value)
        {
            _sp_Register--;
            Write(_sp_Register.OR(0x0100), value);
        }

        /// <summary>
        /// Pop value from the stack.
        /// </summary>
        /// <returns></returns>
        public byte PopFromStack()
        {
            byte tmp = Read(_sp_Register.OR(0x0100));
            _sp_Register++;
            return tmp;
        }

    }
}
