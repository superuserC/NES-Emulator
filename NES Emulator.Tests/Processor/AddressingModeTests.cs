using AutoFixture;
using FluentAssertions;
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

        [Test]
        public void AM_IMP_SetAccumulatorRegisterAsOperand()
        {
            var processor = GetInstance();
            processor._operand_Value = 0x32;

            var cycles = processor.AM_IMP();

            processor._operand_Value.Should().Be(0x00);
            cycles.Should().Be(0);
        }

        [Test]
        public void AM_IMM_SetNextByteAsOperand()
        {
            var processor = GetInstance();
            var nextByte = _fixture.Create<byte>();
            _dataTransfer.Read(Arg.Any<ushort>()).Returns(nextByte);

            var cycles = processor.AM_IMM();

            processor._operand_Value.Should().Be(nextByte);
            processor._operand_Address.Should().Be(0x0000);
            cycles.Should().Be(0);
        }

        [Test]
        public void AM_ZP0_SetNextByteAsZeroPageAddress()
        {
            var processor = GetInstance();
            byte nextByte = 0x25;
            _dataTransfer.Read(Arg.Any<ushort>()).Returns(nextByte);

            var cycles = processor.AM_ZP0();

            // processor._operand_Value.Should().Be(nextByte);
            processor._operand_Address.Should().Be(0x0025);
            cycles.Should().Be(0);
        }

        private _6502 GetInstance() => new _6502(_dataTransfer);
    }
}
