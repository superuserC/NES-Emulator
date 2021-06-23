using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core
{
    public delegate byte Operate();
    public delegate byte AddressMode();
    public class Instruction
    {

        public Instruction(string name, Operate operate, AddressMode addressMode, byte cycle)
        {
            Name = name;
            Operate = operate;
            AddressMode = addressMode;
            Cycle = cycle;
        }

        /// <summary>
        /// Name of the instruction from the instruction set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Operation to perform for the given instruction.
        /// </summary>
        public Operate Operate { get; set; }

        /// <summary>
        /// Mechanism to access address from the instruction;
        /// </summary>
        public AddressMode AddressMode { get; set; }

        public byte Cycle { get; set; }

        #region instructions mnemonic
        public const string UNK = "UNK";
        public const string ADC = "ADC";
        public const string AND = "AND";
        public const string ASL = "ASL";
        public const string BCC = "BCC";
        public const string BCS = "BCS";
        public const string BEQ = "BEQ";
        public const string BIT = "BIT";
        public const string BMI = "BMI";
        public const string BNE = "BNE";
        public const string BPL = "BPL";
        public const string BRK = "BRK";
        public const string BVC = "BVC";
        public const string BVS = "BVS";
        public const string CLC = "CLC";
        public const string CLD = "CLD";
        public const string CLI = "CLI";
        public const string CLV = "CLV";
        public const string CMP = "CMP";
        public const string CPX = "CPX";
        public const string CPY = "CPY";
        public const string DEC = "DEC";
        public const string DEX = "DEX";
        public const string DEY = "DEY";
        public const string EOR = "EOR";
        public const string INC = "INC";
        public const string INX = "INX";
        public const string INY = "INY";
        public const string JMP = "JMP";
        public const string JSR = "JSR";
        public const string LDA = "LDA";
        public const string LDX = "LDX";
        public const string LDY = "LDY";
        public const string LSR = "LSR";
        public const string NOP = "NOP";
        public const string ORA = "ORA";
        public const string PHA = "PHA";
        public const string PHP = "PHP";
        public const string PLA = "PLA";
        public const string PLP = "PLP";
        public const string ROL = "ROL";
        public const string ROR = "ROR";
        public const string RTI = "RTI";
        public const string RTS = "RTS";
        public const string SBC = "SBC";
        public const string SEC = "SEC";
        public const string SED = "SED";
        public const string SEI = "SEI";
        public const string STA = "STA";
        public const string STX = "STX";
        public const string STY = "STY";
        public const string TAX = "TAX";
        public const string TAY = "TAY";
        public const string TSX = "TSX";
        public const string TXA = "TXA";
        public const string TXS = "TXS";
        public const string TYA = "TYA";

        #endregion
    }
}
