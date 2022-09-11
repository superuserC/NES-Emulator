using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core.Mappers
{
    /// <summary>
    /// Represent cartridge mapper.
    /// This class is abstract as the nes system expects many implementations of the mapper in the cartridge.
    /// </summary>
    public abstract class Mapper
    {
        public abstract ushort MapMemoryAddress(ushort address);
    }
}
