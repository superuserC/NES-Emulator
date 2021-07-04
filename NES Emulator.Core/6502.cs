using Dawn;
using NES_Emulator.Core.Extensions;
using NES_Emulator.Core.Interfaces;
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
    public class _6502
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
        private byte _sp_Register = 0xFF;

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

        #region addressing mode

        /// <summary>
        /// No operand required for implied addressing mode.
        /// Operation is self contained in instruction mnemonic.
        /// </summary>
        /// <returns></returns>
        public byte AM_IMP()
        {
            _isAMImplied = true;
            // this is required in case acc register is needed by instruction.
            _operand_Value = _acc_Register;
            return 0;
        }

        /// <summary>
        /// Operand is the value of the next byte.
        /// </summary>
        /// <returns></returns>
        public byte AM_IMM()
        {
            _isAMImplied = false;
            _operand_Address = _pc_Register;
            _operand_Value = Read(_operand_Address);
            _pc_Register++;
            return 0;
        }

        public byte AM_ZP0()
        {
            _isAMImplied = false;
            throw new NotImplementedException();
        }
        public byte AM_ZPY()
        {
            _isAMImplied = false;
            throw new NotImplementedException();
        }
        public byte AM_ABS()
        {
            _isAMImplied = false;
            throw new NotImplementedException();
        }
        public byte AM_ABY()
        {
            _isAMImplied = false;
            throw new NotImplementedException();
        }
        public byte AM_IZX()
        {
            _isAMImplied = false;
            throw new NotImplementedException();
        }
        public byte AM_ZPX()
        {
            _isAMImplied = false;
            _isAMImplied = false; throw new NotImplementedException();
        }
        public byte AM_REL()
        {
            _isAMImplied = false;
            throw new NotImplementedException();
        }
        public byte AM_ABX()
        {
            _isAMImplied = false;
            throw new NotImplementedException();
        }
        public byte AM_IND()
        {
            _isAMImplied = false;
            throw new NotImplementedException();
        }
        public byte AM_IZY()
        {
            _isAMImplied = false;
            throw new NotImplementedException();
        }
        public byte AM_XXX()
        {
            _isAMImplied = false;
            throw new NotImplementedException();
        }

        #endregion

        #region instructions

        public byte ADC() { throw new NotImplementedException(); }
        public byte BCS() { throw new NotImplementedException(); }
        public byte BNE() { throw new NotImplementedException(); }
        public byte BVS() { throw new NotImplementedException(); }

        /// <summary>
        /// Clear overflow flag.
        /// </summary>
        /// <returns></returns>
        public byte CLV()
        {
            ClearFlag(Flags6502.Overflow);
            return 0;
        }

        /// <summary>
        /// Decrement memory by one.
        /// </summary>
        /// <returns></returns>
        public byte DEC()
        {
            byte data = (byte)(_operand_Value - 1);
            Write(_operand_Address, data);

            if (data.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (data.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Increment memory by one.
        /// </summary>
        /// <returns></returns>
        public byte INC()
        {
            byte tmp = _operand_Value.Add(1);
            Write(_operand_Address, tmp);

            if (tmp.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (tmp.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }
        public byte JSR() { throw new NotImplementedException(); }

        /// <summary>
        /// Shift one bit right (memory or accumulator)
        /// </summary>
        /// <returns></returns>
        public byte LSR()
        {
            // Check if LSB is 1 for carry flag.
            if ((_operand_Value & (byte)0x01) == 1)
            {
                SetFlag(Flags6502.Carry);
            }
            else
            {
                ClearFlag(Flags6502.Carry);
            }

            byte tmp = _operand_Value.SR();
            if (tmp.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            if (_isAMImplied)
            {
                _acc_Register = tmp;
            }
            else
            {
                Write(_operand_Address, tmp);
            }

            return 0;
        }

        /// <summary>
        /// Push processor status on stack
        /// </summary>
        /// <returns></returns>
        public byte PHP()
        {
            // Set the break and unused flag to 1.
            byte tmp = _status_Register.OR(0x10).OR(0x20);
            PushToStack(tmp);
            return 0;
        }

        /// <summary>
        /// Rotate one bit right (memory or accumulator).
        /// C -> [76543210] -> C
        /// </summary>
        /// <returns></returns>
        public byte ROR()
        {
            byte carryFlag = ReadStatusRegister(Flags6502.Carry);
            byte tmp = _operand_Value;
            if (tmp.AND(0x01) == 1)
            {
                SetFlag(Flags6502.Carry);
            }
            else
            {
                ClearFlag(Flags6502.Carry);
            }

            tmp = tmp.SR();
            if (carryFlag == 1)
            {
                tmp = tmp.OR(0x80);
            }
            else
            {
                tmp = tmp.AND(~0x80);
            }

            if (_isAMImplied)
            {
                _acc_Register = tmp;
            }
            else
            {
                Write(_operand_Address, tmp);
            }

            return 0;

        }

        /// <summary>
        /// Set carry flag.
        /// </summary>
        /// <returns></returns>
        public byte SEC()
        {
            SetFlag(Flags6502.Carry);
            return 0;
        }

        /// <summary>
        /// Store index X in memory
        /// </summary>
        /// <returns></returns>
        public byte STX()
        {
            Write(_operand_Address, _x_Register);
            return 0;
        }

        /// <summary>
        /// Transfer stack pointer to index X.
        /// SP -> X
        /// </summary>
        /// <returns></returns>
        public byte TSX()
        {
            _x_Register = _sp_Register;
            if (_x_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_x_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;

        }

        /// <summary>
        /// AND memory with accumulator.
        /// </summary>
        /// <returns></returns>
        public byte AND()
        {
            _acc_Register &= _operand_Value;
            if (_acc_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_acc_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        public byte BEQ() { throw new NotImplementedException(); }
        public byte BPL() { throw new NotImplementedException(); }

        /// <summary>
        /// Clear carry flag.
        /// </summary>
        /// <returns></returns>
        public byte CLC()
        {
            ClearFlag(Flags6502.Carry);
            return 0;
        }

        /// <summary>
        /// Compare memory with accumulator
        /// </summary>
        /// <returns></returns>
        public byte CMP()
        {
            byte diff = (byte)(_acc_Register - _operand_Value);
            byte sign = (byte)((diff & 1 << 7) >> 7);

            if (_acc_Register < _operand_Value)
            {
                if (diff.IsNegative())
                {
                    SetFlag(Flags6502.Negative);
                }
                else
                {
                    ClearFlag(Flags6502.Negative);
                }

                ClearFlag(Flags6502.Zero);
                ClearFlag(Flags6502.Carry);
            }
            else if (_acc_Register == _operand_Value)
            {
                SetFlag(Flags6502.Zero);
                SetFlag(Flags6502.Carry);
            }
            else
            {
                if (diff.IsNegative())
                {
                    SetFlag(Flags6502.Negative);
                }
                else
                {
                    ClearFlag(Flags6502.Negative);
                }

                ClearFlag(Flags6502.Zero);
                SetFlag(Flags6502.Carry);
            }

            return 0;
        }

        /// <summary>
        /// Decrement index X by one.
        /// </summary>
        /// <returns></returns>
        public byte DEX()
        {
            _x_Register = (byte)(_x_Register - 1);

            if (_x_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_x_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Increment index X by one.
        /// </summary>
        /// <returns></returns>
        public byte INX()
        {
            _x_Register = _x_Register.Add(1);

            if (_x_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_x_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Load accumulator with memory.
        /// </summary>
        /// <returns></returns>
        public byte LDA()
        {
            _acc_Register = _operand_Value;

            if (_operand_Value.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_operand_Value.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// No operation.
        /// </summary>
        /// <returns></returns>
        public byte NOP()
        {
            return 0;
        }

        /// <summary>
        /// Pull accumulator from stack.
        /// </summary>
        /// <returns></returns>
        public byte PLA()
        {
            _acc_Register = PopFromStack();

            if (_acc_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_acc_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Return from interrupt.
        /// pull SR, pull PC
        /// </summary>
        /// <returns></returns>
        public byte RTI()
        {
            byte tmp = PopFromStack();
            _pc_Register = PopFromStack();
            byte oldBreakStatus = ReadStatusRegister(Flags6502.Break);
            byte oldUnusedStatus = ReadStatusRegister(Flags6502.Unused);

            if (oldBreakStatus == 1)
            {
                // set break to 1.
                tmp = tmp.OR(0x10);
            }
            else
            {
                // set break to 0.
                tmp = tmp.AND(~0x10);
            }

            if (oldUnusedStatus == 1)
            {
                tmp = tmp.OR(0x20);
            }
            else
            {
                tmp = tmp.AND(~0x20);
            }

            _status_Register = tmp;
            return 0;
        }

        /// <summary>
        /// Set decimal flag.
        /// </summary>
        /// <returns></returns>
        public byte SED()
        {
            SetFlag(Flags6502.DecimalMode);
            return 0;
        }

        /// <summary>
        /// Sore index Y in memory.
        /// Y -> M
        /// </summary>
        /// <returns></returns>
        public byte STY()
        {
            Write(_operand_Address, _y_Register);
            return 0;
        }

        /// <summary>
        /// Transfer index X to accumulator.
        /// X -> A
        /// </summary>
        /// <returns></returns>
        public byte TXA()
        {
            _acc_Register = _x_Register;
            if (_acc_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_acc_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Shift left one bit (memory or accumulator).
        /// </summary>
        /// <returns></returns>
        public byte ASL()
        {

            bool setCarryFlag = _operand_Value.IsNegative();
            byte tmp = (byte)(_operand_Value << 1);

            if (tmp.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (tmp.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            if (setCarryFlag)
            {
                SetFlag(Flags6502.Carry);
            }
            else
            {
                ClearFlag(Flags6502.Carry);
            }

            return 0;
        }

        /// <summary>
        /// Test bits in memory with accumulator
        /// </summary>
        /// <returns></returns>
        public byte BIT()
        {
            byte and = (byte)(_acc_Register & _operand_Value);
            byte M7 = (byte)(_operand_Value & (1 << 7));
            byte M6 = (byte)(_operand_Value & (1 << 6));

            if (and.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            if ((M7 >> 7) == 1)
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if ((M6 >> 6) == 1)
            {
                SetFlag(Flags6502.Overflow);
            }
            else
            {
                ClearFlag(Flags6502.Overflow);
            }

            return 0;
        }
        public byte BRK() { throw new NotImplementedException(); }

        /// <summary>
        /// Clear decimal mode.
        /// </summary>
        /// <returns></returns>
        public byte CLD()
        {
            ClearFlag(Flags6502.DecimalMode);
            return 0;
        }

        /// <summary>
        /// Compare memory and index X.
        /// </summary>
        /// <returns></returns>
        public byte CPX()
        {
            byte diff = (byte)(_x_Register - _operand_Value);

            if (_x_Register < _operand_Value)
            {
                if (diff.IsNegative())
                {
                    SetFlag(Flags6502.Negative);
                }
                else
                {
                    ClearFlag(Flags6502.Negative);
                }

                ClearFlag(Flags6502.Zero);
                ClearFlag(Flags6502.Carry);
            }
            else if (_x_Register == _operand_Value)
            {
                SetFlag(Flags6502.Zero);
                SetFlag(Flags6502.Carry);
            }
            else
            {
                if (diff.IsNegative())
                {
                    SetFlag(Flags6502.Negative);
                }
                else
                {
                    ClearFlag(Flags6502.Negative);
                }

                ClearFlag(Flags6502.Zero);
                SetFlag(Flags6502.Carry);
            }

            return 0;
        }

        /// <summary>
        /// Decrement index Y by one.
        /// </summary>
        /// <returns></returns>
        public byte DEY()
        {
            _y_Register = (byte)(_y_Register - 1);

            if (_y_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_y_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Increment index Y by one.
        /// </summary>
        /// <returns></returns>
        public byte INY()
        {
            _y_Register = _y_Register.Add(1);

            if (_y_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_y_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Load index X with memory.
        /// </summary>
        /// <returns></returns>
        public byte LDX()
        {
            _x_Register = _operand_Value;

            if (_operand_Value.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_operand_Value.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Or memory with accumulator.
        /// </summary>
        /// <returns></returns>
        public byte ORA()
        {
            _acc_Register = _operand_Value.OR(_acc_Register);

            if (_acc_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_acc_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// pull processor status from stack.
        /// </summary>
        /// <returns></returns>
        public byte PLP()
        {
            byte tmp = PopFromStack();
            byte breakFlag = ReadStatusRegister(Flags6502.Break);
            byte unusedFlag = ReadStatusRegister(Flags6502.Unused);
            if (breakFlag == 1)
            {
                // set break flag to 1
                tmp = tmp.OR(0x10);
            }
            else
            {
                // set break flag to 0
                tmp = tmp.AND(~0x10);
            }

            if (unusedFlag == 1)
            {
                // set unused flag to 1
                tmp = tmp.OR(0x20);
            }
            else
            {
                // set unused flag to 0
                tmp = tmp.AND(~0x20);
            }

            _status_Register = tmp;
            return 0;
        }

        /// <summary>
        /// Return from subroutine.
        /// pull PC, PC+1 -> PC
        /// </summary>
        /// <returns></returns>
        public byte RTS()
        {
            _pc_Register = PopFromStack().Add(1);
            return 0;
        }

        /// <summary>
        /// Set interrupt disable status.
        /// </summary>
        /// <returns></returns>
        public byte SEI()
        {
            SetFlag(Flags6502.IRQDisable);
            return 0;
        }

        /// <summary>
        /// Transfer accumulator to index X
        /// A -> X
        /// </summary>
        /// <returns></returns>
        public byte TAX()
        {
            _x_Register = _acc_Register;
            if (_x_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_x_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Transfer index X to stack register.
        /// X -> SP
        /// </summary>
        /// <returns></returns>
        public byte TXS()
        {
            _sp_Register = _x_Register;
            if (_sp_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_sp_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }
        public byte BCC() { throw new NotImplementedException(); }
        public byte BMI() { throw new NotImplementedException(); }
        public byte BVC() { throw new NotImplementedException(); }

        /// <summary>
        /// Clear interrupt disable bit.
        /// </summary>
        /// <returns></returns>
        public byte CLI()
        {
            ClearFlag(Flags6502.IRQDisable);
            return 0;
        }

        /// <summary>
        /// Compare memory and index Y.
        /// </summary>
        /// <returns></returns>
        public byte CPY()
        {
            byte diff = (byte)(_y_Register - _operand_Value);

            if (_y_Register < _operand_Value)
            {
                if (diff.IsNegative())
                {
                    SetFlag(Flags6502.Negative);
                }
                else
                {
                    ClearFlag(Flags6502.Negative);
                }

                ClearFlag(Flags6502.Zero);
                ClearFlag(Flags6502.Carry);
            }
            else if (_y_Register == _operand_Value)
            {
                SetFlag(Flags6502.Zero);
                SetFlag(Flags6502.Carry);
            }
            else
            {
                if (diff.IsNegative())
                {
                    SetFlag(Flags6502.Negative);
                }
                else
                {
                    ClearFlag(Flags6502.Negative);
                }

                ClearFlag(Flags6502.Zero);
                SetFlag(Flags6502.Carry);
            }

            return 0;
        }

        /// <summary>
        /// Exclusive-OR memory with accumulator.
        /// </summary>
        /// <returns></returns>
        public byte EOR()
        {
            _acc_Register ^= _operand_Value;

            if (_acc_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_acc_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }
        public byte JMP() { throw new NotImplementedException(); }

        /// <summary>
        /// Load index Y with memory.
        /// </summary>
        /// <returns></returns>
        public byte LDY()
        {
            _y_Register = _operand_Value;

            if (_operand_Value.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_operand_Value.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Push cccumulator on stack
        /// </summary>
        /// <returns></returns>
        public byte PHA()
        {
            PushToStack(_acc_Register);
            return 0;
        }

        /// <summary>
        /// Rotate one bit left (memory or accumulator)
        /// C <- [76543210] <- C
        /// </summary>
        /// <returns></returns>
        public byte ROL()
        {
            byte carryFlag = ReadStatusRegister(Flags6502.Carry);
            byte tmp = _operand_Value;
            if (tmp.IsNegative())
            {
                SetFlag(Flags6502.Carry);
            }
            else
            {
                ClearFlag(Flags6502.Carry);
            }

            tmp = tmp.SL();
            if (carryFlag == 1)
            {
                tmp = tmp.OR(0x01);
            }
            else
            {
                tmp = tmp.AND(~0x01);
            }

            if (_isAMImplied)
            {
                _acc_Register = tmp;
            }
            else
            {
                Write(_operand_Address, tmp);
            }

            return 0;
        }


        public byte SBC() { throw new NotImplementedException(); }

        /// <summary>
        /// Store accumulator in memory.
        /// A -> M
        /// </summary>
        /// <returns></returns>
        public byte STA()
        {
            Write(_operand_Address, _acc_Register);
            return 0;
        }

        /// <summary>
        /// Transfer accumulator to index Y
        /// A -> Y
        /// </summary>
        /// <returns></returns>
        public byte TAY()
        {
            _y_Register = _acc_Register;
            if (_y_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_y_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Transfer index Y to accumulator.
        /// Y -> A
        /// </summary>
        /// <returns></returns>
        public byte TYA()
        {
            _acc_Register = _y_Register;
            if (_acc_Register.IsNegative())
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (_acc_Register.IsZero())
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        /// <summary>
        /// Invalid instructions.
        /// </summary>
        /// <returns></returns>
        public byte XXX()
        {
            throw new NotImplementedException();
        }

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
            _status_Register = _status_Register.OR(state);
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
        /// Map the instruction from the available instruction set 
        /// to the corresponding implementation.
        /// </summary>
        /// <returns></returns>
        private List<Instruction> MapProcessorInstructions()
        {
            List<Instruction> map = new List<Instruction>() {
                new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.ORA, ORA, AM_IZX, 6),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.ORA, ORA, AM_ZP0, 3), new Instruction(Instruction.ASL, ASL, AM_ZP0, 5), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.PHP, PHP, AM_IMP, 3), new Instruction(Instruction.ORA, ORA, AM_IMM, 2), new Instruction(Instruction.ASL, ASL, AM_IMP, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.ORA, ORA, AM_ABS, 4), new Instruction(Instruction.ASL, ASL, AM_ABS, 6), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.BPL, BPL, AM_REL, 2), new Instruction(Instruction.ORA, ORA, AM_IZY, 5),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.ORA, ORA, AM_ZPX, 4), new Instruction(Instruction.ASL, ASL, AM_ZPX, 6), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.CLC, CLC, AM_IMP, 2), new Instruction(Instruction.ORA, ORA, AM_ABY, 4), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.ORA, ORA, AM_ABX, 4), new Instruction(Instruction.ASL, ASL, AM_ABX, 7), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.JSR, JSR, AM_ABS, 6), new Instruction(Instruction.AND, AND, AM_IZX, 6),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.BIT, BIT, AM_ZP0, 3), new Instruction(Instruction.AND, AND, AM_ZP0, 3), new Instruction(Instruction.ROL, ROL, AM_ZP0, 5), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.PLP, PLP, AM_IMP, 4), new Instruction(Instruction.AND, AND, AM_IMM, 2), new Instruction(Instruction.ROL, ROL, AM_IMP, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.BIT, BIT, AM_ABS, 4), new Instruction(Instruction.AND, AND, AM_ABS, 4), new Instruction(Instruction.ROL, ROL, AM_ABS, 6), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.BMI, BMI, AM_REL, 2), new Instruction(Instruction.AND, AND, AM_IZY, 5),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.AND, AND, AM_ZPX, 4), new Instruction(Instruction.ROL, ROL, AM_ZPX, 6), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.SEC, SEC, AM_IMP, 2), new Instruction(Instruction.AND, AND, AM_ABY, 4), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.AND, AND, AM_ABX, 4), new Instruction(Instruction.ROL, ROL, AM_ABX, 7), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.RTI, RTI, AM_IMP, 6), new Instruction(Instruction.EOR, EOR, AM_IZX, 6),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.EOR, EOR, AM_ZP0, 3), new Instruction(Instruction.LSR, LSR, AM_ZP0, 5), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.PHA, PHA, AM_IMP, 3), new Instruction(Instruction.EOR, EOR, AM_IMM, 2), new Instruction(Instruction.LSR, LSR, AM_IMP, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.JMP, JMP, AM_ABS, 3), new Instruction(Instruction.EOR, EOR, AM_ABS, 4), new Instruction(Instruction.LSR, LSR, AM_ABS, 6), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.BVC, BVC, AM_REL, 2), new Instruction(Instruction.EOR, EOR, AM_IZY, 5),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.EOR, EOR, AM_ZPX, 4), new Instruction(Instruction.LSR, LSR, AM_ZPX, 6), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.CLI, CLI, AM_IMP, 2), new Instruction(Instruction.EOR, EOR, AM_ABY, 4), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.EOR, EOR, AM_ABX, 4), new Instruction(Instruction.LSR, LSR, AM_ABX, 7), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.RTS, RTS, AM_IMP, 6), new Instruction(Instruction.ADC, ADC, AM_IZX, 6),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.ADC, ADC, AM_ZP0, 3), new Instruction(Instruction.ROR, ROR, AM_ZP0, 5), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.PLA, PLA, AM_IMP, 4), new Instruction(Instruction.ADC, ADC, AM_IMM, 2), new Instruction(Instruction.ROR, ROR, AM_IMP, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.JMP, JMP, AM_IND, 5), new Instruction(Instruction.ADC, ADC, AM_ABS, 4), new Instruction(Instruction.ROR, ROR, AM_ABS, 6), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.BVS, BVS, AM_REL, 2), new Instruction(Instruction.ADC, ADC, AM_IZY, 5),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.ADC, ADC, AM_ZPX, 4), new Instruction(Instruction.ROR, ROR, AM_ZPX, 6), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.SEI, SEI, AM_IMP, 2), new Instruction(Instruction.ADC, ADC, AM_ABY, 4), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.ADC, ADC, AM_ABX, 4), new Instruction(Instruction.ROR, ROR, AM_ABX, 7), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.STA, STA, AM_IZX, 6),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.STY, STY, AM_ZP0, 3), new Instruction(Instruction.STA, STA, AM_ZP0, 3), new Instruction(Instruction.STX, STX, AM_ZP0, 3), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.DEY, DEY, AM_IMP, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.TXA, TXA, AM_IMP, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.STY, STY, AM_ABS, 4), new Instruction(Instruction.STA, STA, AM_ABS, 4), new Instruction(Instruction.STX, STX, AM_ABS, 4), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.BCC, BCC, AM_REL, 2), new Instruction(Instruction.STA, STA, AM_IZY, 6),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.STY, STY, AM_ZPX, 4), new Instruction(Instruction.STA, STA, AM_ZPX, 4), new Instruction(Instruction.STX, STX, AM_ZPY, 4), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.TYA, TYA, AM_IMP, 2), new Instruction(Instruction.STA, STA, AM_ABY, 5), new Instruction(Instruction.TXS, TXS, AM_IMP, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.STA, STA, AM_ABX, 5), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.LDY, LDY, AM_IMM, 2), new Instruction(Instruction.LDA, LDA, AM_IZX, 6),new Instruction(Instruction.LDX, LDX, AM_IMM, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.LDY, LDY, AM_ZP0, 3), new Instruction(Instruction.LDA, LDA, AM_ZP0, 3), new Instruction(Instruction.LDX, LDX, AM_ZP0, 3), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.TAY, TAY, AM_IMP, 2), new Instruction(Instruction.LDA, LDA, AM_IMM, 2), new Instruction(Instruction.TAX, TAX, AM_IMP, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.LDY, LDY, AM_ABS, 4), new Instruction(Instruction.LDA, LDA, AM_ABS, 4), new Instruction(Instruction.LDX, LDX, AM_ABS, 4), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.BCS, BCS, AM_REL, 2), new Instruction(Instruction.LDA, LDA, AM_IZY, 5),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.LDY, LDY, AM_ZPX, 4), new Instruction(Instruction.LDA, LDA, AM_ZPX, 4), new Instruction(Instruction.LDX, LDX, AM_ZPY, 4), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.CLV, CLV, AM_IMP, 2), new Instruction(Instruction.LDA, LDA, AM_ABY, 4), new Instruction(Instruction.TSX, TSX, AM_IMP, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.LDY, LDY, AM_ABX, 4), new Instruction(Instruction.LDA, LDA, AM_ABX, 4), new Instruction(Instruction.LDX, LDX, AM_ABY, 4), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.CPY, CPY, AM_IMM, 2), new Instruction(Instruction.CMP, CMP, AM_IZX, 6),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.CPY, CPY, AM_ZP0, 3), new Instruction(Instruction.CMP, CMP, AM_ZP0, 3), new Instruction(Instruction.DEC, DEC, AM_ZP0, 5), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.INY, INY, AM_IMP, 2), new Instruction(Instruction.CMP, CMP, AM_IMM, 2), new Instruction(Instruction.DEX, DEX, AM_IMP, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.CPY, CPY, AM_ABS, 4), new Instruction(Instruction.CMP, CMP, AM_ABS, 4), new Instruction(Instruction.DEC, DEC, AM_ABS, 3), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.BNE, BNE, AM_REL, 2), new Instruction(Instruction.CMP, CMP, AM_IZY, 5),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.CMP, CMP, AM_ZPX, 4), new Instruction(Instruction.DEC, DEC, AM_ZPX, 6), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.CLD, CLD, AM_IMP, 2), new Instruction(Instruction.CMP, CMP, AM_ABY, 4), new Instruction(Instruction.UNK, XXX, AM_XXX, 7), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.CMP, CMP, AM_ABX, 4), new Instruction(Instruction.DEC, DEC, AM_ABX, 7), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.CPX, CPX, AM_IMM, 2), new Instruction(Instruction.SBC, SBC, AM_IZX, 6),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.CPX, CPX, AM_ZP0, 3), new Instruction(Instruction.SBC, SBC, AM_ZP0, 3), new Instruction(Instruction.INC, INC, AM_ZP0, 5), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.INX, INX, AM_IMP, 2), new Instruction(Instruction.SBC, SBC, AM_IMM, 2), new Instruction(Instruction.NOP, NOP, AM_IMP, 2), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.CPX, CPX, AM_ABS, 4), new Instruction(Instruction.SBC, SBC, AM_ABS, 4), new Instruction(Instruction.INC, INC, AM_ABS, 6), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
                new Instruction(Instruction.BEQ, BEQ, AM_REL, 2), new Instruction(Instruction.SBC, SBC, AM_IZY, 5),new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.SBC, SBC, AM_ZP0, 4), new Instruction(Instruction.INC, INC, AM_ZPX, 6), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.SED, SED, AM_IMP, 2), new Instruction(Instruction.SBC, SBC, AM_ABY, 4), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.SBC, SBC, AM_ABX, 4), new Instruction(Instruction.INC, INC, AM_ABX, 7), new Instruction(Instruction.UNK, XXX, AM_XXX, 0),
            };
            return map;
        }

        /// <summary>
        /// Provides the address targeted by stack pointer register.
        /// Address can be in the range 0x0100 to 0x01FF.
        /// </summary>
        /// <param name="stackPointer"></param>
        /// <returns></returns>
        public ushort GetStackPointerAddress()
        {
            return _sp_Register.Add(0x0100);
        }


        /// <summary>
        /// Push value to the stack.
        /// </summary>
        /// <param name="value"></param>
        public void PushToStack(byte value)
        {
            _sp_Register--;
            Write(GetStackPointerAddress(), value);
        }

        /// <summary>
        /// Pop value from the stack.
        /// </summary>
        /// <returns></returns>
        public byte PopFromStack()
        {
            byte tmp = Read(GetStackPointerAddress());
            _sp_Register++;
            return tmp;
        }

    }
}
