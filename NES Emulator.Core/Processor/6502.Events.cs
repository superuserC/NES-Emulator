using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core.Processor
{
    public partial class _6502
    {
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
        public void IRQ() => throw new NotImplementedException();

        /// <summary>
        /// See processor sheet.
        /// </summary>
        public void NMI() => throw new NotImplementedException();

        /// <summary>
        /// See processor sheet.
        /// </summary>
        public void Reset()
        {
            ResetRegisters();
            byte resetHighByte = Read(0xfffd);
            byte resetLowByte = Read(0xfffc);
            _pc_Register = (ushort)(resetHighByte << 8 | (ushort)resetLowByte);
        }

        private void ResetRegisters()
        {
            _acc_Register = 0;
            _x_Register = 0;
            _y_Register = 0;
            _status_Register = 0;
            _sp_Register = 0;
        }
    }
}
