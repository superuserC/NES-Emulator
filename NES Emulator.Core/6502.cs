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
        private byte _sp_Register = 0x00;

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
            _operand_Address = _pc_Register;
            _operand_Value = Read(_operand_Address);
            _pc_Register++;
            return 0;
        }

        public byte AM_ZP0() { throw new NotImplementedException(); }
        public byte AM_ZPY() { throw new NotImplementedException(); }
        public byte AM_ABS() { throw new NotImplementedException(); }
        public byte AM_ABY() { throw new NotImplementedException(); }
        public byte AM_IZX() { throw new NotImplementedException(); }
        public byte AM_ZPX() { throw new NotImplementedException(); }
        public byte AM_REL() { throw new NotImplementedException(); }
        public byte AM_ABX() { throw new NotImplementedException(); }
        public byte AM_IND() { throw new NotImplementedException(); }
        public byte AM_IZY() { throw new NotImplementedException(); }
        public byte AM_XXX() { throw new NotImplementedException(); }

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

            if (IsNegative(data))
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (IsZero(data))
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
        public byte LSR() { throw new NotImplementedException(); }
        public byte PHP() { throw new NotImplementedException(); }
        public byte ROR() { throw new NotImplementedException(); }

        /// <summary>
        /// Set carry flag.
        /// </summary>
        /// <returns></returns>
        public byte SEC()
        {
            SetFlag(Flags6502.Carry);
            return 0;
        }
        public byte STX() { throw new NotImplementedException(); }
        public byte TSX() { throw new NotImplementedException(); }

        /// <summary>
        /// AND memory with accumulator.
        /// </summary>
        /// <returns></returns>
        public byte AND()
        {
            _acc_Register &= _operand_Value;
            if (IsNegative(_acc_Register))
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (IsZero(_acc_Register))
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
                if (IsNegative(diff))
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
                if (IsNegative(diff))
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

            if (IsNegative(_x_Register))
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (IsZero(_x_Register))
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

            if (IsNegative(_operand_Value))
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (IsZero(_operand_Value))
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }
        public byte NOP() { throw new NotImplementedException(); }
        public byte PLA() { throw new NotImplementedException(); }
        public byte RTI() { throw new NotImplementedException(); }

        /// <summary>
        /// Set decimal flag.
        /// </summary>
        /// <returns></returns>
        public byte SED()
        {
            SetFlag(Flags6502.DecimalMode);
            return 0;
        }

        public byte STY() { throw new NotImplementedException(); }
        public byte TXA() { throw new NotImplementedException(); }

        /// <summary>
        /// Shift left one bit (memory or accumulator).
        /// </summary>
        /// <returns></returns>
        public byte ASL()
        {

            bool setCarryFlag = IsNegative(_operand_Value);
            byte tmp = (byte)(_operand_Value << 1);

            if (IsNegative(tmp))
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (IsZero(tmp))
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

            if (IsZero(and))
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
                if (IsNegative(diff))
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
                if (IsNegative(diff))
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

            if (IsNegative(_y_Register))
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (IsZero(_y_Register))
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

            if (IsNegative(_operand_Value))
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (IsZero(_operand_Value))
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }

        public byte ORA() { throw new NotImplementedException(); }
        public byte PLP() { throw new NotImplementedException(); }
        public byte RTS() { throw new NotImplementedException(); }

        /// <summary>
        /// Set interrupt disable status.
        /// </summary>
        /// <returns></returns>
        public byte SEI()
        {
            SetFlag(Flags6502.IRQDisable);
            return 0;
        }
        public byte TAX() { throw new NotImplementedException(); }
        public byte TXS() { throw new NotImplementedException(); }
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
                if (IsNegative(diff))
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
                if (IsNegative(diff))
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

            if (IsNegative(_acc_Register))
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (IsZero(_acc_Register))
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

            if (IsNegative(_operand_Value))
            {
                SetFlag(Flags6502.Negative);
            }
            else
            {
                ClearFlag(Flags6502.Negative);
            }

            if (IsZero(_operand_Value))
            {
                SetFlag(Flags6502.Zero);
            }
            else
            {
                ClearFlag(Flags6502.Zero);
            }

            return 0;
        }
        public byte PHA() { throw new NotImplementedException(); }
        public byte ROL() { throw new NotImplementedException(); }
        public byte SBC() { throw new NotImplementedException(); }
        public byte STA() { throw new NotImplementedException(); }
        public byte TAY() { throw new NotImplementedException(); }
        public byte TYA() { throw new NotImplementedException(); }

        /// <summary>
        /// Neede for invalid instructions.
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
        /// Check if the MSB is equal to 1.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsNegative(byte data)
        {
            return (data & (byte)0x80) == 0x80;
        }

        /// <summary>
        /// Check if data is equal to zero;
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsZero(byte data)
        {
            return data == 0x00;
        }
    }
}
