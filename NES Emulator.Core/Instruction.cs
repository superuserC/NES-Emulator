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
    }
}
