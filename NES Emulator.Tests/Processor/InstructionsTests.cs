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
    public class InstructionsTests
    {
        private IDataTransfer _dataTransfer;

        [SetUp]
        public void Setup()
        {
            _dataTransfer = Substitute.For<IDataTransfer>();
        }

        [TestCase((byte)0xff, (byte)0xbf)]
        [TestCase((byte)0x00, (byte)0x00)]
        [TestCase((byte)0x41, (byte)0x01)]
        public void CLV_Test(byte initialSRegister, byte finalSRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialSRegister;

            processor.CLV();

            processor._status_Register.Should().Be(finalSRegister);
        }

        [TestCase((byte)0x41, (ushort)0xff01, (byte)0x40)]
        [TestCase((byte)0x01, (ushort)0xffff, (byte)0x00)]
        [TestCase((byte)0x00, (ushort)0x01a5, (byte)0xff)]
        public void DEC_Test(byte opValue, ushort opAddress, byte newValue)
        {
            var processor = GetInstance();
            processor._operand_Value = opValue;
            processor._operand_Address = opAddress;

            processor.DEC();

            processor.DataTransfer.Received(1).Write(opAddress, newValue);
        }

        [TestCase((byte)0x41, (ushort)0xff01, (byte)0x42)]
        [TestCase((byte)0x00, (ushort)0xffff, (byte)0x01)]
        [TestCase((byte)0xff, (ushort)0x0000, (byte)0x00)]
        public void INC_Test(byte opValue, ushort opAddress, byte newValue)
        {
            var processor = GetInstance();
            processor._operand_Value = opValue;
            processor._operand_Address = opAddress;

            processor.INC();

            processor.DataTransfer.Received(1).Write(opAddress, newValue);
        }

        private _6502 GetInstance() => new _6502(_dataTransfer);
    }
}
