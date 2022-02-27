using FluentAssertions;
using NES_Emulator.Core.Extensions;
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

        [TestCase(0x01, 0x00, 0x03, true)]
        [TestCase(0x02, 0x01, 0x00, true)]
        [TestCase(0x00, 0x00, 0x02, true)]
        [TestCase(0xff, 0x7f, 0x01, true)]
        [TestCase(0x01, 0x00, 0x03, false)]
        [TestCase(0x02, 0x01, 0x00, false)]
        [TestCase(0x00, 0x00, 0x02, false)]
        [TestCase(0xff, 0x7f, 0x01, false)]
        public void LSR_Test(byte dataIn, byte dataOut, byte expectedStatusRegister, bool isAMImplied)
        {
            var processor = GetInstance();
            processor._status_Register = 0x00;
            processor._operand_Value = dataIn;
            processor._acc_Register = 0x00;
            processor._isAMImplied = isAMImplied;

            processor.LSR();

            processor._acc_Register.Should().Be(isAMImplied ? dataOut : (byte)0x00);
            processor.DataTransfer.Received(isAMImplied ? 0 : 1).Write(processor._operand_Address, dataOut);
            processor._status_Register.Should().Be(expectedStatusRegister);
        }

        [TestCase((byte)0xff, (byte)0b00000000, (byte)0b00110000)]
        [TestCase((byte)0x00, (byte)0b00110010, (byte)0b00110010)]
        [TestCase((byte)0xf1, (byte)0b00100001, (byte)0b00110001)]
        [TestCase((byte)0x01, (byte)0b10100001, (byte)0b10110001)]
        public void PHP_TEST(byte initialSPRegister, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._sp_Register = initialSPRegister;
            processor._status_Register = initialStatusRegister;

            processor.PHP();

            processor.DataTransfer.Received(1).Write(processor._sp_Register.OR(0x0100), finalStatusRegister);
        }

        [TestCase((byte)0b11110011, (byte)0b11110010, (byte)0b10101010, (byte)0b11010101)]
        [TestCase((byte)0b11110010, (byte)0b11110010, (byte)0b10101010, (byte)0b01010101)]
        [TestCase((byte)0b11110001, (byte)0b11110001, (byte)0b10101011, (byte)0b11010101)]
        public void ROR_Imp_Test(byte initialStatusRegister, byte finalStatusRegister, byte data, byte finalData)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;
            processor._operand_Value = data;
            processor._isAMImplied = true;

            processor.ROR();

            processor._acc_Register.Should().Be(finalData);
            processor._status_Register.Should().Be(finalStatusRegister);
            processor.DataTransfer.Received(0).Write(processor._operand_Address, finalData);
        }

        [TestCase((byte)0b11110011, (byte)0b11110010, (byte)0b10101010, (byte)0b11010101)]
        [TestCase((byte)0b11110010, (byte)0b11110010, (byte)0b10101010, (byte)0b01010101)]
        [TestCase((byte)0b11110001, (byte)0b11110001, (byte)0b10101011, (byte)0b11010101)]
        public void ROR_NotImp_Test(byte initialStatusRegister, byte finalStatusRegister, byte data, byte finalData)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;
            processor._operand_Value = data;
            processor._isAMImplied = false;
            processor._acc_Register = 0x00;

            processor.ROR();

            processor._acc_Register.Should().Be((byte)0);
            processor._status_Register.Should().Be(finalStatusRegister);
            processor.DataTransfer.Received(1).Write(processor._operand_Address, finalData);
        }

        [TestCase((byte)0b11110010, (byte)0b11110011)]
        [TestCase((byte)0x00, (byte)0x01)]
        [TestCase((byte)0xff, (byte)0xff)]
        [TestCase((byte)0xfe, (byte)0xff)]
        public void SEC_Test(byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;

            processor.SEC();

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [Test]
        public void STX_Test()
        {
            var processor = GetInstance();
            processor._operand_Address = 0x123f;
            processor._x_Register = 0x12;

            processor.STX();

            processor.DataTransfer.Received(1).Write(processor._operand_Address, processor._x_Register);
        }

        [TestCase((byte)0xff, (byte)0b00000000, (byte)0b10000000)]
        [TestCase((byte)0xff, (byte)0b00000010, (byte)0b10000000)]
        [TestCase((byte)0xff, (byte)0b00100010, (byte)0b10100000)]
        [TestCase((byte)0x00, (byte)0b00000000, (byte)0b00000010)]
        [TestCase((byte)0x00, (byte)0b11111101, (byte)0b01111111)]
        [TestCase((byte)0x00, (byte)0b11111111, (byte)0b01111111)]
        public void TSX_Test(byte spRegister, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._sp_Register = spRegister;
            processor._status_Register = initialStatusRegister;

            processor.TSX();

            processor._x_Register.Should().Be(spRegister);
            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0b10101010, (byte)0b01010101, (byte)0, (byte)0b00000000, (byte)0b00000010)]
        [TestCase((byte)0b10101010, (byte)0b11010101, (byte)0b10000000, (byte)0b00000000, (byte)0b10000000)]
        public void AND_Test(byte accRegister, byte memory, byte expectedValue, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._acc_Register = accRegister;
            processor._operand_Value = memory;
            processor._status_Register = initialStatusRegister;

            processor.AND();

            processor._acc_Register.Should().Be(expectedValue);
            processor._status_Register.Should().Be(finalStatusRegister);
        }
        private _6502 GetInstance() => new _6502(_dataTransfer);
    }
}
