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
        public _6502()
        {
            InstructionsMap = MapProcessorInstructions();
        }

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

        public List<Instruction> InstructionsMap { get; set; }

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

        #region addressing mode

        public byte AM_IMP() { throw new NotImplementedException(); }
        public byte AM_ZP0() { throw new NotImplementedException(); }
        public byte AM_ZPY() { throw new NotImplementedException(); }
        public byte AM_ABS() { throw new NotImplementedException(); }
        public byte AM_ABY() { throw new NotImplementedException(); }
        public byte AM_IZX() { throw new NotImplementedException(); }
        public byte AM_IMM() { throw new NotImplementedException(); }
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
        public byte CLV() { throw new NotImplementedException(); }
        public byte DEC() { throw new NotImplementedException(); }
        public byte INC() { throw new NotImplementedException(); }
        public byte JSR() { throw new NotImplementedException(); }
        public byte LSR() { throw new NotImplementedException(); }
        public byte PHP() { throw new NotImplementedException(); }
        public byte ROR() { throw new NotImplementedException(); }
        public byte SEC() { throw new NotImplementedException(); }
        public byte STX() { throw new NotImplementedException(); }
        public byte TSX() { throw new NotImplementedException(); }
        public byte AND() { throw new NotImplementedException(); }
        public byte BEQ() { throw new NotImplementedException(); }
        public byte BPL() { throw new NotImplementedException(); }
        public byte CLC() { throw new NotImplementedException(); }
        public byte CMP() { throw new NotImplementedException(); }
        public byte DEX() { throw new NotImplementedException(); }
        public byte INX() { throw new NotImplementedException(); }
        public byte LDA() { throw new NotImplementedException(); }
        public byte NOP() { throw new NotImplementedException(); }
        public byte PLA() { throw new NotImplementedException(); }
        public byte RTI() { throw new NotImplementedException(); }
        public byte SED() { throw new NotImplementedException(); }
        public byte STY() { throw new NotImplementedException(); }
        public byte TXA() { throw new NotImplementedException(); }
        public byte ASL() { throw new NotImplementedException(); }
        public byte BIT() { throw new NotImplementedException(); }
        public byte BRK() { throw new NotImplementedException(); }
        public byte CLD() { throw new NotImplementedException(); }
        public byte CPX() { throw new NotImplementedException(); }
        public byte DEY() { throw new NotImplementedException(); }
        public byte INY() { throw new NotImplementedException(); }
        public byte LDX() { throw new NotImplementedException(); }
        public byte ORA() { throw new NotImplementedException(); }
        public byte PLP() { throw new NotImplementedException(); }
        public byte RTS() { throw new NotImplementedException(); }
        public byte SEI() { throw new NotImplementedException(); }
        public byte TAX() { throw new NotImplementedException(); }
        public byte TXS() { throw new NotImplementedException(); }
        public byte BCC() { throw new NotImplementedException(); }
        public byte BMI() { throw new NotImplementedException(); }
        public byte BVC() { throw new NotImplementedException(); }
        public byte CLI() { throw new NotImplementedException(); }
        public byte CPY() { throw new NotImplementedException(); }
        public byte EOR() { throw new NotImplementedException(); }
        public byte JMP() { throw new NotImplementedException(); }
        public byte LDY() { throw new NotImplementedException(); }
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

        public void SetStatusRegister(Flags6502 state, bool clear)
        {
            if (clear)
            {
                Status_Register = (byte)(Status_Register & ~(byte)state);
            }
            else
            {
                Status_Register = (byte)(Status_Register | (byte)state);
            }
        }

        public byte ReadStatusRegister(Flags6502 state)
        {
            return (Status_Register & (byte)state) == 0 ? (byte)0 : (byte)1;
        }

        /// <summary>
        /// Map the instruction from the available instruction set 
        /// to the corresponding implementation.
        /// </summary>
        /// <returns></returns>
        private List<Instruction> MapProcessorInstructions()
        {
            List<Instruction> map = new List<Instruction>() {
                new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.BPL, BPL, AM_REL, 2), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.JSR, JSR, AM_ABS, 6), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.BMI, BMI, AM_REL, 2), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.RTI, RTI, AM_IMP, 6), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.BVC, BVC, AM_REL, 2), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.RTS, RTS, AM_IMP, 6), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.BVS, BVS, AM_REL, 2), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.UNK, XXX, AM_XXX, 0), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.BCC, BCC, AM_REL, 2), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.LDY, LDY, AM_IMM, 2), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.BCS, BCS, AM_REL, 2), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.CPY, CPY, AM_IMM, 2), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.BNE, BNE, AM_REL, 2), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.CPX, CPX, AM_IMM, 2), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
                new Instruction(Instruction.BEQ, BEQ, AM_REL, 2), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7), new Instruction(Instruction.BRK, BRK, AM_IMP, 7),
            };

            return map;
        }
    }
}
