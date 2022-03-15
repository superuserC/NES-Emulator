using FluentAssertions;
using NES_Emulator.Core.Interfaces;
using NES_Emulator.Core.Processor;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Emulator.Tests.Processor
{
    public class EventsTests
    {
        private IDataTransfer _dataTransfer;

        [SetUp]
        public void SetUp()
        {
            _dataTransfer = Substitute.For<IDataTransfer>();
        }

        [Test]
        public void Reset_Test()
        {
            var processor = GetInstance();
            processor.DataTransfer.Read(0xfffc).Returns((byte)0xab);
            processor.DataTransfer.Read(0xfffd).Returns((byte)0xcd);

            var cycles = processor.Reset();

            processor._pc_Register.Should().Be(0xcdab);
            processor._acc_Register.Should().Be(0);
            processor._x_Register.Should().Be(0);
            processor._y_Register.Should().Be(0);
            processor._sp_Register.Should().Be(0);
            processor._status_Register.Should().Be(0);
            cycles.Should().Be(8);
        }

        [TestCase((byte)0b00010000, (byte)0b00100100, 7, (ushort)0xcdab)]
        public void IRQ_Test(byte initialStatusRegister, byte finalStatusRegister, byte expectedCycles, ushort expectedPCRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;
            processor._pc_Register = 0x1234;
            processor.DataTransfer.Read(0xfffe).Returns((byte)0xab);
            processor.DataTransfer.Read(0xffff).Returns((byte)0xcd);

            var cycles = processor.IRQ();

            processor._status_Register.Should().Be(finalStatusRegister);
            processor._pc_Register.Should().Be(expectedPCRegister);
            cycles.Should().Be(expectedCycles);
            processor.DataTransfer.Received(1).Write(0x01fe, 0x12);
            processor.DataTransfer.Received(1).Write(0x01fd, 0x34);
            processor.DataTransfer.Received(1).Write(0x01fc, finalStatusRegister);
        }

        [TestCase((byte)0b00010100, (byte)0b00010100, 0, (ushort)0x1234)]
        public void IRQ_Test_IRQNotSet(byte initialStatusRegister, byte finalStatusRegister, byte expectedCycles, ushort expectedPCRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;
            processor._pc_Register = 0x1234;

            var cycles = processor.IRQ();

            processor._status_Register.Should().Be(finalStatusRegister);
            processor._pc_Register.Should().Be(expectedPCRegister);
            cycles.Should().Be(expectedCycles);
            processor.DataTransfer.Received(0).Write(0x01fe, 0x12);
            processor.DataTransfer.Received(0).Write(0x01fd, 0x34);
            processor.DataTransfer.Received(0).Write(0x01fc, finalStatusRegister);
        }

        private _6502 GetInstance() => new _6502(_dataTransfer);
    }
}
