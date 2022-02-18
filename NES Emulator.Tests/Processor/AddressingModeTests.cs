
using AutoFixture;
using NES_Emulator.Core.Interfaces;
using NES_Emulator.Core.Processor;
using NSubstitute;
using NUnit.Framework;

namespace NES_Emulator.Tests.Processor
{
    public class AddressingModeTests
    {
        private Fixture _fixture;
        private IDataTransfer _dataTransfer;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _dataTransfer = Substitute.For<IDataTransfer>();
        }

        private _6502 GetInstance() => new _6502(_dataTransfer);
    }
}
