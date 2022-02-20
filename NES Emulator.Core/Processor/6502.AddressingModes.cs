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
        /// No operand required for implied addressing mode.
        /// Operation is self contained in instruction mnemonic.
        /// Also used for Acc mode.
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

        /// <summary>
        /// Zero page.
        /// hi-byte is zero, address = $00LL
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public byte AM_ZP0()
        {
            _isAMImplied = false;
            _operand_Address = (ushort)(0x00ff & Read(_pc_Register));
            _pc_Register++;
            return 0;
        }

        /// <summary>
        /// Zero page, x indexed.
        /// </summary>
        /// <returns></returns>
        public byte AM_ZPX()
        {
            _isAMImplied = false;
            _operand_Address = (ushort)(0x00ff & (Read(_pc_Register) + _x_Register));
            _pc_Register++;
            return 0;
        }

        /// <summary>
        /// Zero page, y indexed.
        /// </summary>
        /// <returns></returns>
        public byte AM_ZPY()
        {
            _isAMImplied = false;
            _operand_Address = (ushort)(0x00ff & (Read(_pc_Register) + _y_Register));
            _pc_Register++;
            return 0;
        }

        /// <summary>
        /// Absolute.
        /// Operand is address $LLHH.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public byte AM_ABS()
        {
            _isAMImplied = false;
            ushort low = Read(_pc_Register);
            _pc_Register++;
            ushort high = Read(_pc_Register);
            _pc_Register++;

            _operand_Address = (ushort)((ushort)(high << 8) | low);
            return 0;
        }

        /// <summary>
        /// Absolute, x-indexed.
        /// </summary>
        /// <returns></returns>
        public byte AM_ABX()
        {
            _isAMImplied = false;
            ushort low = Read(_pc_Register);
            _pc_Register++;
            ushort high = Read(_pc_Register);
            _pc_Register++;

            _operand_Address = (ushort)((ushort)(high << 8) | low);
            _operand_Address += _x_Register;

            // Check if a page boundary is passed.
            // If a boundary is passed then an additional cycle is needed for instruction.
            if ((ushort)(_operand_Address & 0xff00) != (ushort)(high << 8))
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Absolute, y-indexed.
        /// </summary>
        /// <returns></returns>
        public byte AM_ABY()
        {
            _isAMImplied = false;
            ushort low = Read(_pc_Register);
            _pc_Register++;
            ushort high = Read(_pc_Register);
            _pc_Register++;

            _operand_Address = (ushort)((ushort)(high << 8) | low);
            _operand_Address += _y_Register;

            // Check if a page boundary is passed.
            // If a boundary is passed then an additional cycle is needed for instruction.
            if ((ushort)(_operand_Address & 0xff00) != (ushort)(high << 8))
            {
                return 1;
            }
            return 0;
        }

        public byte AM_REL()
        {
            _isAMImplied = false;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indirect. 
        /// Operand is address; effective address is contents of word at address.
        /// </summary>
        /// <returns></returns>
        public byte AM_IND()
        {
            _isAMImplied = false;
            ushort low = Read(_pc_Register);
            _pc_Register++;
            ushort high = Read(_pc_Register);
            _pc_Register++;

            ushort address = (ushort)((ushort)(high << 8) | low);

            if (low == 0x00ff)
            {
                // if we increment by 1 we will got to next page.
                _operand_Address = (ushort)(Read((ushort)(address & 0xff00)) << 8 | Read(address));
            }
            else
            {
                _operand_Address = (ushort)(Read((ushort)(address + 1)) << 8 | Read(address));
            }

            return 0;
        }

        /// <summary>
        /// Indirect, x-indexed (without carry)
        /// </summary>
        /// <returns></returns>
        public byte AM_IZX()
        {
            _isAMImplied = false;
            byte low = Read(_pc_Register);
            _pc_Register++;

            byte indLow = Read((ushort)((low & 0x00ff) + _x_Register));
            byte indHigh = Read((ushort)((low & 0x00ff) + _x_Register + 1));

            _operand_Address = (ushort)((indHigh << 8) | (indLow & 0x00ff));
            return 0;

        }

        /// <summary>
        /// Indirect, y-indexed. (with carry)
        /// </summary>
        /// <returns></returns>
        public byte AM_IZY()
        {
            _isAMImplied = false;
            byte low = Read(_pc_Register);
            _pc_Register++;

            byte indLow = Read((ushort)((low & 0x00ff)));
            byte indHigh = Read((ushort)((low & 0x00ff) + 1));

            _operand_Address = (ushort)((indHigh << 8) | (indLow & 0x00ff));
            _operand_Address += _y_Register;
            return 0;
        }

        /// <summary>
        /// Invalide addressing mode.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public byte AM_XXX() => throw new NotImplementedException("Invalid addressing mode.");
    }
}
