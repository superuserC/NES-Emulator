using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Core.Exceptions
{
    public class CartridgeCreationException : Exception
    {
        public CartridgeCreationException()
        {
        }

        public CartridgeCreationException(string message) : base(message)
        {
        }

        public CartridgeCreationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CartridgeCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
