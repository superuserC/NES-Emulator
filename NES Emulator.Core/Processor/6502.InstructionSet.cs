﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core.Processor
{
    public partial class _6502
    {
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
    }
}
