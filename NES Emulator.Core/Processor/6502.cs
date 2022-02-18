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
        private byte _acc_Register = 0x00;
        private byte _x_Register = 0x00;
        private byte _y_Register = 0x00;

        /// <summary>
        /// Stack pointer
        /// </summary>
        private ushort _sp_Register = 0x01ff;

        /// <summary>
        /// Program counter
        /// </summary>
        private ushort _pc_Register = 0x0000;

        /// <summary>
        /// Flags
        /// </summary>
        private byte _status_Register = 0x00;

        /// <summary>
        /// The mapping of the processor instruction set with the insttuction implementation.
        /// </summary>
        private readonly List<Instruction> _instructionsMap;

        /// <summary>
        /// Represents the number of cycle for the current instruction.
        /// </summary>
        private int _cycles = 0;

        /// <summary>
        /// Represents the current processor instruction code.
        /// </summary>
        private byte _opcode = 0x00;

        /// <summary>
        /// Represent the instruction set operand;
        /// </summary>
        private ushort _operand_Address = 0x0000;

        /// <summary>
        /// Represents the the actual value of the operand for the given <see cref="_operand_Address"/>.
        /// </summary>
        private byte _operand_Value = 0x00;

        /// <summary>
        /// Defines if addressing mode is implied.
        /// </summary>
        private bool _isAMImplied = false;

        #region events
        /// <summary>
        /// Represents the clock.
        /// See processor sheet for more information.
        /// </summary>
        public void Clock()
        {

            if (_cycles == 0)
            {
                // Means processor must read a new instruction code.
                _opcode = Read(_pc_Register);

                // Set the unused flag register to true.
                ClearFlag(Flags6502.Unused);

                // Get the instruction by using the instruction code.
                Instruction instruction = _instructionsMap[_opcode];
                _pc_Register++;

                _cycles = instruction.Cycle;

                // CHECK
                int additionalCycleFromAM = instruction.AddressMode();
                int additionalCycleFromOP = instruction.Operate();

                // CHECK
                _cycles += additionalCycleFromAM & additionalCycleFromOP;

                // Set the unused flag register to true.
                ClearFlag(Flags6502.Unused);
            }
            // Decrements the number of cycle for the current procesor instruction.
            _cycles--;
        }

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
            _status_Register = _status_Register | state;
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
            Write(_sp_Register, value);
        }

        /// <summary>
        /// Pop value from the stack.
        /// </summary>
        /// <returns></returns>
        public byte PopFromStack()
        {
            byte tmp = Read(_sp_Register);
            _sp_Register++;
            return tmp;
        }

    }
}
