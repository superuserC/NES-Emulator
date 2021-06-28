using Moq;
using NES_Emulator.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Tests.Helper
{
    public static class MockDataTransferHelper
    {
        public static Mock<IDataTransfer> GetMock()
        {
            Mock<IDataTransfer> result = new Mock<IDataTransfer>();
            result.Setup(mock => mock.Read(It.IsAny<ushort>())).Returns((ushort address) => 0x00);
            return result;
        }
    }
}
