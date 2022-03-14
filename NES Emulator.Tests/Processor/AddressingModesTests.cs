using AutoFixture;
using FluentAssertions;
using NES_Emulator.Core.Interfaces;
using NES_Emulator.Core.Processor;
using NSubstitute;
using NUnit.Framework;
using System;

namespace NES_Emulator.Tests.Processor
{
    public class AddressingModesTests
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
            processor.DataTransfer.Received(1).Read(processor._operand_Address);
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
            processor.DataTransfer.Received(1).Read(processor._operand_Address);
            cycles.Should().Be(0);
        }

        [Test]
        public void AM_ZPX_Test()
        {
            var processor = GetInstance();
            byte nextByte = 0x25;
            _dataTransfer.Read(Arg.Any<ushort>()).Returns(nextByte);
            processor.INX();

            var cycles = processor.AM_ZPX();

            processor._operand_Address.Should().Be(0x0026);
            processor.DataTransfer.Received(1).Read(processor._operand_Address);
            cycles.Should().Be(0);
        }

        [Test]
        public void AM_ZPY_Test()
        {
            var processor = GetInstance();
            byte nextByte = 0x25;
            _dataTransfer.Read(Arg.Any<ushort>()).Returns(nextByte);
            processor.INY();

            var cycles = processor.AM_ZPY();

            processor._operand_Address.Should().Be(0x0026);
            processor.DataTransfer.Received(1).Read(processor._operand_Address);
            cycles.Should().Be(0);
        }

        [TestCase((byte)0x23, (byte)0xac, (ushort)0xac23)]
        [TestCase((byte)0x23, (byte)0x00, (ushort)0x0023)]
        [TestCase((byte)0x00, (byte)0xac, (ushort)0xac00)]
        [TestCase((byte)0x00, (byte)0x00, (ushort)0x0000)]
        public void AM_ABS_Test(byte low, byte high, ushort address)
        {
            var processor = GetInstance();
            _dataTransfer.Read(0x0000).Returns(low);
            _dataTransfer.Read(0x0001).Returns(high);

            processor.AM_ABS();

            processor._operand_Address.Should().Be(address);
        }

        [TestCase((byte)0x23, (byte)0xac, (byte)0x01, (ushort)0xac24, false)]
        [TestCase((byte)0x23, (byte)0xac, (byte)0xf3, (ushort)0xad16, true)]
        [TestCase((byte)0x00, (byte)0x00, (byte)0xf3, (ushort)0x00f3, false)]
        [TestCase((byte)0x01, (byte)0x00, (byte)0xff, (ushort)0x0100, true)]
        public void AM_ABX_Test(byte low, byte high, byte xRegister ,ushort address, bool pagePassed)
        {
            var processor = GetInstance();
            _dataTransfer.Read(0x0000).Returns(low);
            _dataTransfer.Read(0x0001).Returns(high);
            processor._x_Register = xRegister;

            var cycles = processor.AM_ABX();

            processor._operand_Address.Should().Be(address);
            processor.DataTransfer.Received(1).Read(processor._operand_Address);
            cycles.Should().Be((byte)(pagePassed ? 1 : 0));
        }

        [TestCase((byte)0x23, (byte)0xac, (byte)0x01, (ushort)0xac24, false)]
        [TestCase((byte)0x23, (byte)0xac, (byte)0xf3, (ushort)0xad16, true)]
        [TestCase((byte)0x00, (byte)0x00, (byte)0xf3, (ushort)0x00f3, false)]
        [TestCase((byte)0x01, (byte)0x00, (byte)0xff, (ushort)0x0100, true)]
        public void AM_ABY_Test(byte low, byte high, byte yRegister, ushort address, bool pageCrossed)
        {
            var processor = GetInstance();
            _dataTransfer.Read(0x0000).Returns(low);
            _dataTransfer.Read(0x0001).Returns(high);
            processor._y_Register = yRegister;

            var cycles = processor.AM_ABY();

            processor._operand_Address.Should().Be(address);
            processor.DataTransfer.Received(1).Read(processor._operand_Address);
            cycles.Should().Be((byte)(pageCrossed ? 1 : 0));
        }

        [TestCase((byte)0x25, (byte)0x04, (ushort)0xac18)]
        public void AM_IZX_Test(byte low, byte xRegister, ushort address)
        {
            var processor = GetInstance();
            _dataTransfer.Read(0x0000).Returns(low);
            _dataTransfer.Read(0x0029).Returns((byte)0x18);
            _dataTransfer.Read(0x002a).Returns((byte)0xac);
            processor._x_Register = xRegister;

            byte cycles = processor.AM_IZX();

            processor._operand_Address.Should().Be(address);
            cycles.Should().Be(0);
        }

        [TestCase((byte)0x01, (byte)0x02, (byte)0x04, (ushort)0x0205, false)]
        [TestCase((byte)0x01, (byte)0x02, (byte)0xff, (ushort)0x0300, true)]
        [TestCase((byte)0x00, (byte)0x00, (byte)0xff, (ushort)0x00ff, false)]
        [TestCase((byte)0x00, (byte)0xff, (byte)0xff, (ushort)0xffff, false)]
        [TestCase((byte)0xff, (byte)0x00, (byte)0xff, (ushort)0x01fe, true)]
        [TestCase((byte)0x00, (byte)0x00, (byte)0x00, (ushort)0x0000, false)]
        public void AM_IZY_Test(byte low, byte high, byte yRegister, ushort address, bool pageCrossed)
        {
            var processor = GetInstance();
             byte tmpData = 0x25;
            _dataTransfer.Read(0x0000).Returns(tmpData);
            _dataTransfer.Read((ushort)(tmpData & 0x00ff)).Returns(low);
            _dataTransfer.Read((ushort)((tmpData & 0x00ff) + 1)).Returns(high);
            processor._y_Register = yRegister;

            byte cycles = processor.AM_IZY();

            processor._operand_Address.Should().Be(address);
            cycles.Should().Be((byte)(pageCrossed ? 1 : 0));
        }

        [TestCase((byte)0x05)]
        [TestCase((byte)0xff)]
        [TestCase((byte)0xfb)]
        public void AM_REL_Test(byte op)
        {
            var processor = GetInstance();
            _dataTransfer.Read(0x0000).Returns(op);

            byte cycles = processor.AM_REL();

            processor._offset.Should().Be((sbyte)op);
            processor.DataTransfer.Received(1).Read(processor._operand_Address);
            cycles.Should().Be(0);
        }

        [Test]
        public void AM_XXX_Test()
        {
            var processor = GetInstance();
            Action action = () => processor.AM_XXX();
            action.Should().Throw<NotImplementedException>();
        }

        private _6502 GetInstance() => new _6502(_dataTransfer);
    }
}
