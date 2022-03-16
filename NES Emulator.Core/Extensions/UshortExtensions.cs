using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core.Extensions
{
    public static class UshortExtensions
    {
        public static ushort OR(this ushort value, ushort value2) => (ushort)(value | value2);

        public static ushort OR(this ushort value, byte value2) => (ushort)(value | (ushort)value2);
    }
}
