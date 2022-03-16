using AutoFixture;
using FluentAssertions;
using NES_Emulator.Core.Interfaces;
using NES_Emulator.Core.Processor;
using NSubstitute;
using NUnit.Framework;

namespace NES_Emulator.Tests.Processor
{
    public class _6502Tests
    {
        private IDataTransfer _dataTransfer;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _dataTransfer = Substitute.For<IDataTransfer>();
            _fixture = new Fixture();
        }

        [TestCase((byte)0xff, (ushort)0x01fe, (byte)0xfe)]
        [TestCase((byte)0x00, (ushort)0x01ff, (byte)0xff)]
        public void Test_PushToStack(byte spRegister, ushort expectedAddress, byte expectedSpRegister)
        {
            var processor = GetInstance();
            processor._sp_Register = spRegister;
            var data = _fixture.Create<byte>();

            processor.PushToStack(data);

            processor.DataTransfer.Received(1).Write(expectedAddress, data);
            processor._sp_Register.Should().Be(expectedSpRegister);
        }

        [TestCase((byte)0x00, (ushort)0x0100, (byte)0x01)]
        [TestCase((byte)0xff, (ushort)0x01ff, (byte)0x00)]
        public void Test_PopFromStack(byte spRegister, ushort expectedAddress, byte expectedSpRegister)
        {
            var processor = GetInstance();
            processor._sp_Register = spRegister;

            processor.PopFromStack();

            processor.DataTransfer.Received(1).Read(expectedAddress);
            processor._sp_Register.Should().Be(expectedSpRegister);
        }

        [TestCase(Flags6502.Carry, (byte)0b10101010, 0)]
        [TestCase(Flags6502.Carry, (byte)0b10101011, 1)]
        [TestCase(Flags6502.Zero, (byte)0b10101010, 1)]
        [TestCase(Flags6502.Zero, (byte)0b10101000, 0)]
        [TestCase(Flags6502.IRQDisable, (byte)0b10101010, 0)]
        [TestCase(Flags6502.IRQDisable, (byte)0b10101110, 1)]
        [TestCase(Flags6502.DecimalMode, (byte)0b10101010, 1)]
        [TestCase(Flags6502.DecimalMode, (byte)0b10100010, 0)]
        [TestCase(Flags6502.Break, (byte)0b10101010, 0)]
        [TestCase(Flags6502.Break, (byte)0b10111011, 1)]
        [TestCase(Flags6502.Unused, (byte)0b10101010, 1)]
        [TestCase(Flags6502.Unused, (byte)0b10001000, 0)]
        [TestCase(Flags6502.Overflow, (byte)0b10101010, 0)]
        [TestCase(Flags6502.Overflow, (byte)0b11101110, 1)]
        [TestCase(Flags6502.Negative, (byte)0b10101010, 1)]
        [TestCase(Flags6502.Negative, (byte)0b00100010, 0)]
        public void Test_ReadStatusRegister(Flags6502 flag, byte statusRegister, int expectedValue)
        {
            var processor = GetInstance();
            processor._status_Register = statusRegister;

            int value = processor.ReadStatusRegister(flag);

            value.Should().Be(expectedValue);

        }

        [Test]
        public void Test_Read()
        {
            var processor = GetInstance();
            var address = _fixture.Create<ushort>();
            var data = _fixture.Create<byte>();
            processor.DataTransfer.Read(address).Returns(data);

            var result = processor.Read(address);

            processor.DataTransfer.Received(1).Read(address);
            result.Should().Be(data);
        }

        [Test]
        public void Test_Write()
        {
            var processor = GetInstance();
            var address = _fixture.Create<ushort>();
            var data = _fixture.Create<byte>();

            processor.Write(address, data);

            processor.DataTransfer.Received(1).Write(address, data);
        }

        [TestCase((byte)0b00000000, Flags6502.Negative, (byte)0b10000000)]
        [TestCase((byte)0b00000000, Flags6502.Overflow, (byte)0b01000000)]
        [TestCase((byte)0b00000000, Flags6502.Unused, (byte)0b00100000)]
        [TestCase((byte)0b00000000, Flags6502.Break, (byte)0b00010000)]
        [TestCase((byte)0b00000000, Flags6502.DecimalMode, (byte)0b00001000)]
        [TestCase((byte)0b00000000, Flags6502.IRQDisable, (byte)0b00000100)]
        [TestCase((byte)0b00000000, Flags6502.Zero, (byte)0b00000010)]
        [TestCase((byte)0b00000000, Flags6502.Carry, (byte)0b00000001)]
        public void Test_SetFlag(byte initialStatusRegister, Flags6502 flag, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;

            processor.SetFlag(flag);

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0b00000000, Flags6502.Negative, true, (byte)0b10000000)]
        [TestCase((byte)0b00000000, Flags6502.Overflow, true, (byte)0b01000000)]
        [TestCase((byte)0b00000000, Flags6502.Unused, true, (byte)0b00100000)]
        [TestCase((byte)0b00000000, Flags6502.Break, true, (byte)0b00010000)]
        [TestCase((byte)0b00000000, Flags6502.DecimalMode, true, (byte)0b00001000)]
        [TestCase((byte)0b00000000, Flags6502.IRQDisable, true, (byte)0b00000100)]
        [TestCase((byte)0b00000000, Flags6502.Zero, true, (byte)0b00000010)]
        [TestCase((byte)0b00000000, Flags6502.Carry, true, (byte)0b00000001)]
        [TestCase((byte)0b11111111, Flags6502.Negative, false, (byte)0b01111111)]
        [TestCase((byte)0b11111111, Flags6502.Overflow, false, (byte)0b10111111)]
        [TestCase((byte)0b11111111, Flags6502.Unused, false, (byte)0b11011111)]
        [TestCase((byte)0b11111111, Flags6502.Break, false, (byte)0b11101111)]
        [TestCase((byte)0b11111111, Flags6502.DecimalMode, false, (byte)0b11110111)]
        [TestCase((byte)0b11111111, Flags6502.IRQDisable, false, (byte)0b11111011)]
        [TestCase((byte)0b11111111, Flags6502.Zero, false, (byte)0b11111101)]
        [TestCase((byte)0b11111111, Flags6502.Carry, false, (byte)0b11111110)]
        public void Test_SetFlag_B(byte initialStatusRegister, Flags6502 flag, bool activate, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;

            processor.SetFlag(flag, activate);

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0b11111111, Flags6502.Negative, (byte)0b01111111)]
        [TestCase((byte)0b11111111, Flags6502.Overflow, (byte)0b10111111)]
        [TestCase((byte)0b11111111, Flags6502.Unused, (byte)0b11011111)]
        [TestCase((byte)0b11111111, Flags6502.Break, (byte)0b11101111)]
        [TestCase((byte)0b11111111, Flags6502.DecimalMode, (byte)0b11110111)]
        [TestCase((byte)0b11111111, Flags6502.IRQDisable, (byte)0b11111011)]
        [TestCase((byte)0b11111111, Flags6502.Zero, (byte)0b11111101)]
        [TestCase((byte)0b11111111, Flags6502.Carry, (byte)0b11111110)]
        public void ClearFlag(byte initialStatusRegister, Flags6502 flag, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;

            processor.ClearFlag(flag);

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        public _6502 GetInstance() => new _6502(_dataTransfer);
    }
}
