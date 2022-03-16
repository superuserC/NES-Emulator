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
        public byte IRQ()
        {
            if(ReadStatusRegister(Flags6502.IRQDisable) == 0)
            {
                PushToStack((byte)(_pc_Register >> 8));
                PushToStack((byte)_pc_Register);
                SetFlag(Flags6502.Unused);
                SetFlag(Flags6502.IRQDisable);
                ClearFlag(Flags6502.Break);
                PushToStack(_status_Register);

                byte resetHighByte = Read(0xffff);
                byte resetLowByte = Read(0xfffe);
                _pc_Register = (ushort)(resetHighByte << 8 | (ushort)resetLowByte);
                return 7;
            }
            return 0;
        }

        /// <summary>
        /// See processor sheet.
        /// </summary>
        public byte NMI()
        {
            PushToStack((byte)(_pc_Register >> 8));
            PushToStack((byte)_pc_Register);
            SetFlag(Flags6502.Unused);
            SetFlag(Flags6502.IRQDisable);
            ClearFlag(Flags6502.Break);
            PushToStack(_status_Register);

            byte resetHighByte = Read(0xfffb);
            byte resetLowByte = Read(0xfffa);
            _pc_Register = (ushort)(resetHighByte << 8 | (ushort)resetLowByte);
            return 7;
        }

        /// <summary>
        /// Reset register
        /// Set PC to 0xfffd - 0xfffc
        /// </summary>
        public byte Reset()
        {
            ResetRegisters();
            byte resetHighByte = Read(0xfffd);
            byte resetLowByte = Read(0xfffc);
            _pc_Register = (ushort)(resetHighByte << 8 | (ushort)resetLowByte);
            return 8;
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
