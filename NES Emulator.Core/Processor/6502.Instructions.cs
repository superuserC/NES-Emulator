using NES_Emulator.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core.Processor
{
    public partial class _6502
    {
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
            byte data = _operand_Value.Substract(1);
            Write(_operand_Address, data);
            SetFlag(Flags6502.Negative, data.IsNegative());
            SetFlag(Flags6502.Zero, data.IsZero());
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
            SetFlag(Flags6502.Negative, tmp.IsNegative());
            SetFlag(Flags6502.Zero, tmp.IsZero());
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
            SetFlag(Flags6502.Carry, _operand_Value.AND(0x01) == 1);
            byte tmp = _operand_Value.SR();
            SetFlag(Flags6502.Zero, tmp.IsZero());
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
            SetFlag(Flags6502.Carry, tmp.AND(0x01) == 1);
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
            SetFlag(Flags6502.Negative, _x_Register.IsNegative());
            SetFlag(Flags6502.Zero, _x_Register.IsZero());
            return 0;

        }

        /// <summary>
        /// AND memory with accumulator.
        /// </summary>
        /// <returns></returns>
        public byte AND()
        {
            _acc_Register &= _operand_Value;
            SetFlag(Flags6502.Negative, _acc_Register.IsNegative());
            SetFlag(Flags6502.Zero, _acc_Register.IsZero());
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
            if (_acc_Register < _operand_Value)
            {
                SetFlag(Flags6502.Negative, diff.IsNegative());
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
                SetFlag(Flags6502.Negative, diff.IsNegative());
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
            SetFlag(Flags6502.Negative, _x_Register.IsNegative());
            SetFlag(Flags6502.Zero, _x_Register.IsZero());
            return 0;
        }

        /// <summary>
        /// Increment index X by one.
        /// </summary>
        /// <returns></returns>
        public byte INX()
        {
            _x_Register = _x_Register.Add(1);
            SetFlag(Flags6502.Negative, _x_Register.IsNegative());
            SetFlag(Flags6502.Zero, _x_Register.IsZero());
            return 0;
        }

        /// <summary>
        /// Load accumulator with memory.
        /// </summary>
        /// <returns></returns>
        public byte LDA()
        {
            _acc_Register = _operand_Value;
            SetFlag(Flags6502.Negative, _operand_Value.IsNegative());
            SetFlag(Flags6502.Zero, _operand_Value.IsZero());
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
            SetFlag(Flags6502.Negative, _acc_Register.IsNegative());
            SetFlag(Flags6502.Zero, _acc_Register.IsZero());
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
            SetFlag(Flags6502.Negative, _acc_Register.IsNegative());
            ClearFlag(Flags6502.Negative);
            SetFlag(Flags6502.Zero, _acc_Register.IsZero());
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
            SetFlag(Flags6502.Negative, tmp.IsNegative());
            SetFlag(Flags6502.Zero, tmp.IsZero());
            SetFlag(Flags6502.Carry, setCarryFlag);
            return 0;
        }

        /// <summary>
        /// Test bits in memory with accumulator
        /// </summary>
        /// <returns></returns>
        public byte BIT()
        {
            byte and = _acc_Register.AND(_operand_Value);
            byte M7 = _operand_Value.AND(1 << 7);
            byte M6 = _operand_Value.AND(1 << 6);
            SetFlag(Flags6502.Zero, and.IsZero());
            SetFlag(Flags6502.Negative, (M7 >> 7) == 1);
            SetFlag(Flags6502.Overflow, (M6 >> 6) == 1);
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
                SetFlag(Flags6502.Negative, diff.IsNegative());
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
                SetFlag(Flags6502.Negative, diff.IsNegative());
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
            SetFlag(Flags6502.Negative, _y_Register.IsNegative());
            SetFlag(Flags6502.Zero, _y_Register.IsZero());
            return 0;
        }

        /// <summary>
        /// Increment index Y by one.
        /// </summary>
        /// <returns></returns>
        public byte INY()
        {
            _y_Register = _y_Register.Add(1);
            SetFlag(Flags6502.Negative, _y_Register.IsNegative());
            SetFlag(Flags6502.Zero, _y_Register.IsZero());
            return 0;
        }

        /// <summary>
        /// Load index X with memory.
        /// </summary>
        /// <returns></returns>
        public byte LDX()
        {
            _x_Register = _operand_Value;
            SetFlag(Flags6502.Negative, _operand_Value.IsNegative());
            SetFlag(Flags6502.Zero, _operand_Value.IsZero());
            return 0;
        }

        /// <summary>
        /// Or memory with accumulator.
        /// </summary>
        /// <returns></returns>
        public byte ORA()
        {
            _acc_Register = _operand_Value.OR(_acc_Register);
            SetFlag(Flags6502.Negative, _acc_Register.IsNegative());
            SetFlag(Flags6502.Zero, _acc_Register.IsZero());
            return 0;
        }

        /// <summary>
        /// Pull processor status from stack.
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
            SetFlag(Flags6502.Negative, _x_Register.IsNegative());
            SetFlag(Flags6502.Zero, _x_Register.IsZero());
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
            SetFlag(Flags6502.Negative, _sp_Register.IsNegative());
            SetFlag(Flags6502.Zero, _sp_Register.IsZero());
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
            byte diff = _y_Register.Substract(_operand_Value);
            if (_y_Register < _operand_Value)
            {
                SetFlag(Flags6502.Negative, diff.IsNegative());
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
                SetFlag(Flags6502.Negative, diff.IsNegative());
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
            SetFlag(Flags6502.Negative, _acc_Register.IsNegative());
            SetFlag(Flags6502.Zero, _acc_Register.IsZero());
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
            SetFlag(Flags6502.Negative, _y_Register.IsNegative());
            SetFlag(Flags6502.Zero, _y_Register.IsZero());
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
            SetFlag(Flags6502.Carry, tmp.IsNegative());
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
            SetFlag(Flags6502.Negative, _y_Register.IsNegative());
            SetFlag(Flags6502.Zero, _y_Register.IsZero());
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
            SetFlag(Flags6502.Negative, _acc_Register.IsNegative());
            SetFlag(Flags6502.Zero, _acc_Register.IsZero());
            return 0;
        }

        /// <summary>
        /// Invalid instruction.
        /// </summary>
        /// <returns></returns>
        public byte XXX() => throw new NotImplementedException("Invalid instruction.");
    }
}
