using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core.Interfaces
{
    /// <summary>
    /// Provide read/write capability on the bus.
    /// </summary>
    public interface IDataTransfer
    {
        /// <summary>
        /// Read data from bus.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        byte Read(ushort address);

        /// <summary>
        /// Write data to bus.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        void Write(ushort address, byte data);
    }
}
