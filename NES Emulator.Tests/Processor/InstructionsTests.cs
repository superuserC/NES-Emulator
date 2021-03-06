using AutoFixture;
using FluentAssertions;
using NES_Emulator.Core.Extensions;
using NES_Emulator.Core.Interfaces;
using NES_Emulator.Core.Processor;
using NSubstitute;
using NUnit.Framework;
using System;

namespace NES_Emulator.Tests.Processor
{
    public class InstructionsTests
    {
        private IDataTransfer _dataTransfer;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _dataTransfer = Substitute.For<IDataTransfer>();
            _fixture = new Fixture();
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

            processor.DataTransfer.Received(1).Write(((ushort)(0x0100)).OR(processor._sp_Register), finalStatusRegister);
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

        [TestCase((byte)0b11111111, (byte)0b11111110)]
        [TestCase((byte)0b11111110, (byte)0b11111110)]
        [TestCase((byte)0b00000001, (byte)0b00000000)]
        public void CLC_Test(byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;

            processor.CLC();

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0xff, (byte)0x05, (byte)0b01111111, (byte)0b11111101)]
        [TestCase((byte)0xff, (byte)0xff, (byte)0b11111100, (byte)0b01111111)]
        [TestCase((byte)0x01, (byte)0xff, (byte)0b11111111, (byte)0b01111100)]
        public void CMP_Test(byte accRegister, byte data, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._acc_Register = accRegister;
            processor._operand_Value = data;
            processor._status_Register = initialStatusRegister;

            processor.CMP();

            processor._status_Register.Should().Be(finalStatusRegister);

        }

        [TestCase((byte)10, (byte)9, (byte)0b11111111, (byte)0b01111101)]
        [TestCase((byte)0, (byte)0xff, (byte)0b01111111, (byte)0b11111101)]
        [TestCase((byte)1, (byte)0, (byte)0b11111101, (byte)0b01111111)]
        public void DEX_Test(byte initialXRegister, byte finalXRegister, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._x_Register = initialXRegister;
            processor._status_Register = initialStatusRegister;

            processor.DEX();

            processor._x_Register.Should().Be(finalXRegister);
            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)10, (byte)11, (byte)0b11111111, (byte)0b01111101)]
        [TestCase((byte)0xfe, (byte)0xff, (byte)0b01111111, (byte)0b11111101)]
        [TestCase((byte)0xff, (byte)0, (byte)0b11111101, (byte)0b01111111)]
        public void INX_Test(byte initialXRegister, byte finalXRegister, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._x_Register = initialXRegister;
            processor._status_Register = initialStatusRegister;

            processor.INX();

            processor._x_Register.Should().Be(finalXRegister);
            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0, (byte)0xff, (byte)0b01111111, (byte)0b11111101)]
        [TestCase((byte)10, (byte)0, (byte)0b01111101, (byte)0b01111111)]
        public void LDA_Test(byte initialAccRegister, byte data, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._acc_Register = initialAccRegister;
            processor._operand_Value = data;
            processor._status_Register = initialStatusRegister;

            processor.LDA();

            processor._acc_Register.Should().Be(data);
            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [Test]
        public void NOP_Test()
        {
            var processor = GetInstance();

            var cycles = processor.NOP();

            cycles.Should().Be(0);
        }

        [TestCase((byte)0xff, (byte)0b00001111, (byte)0b10001101)]
        [TestCase((byte)0x00, (byte)0b10001101, (byte)0b00001111)]
        public void PLA_Test(byte data, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;
            processor.DataTransfer.Read(Arg.Any<ushort>()).Returns(data);

            processor.PLA();

            processor._acc_Register.Should().Be(data);
            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0b11111111, (byte)0xf5, (byte)0b11001111)]
        public void RTI_Test(byte stackFirst, byte stackSecond, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._sp_Register = 0xf0;
            processor.DataTransfer.Read(0x01f0).Returns(stackFirst);
            processor.DataTransfer.Read(0x01f1).Returns(stackSecond);
            processor._status_Register = 0b00000000;

            processor.RTI();

            processor._status_Register.Should().Be(finalStatusRegister);
            processor._pc_Register.Should().Be(stackSecond);

        }

        [Test]
        public void SED_Test()
        {
            var processor = GetInstance();
            processor._status_Register = 0b00000000;

            processor.SED();

            processor._status_Register.Should().Be((byte)0b00001000);
        }

        [Test]
        public void STY_Test()
        {
            var processor = GetInstance();
            processor._operand_Address = _fixture.Create<ushort>();
            processor._y_Register = _fixture.Create<byte>();

            processor.STY();

            processor.DataTransfer.Received(1).Write(processor._operand_Address, processor._y_Register);
        }

        [TestCase((byte)0b11111111, (byte)0b00000000, (byte)0b10000000)]
        [TestCase((byte)0b00000000, (byte)0b10000000, (byte)0b00000010)]
        public void TXA_Test(byte xRegister, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._x_Register = xRegister;
            processor._status_Register = initialStatusRegister;

            processor.TXA();

            processor._status_Register.Should().Be(finalStatusRegister);
            processor._acc_Register.Should().Be(xRegister);
        }

        [TestCase((byte)0b11111111, (byte)0b00000000, (byte)0b10000001)]
        [TestCase((byte)0b01111111, (byte)0b00000000, (byte)0b10000000)]
        [TestCase((byte)0b00111111, (byte)0b00000000, (byte)0b00000000)]
        [TestCase((byte)0b00000000, (byte)0b00000000, (byte)0b00000010)]
        [TestCase((byte)0b10000000, (byte)0b00000000, (byte)0b00000011)]
        public void ASL_Test(byte opValue, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._operand_Value = opValue;
            processor._status_Register = initialStatusRegister;

            processor.ASL();

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0b11111111, (byte)0b00000000, (byte)0b11111101, (byte)0b00111111)]
        [TestCase((byte)0b11111111, (byte)0b11111111, (byte)0b00111111, (byte)0b11111101)]
        public void BIT_Test(byte accRegister, byte opValue, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._acc_Register = accRegister;
            processor._operand_Value = opValue;
            processor._status_Register = initialStatusRegister;

            processor.BIT();

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0b11111111, (byte)0b11110111)]
        [TestCase((byte)0b00000000, (byte)0b00000000)]
        public void CLD_Test(byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;

            processor.CLD();

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)50, (byte)25, (byte)0b10000010, (byte)0b00000001)]
        [TestCase((byte)255, (byte)5, (byte)0b00000010, (byte)0b10000001)]
        [TestCase((byte)255, (byte)255, (byte)0b10000000, (byte)0b00000011)]
        [TestCase((byte)0, (byte)25, (byte)0b00000011, (byte)0b10000000)]
        [TestCase((byte)0, (byte)250, (byte)0b10000011, (byte)0b00000000)]
        public void CPX_Test(byte xRegister, byte opValue, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._x_Register = xRegister;
            processor._operand_Value = opValue;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.CPX();

            processor._status_Register.Should().Be(finalStatusRegister);
            cycles.Should().Be(0);
        }

        [TestCase((byte)0, (byte)255, (byte)0b00000010, (byte)0b10000000)]
        [TestCase((byte)1, (byte)0, (byte)0b10000000, (byte)0b00000010)]
        [TestCase((byte)10, (byte)9, (byte)0b10000010, (byte)0b00000000)]
        public void DEY_Test(byte yRegister, byte expectedYRegister, byte initialStatusRegister, byte finalSatusRegister)
        {
            var processor = GetInstance();
            processor._y_Register = yRegister;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.DEY();

            processor._status_Register.Should().Be(finalSatusRegister);
            processor._y_Register.Should().Be(expectedYRegister);
            cycles.Should().Be(0);
        }

        [TestCase((byte)0, (byte)1, (byte)0b10000010, (byte)0b00000000)]
        [TestCase((byte)255, (byte)0, (byte)0b10000000, (byte)0b00000010)]
        [TestCase((byte)250, (byte)251, (byte)0b00000010, (byte)0b10000000)]
        public void INY_Test(byte yRegister, byte expectedYRegister, byte initialStatusRegister, byte finalSatusRegister)
        {
            var processor = GetInstance();
            processor._y_Register = yRegister;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.INY();

            processor._status_Register.Should().Be(finalSatusRegister);
            processor._y_Register.Should().Be(expectedYRegister);
            cycles.Should().Be(0);
        }

        [TestCase((byte)0b10000000, (byte)0b00000000, (byte)0b10000000)]
        [TestCase((byte)0b00000000, (byte)0b11111111, (byte)0b01111111)]
        [TestCase((byte)0b00011000, (byte)0b11111111, (byte)0b01111101)]
        public void LDX_Test(byte opValue, byte initialStatusRegister, byte finalSatusRegister)
        {
            var processor = GetInstance();
            processor._x_Register = _fixture.Create<byte>();
            processor._operand_Value = opValue;
            processor._status_Register = initialStatusRegister;

            processor.LDX();

            processor._x_Register.Should().Be(opValue);
            processor._status_Register.Should().Be(finalSatusRegister);
        }

        [TestCase((byte)0b11111111, (byte)0b11111111, (byte)0b11111111, (byte)0b00000000, (byte)0b10000000)]
        [TestCase((byte)0b11111111, (byte)0b00000000, (byte)0b11111111, (byte)0b00000000, (byte)0b10000000)]
        [TestCase((byte)0b00000000, (byte)0b00000000, (byte)0b00000000, (byte)0b10000000, (byte)0b00000010)]
        public void ORA_Test(byte accRegister, byte opValue, byte expectedAccRegister, byte initialStatusRegister, byte finalSatusRegister)
        {
            var processor = GetInstance();
            processor._acc_Register = accRegister;
            processor._operand_Value = opValue;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.ORA();

            processor._acc_Register.Should().Be(expectedAccRegister);
            processor._status_Register.Should().Be(finalSatusRegister);
            cycles.Should().Be(0);
        }

        [TestCase((byte)0b11111111, (byte)0b00000000, (byte)0b11001111)]
        [TestCase((byte)0b00000000, (byte)0b11111111, (byte)0b00110000)]
        public void PLP_Test(byte stack, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor.DataTransfer.Read(Arg.Any<ushort>()).Returns(stack);
            processor._status_Register = initialStatusRegister;

            processor.PLP();

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0, (byte)1)]
        [TestCase((byte)255, (byte)0)]
        public void RTS_Test(byte stack, byte expectedPcRegister)
        {
            var processor = GetInstance();
            processor._pc_Register = _fixture.Create<byte>();
            processor.DataTransfer.Read(Arg.Any<ushort>()).Returns(stack);

            processor.RTS();

            processor._pc_Register.Should().Be(expectedPcRegister);

        }

        [TestCase((byte)0b00000000, (byte)0b00000100)]
        [TestCase((byte)0b00000100, (byte)0b00000100)]
        public void SEI_Test(byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;

            processor.SEI();

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0b10000000, (byte)0b00000010, (byte)0b10000000)]
        [TestCase((byte)0b00000000, (byte)0b10000000, (byte)0b00000010)]
        public void TAX_Test(byte accRegister, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._x_Register = _fixture.Create<byte>();
            processor._acc_Register = accRegister;
            processor._status_Register = initialStatusRegister;

            processor.TAX();

            processor._status_Register.Should().Be(finalStatusRegister);
            processor._x_Register.Should().Be(accRegister);
        }

        [TestCase((byte)0b10000000, (byte)0b00000010, (byte)0b10000000)]
        [TestCase((byte)0b00000000, (byte)0b10000000, (byte)0b00000010)]
        public void TXS_Test(byte xRegister, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._sp_Register = _fixture.Create<byte>();
            processor._x_Register = xRegister;
            processor._status_Register = initialStatusRegister;

            processor.TXS();

            processor._status_Register.Should().Be(finalStatusRegister);
            processor._sp_Register.Should().Be(xRegister);
        }

        [TestCase((byte)0b11111111, (byte)0b11111011)]
        [TestCase((byte)0b11111011, (byte)0b11111011)]
        public void CLI_Test(byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;

            processor.CLI();

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)10, (byte)9, (byte)0b00000010, (byte)0b00000001)]
        [TestCase((byte)255, (byte)1, (byte)0b00000010, (byte)0b10000001)]
        [TestCase((byte)10, (byte)10, (byte)0b00000000, (byte)0b00000011)]
        [TestCase((byte)10, (byte)11, (byte)0b00000011, (byte)0b10000000)]
        [TestCase((byte)1, (byte)255, (byte)0b10000011, (byte)0b00000000)]
        public void CPY_Test(byte yRegister, byte opValue, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._y_Register = yRegister;
            processor._operand_Value = opValue;
            processor._status_Register = initialStatusRegister;

            processor.CPY();

            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0b11111111, (byte)0b11111111, (byte)0b00000000, (byte)0b10000000, (byte)0b00000010)]
        [TestCase((byte)0b11111111, (byte)0b01111111, (byte)0b10000000, (byte)0b00000010, (byte)0b10000000)]
        [TestCase((byte)0b00000000, (byte)0b00000000, (byte)0b00000000, (byte)0b10000000, (byte)0b00000010)]
        public void EOR_Test(byte accRegister, byte opValue, byte expectedAccRegister, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;
            processor._acc_Register = accRegister;
            processor._operand_Value = opValue;

            processor.EOR();

            processor._status_Register.Should().Be(finalStatusRegister);
            processor._acc_Register.Should().Be(expectedAccRegister);
        }

        [TestCase((byte)0, (byte)0b10000000, (byte)0b00000010)]
        [TestCase((byte)255, (byte)0b00000010, (byte)0b10000000)]
        public void LDY_Test(byte opValue, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._y_Register = _fixture.Create<byte>();
            processor._operand_Value = opValue;
            processor._status_Register = initialStatusRegister;

            processor.LDY();

            processor._y_Register.Should().Be(opValue);
            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [Test]
        public void PHA_Test()
        {
            var processor = GetInstance();
            var accRegister = _fixture.Create<byte>();
            byte stackRegister = 0x04;
            processor._acc_Register = accRegister;
            processor._sp_Register = stackRegister;

            processor.PHA();

            processor.DataTransfer.Received(1).Write(0x0103, accRegister);
        }

        [TestCase((byte)0b11110000, (byte)0b00000000, (byte)0b00000001, (byte)0b11100000)]
        [TestCase((byte)0b00000000, (byte)0b00000001, (byte)0b00000000, (byte)0b00000001)]
        [TestCase((byte)0b10000000, (byte)0b00000001, (byte)0b00000001, (byte)0b00000001)]
        [TestCase((byte)0b11111111, (byte)0b00000001, (byte)0b00000001, (byte)0b11111111)]
        public void ROL_Test_Implied(byte opValue, byte initialStatusRegister, byte finalStatusRegister, byte expectedAccRegister)
        {
            var processor = GetInstance();
            processor._status_Register = initialStatusRegister;
            processor._operand_Value = opValue;
            processor._acc_Register = _fixture.Create<byte>();
            processor._isAMImplied = true;

            processor.ROL();

            processor._status_Register.Should().Be(finalStatusRegister);
            processor._acc_Register.Should().Be(expectedAccRegister);
            processor.DataTransfer.Received(0).Write(Arg.Any<ushort>(), Arg.Any<byte>());
        }

        [TestCase((byte)0b11110000, (byte)0b00000000, (byte)0b00000001, (byte)0b11100000)]
        [TestCase((byte)0b00000000, (byte)0b00000001, (byte)0b00000000, (byte)0b00000001)]
        [TestCase((byte)0b10000000, (byte)0b00000001, (byte)0b00000001, (byte)0b00000001)]
        [TestCase((byte)0b11111111, (byte)0b00000001, (byte)0b00000001, (byte)0b11111111)]
        public void ROL_Test_NotImplied(byte opValue, byte initialStatusRegister, byte finalStatusRegister, byte expectedValue)
        {
            var processor = GetInstance();
            var initialAccRegister = _fixture.Create<byte>();
            var initialOpAddress = _fixture.Create<ushort>();
            processor._status_Register = initialStatusRegister;
            processor._acc_Register = initialAccRegister;
            processor._isAMImplied = false;
            processor._operand_Value = opValue;
            processor._operand_Address = initialOpAddress;

            processor.ROL();

            processor._status_Register.Should().Be(finalStatusRegister);
            processor._acc_Register.Should().Be(initialAccRegister);
            processor.DataTransfer.Received(1).Write(initialOpAddress, expectedValue);
        }

        [Test]
        public void STA_Test()
        {
            var processor = GetInstance();
            var opAddress = _fixture.Create<ushort>();
            var accRegister = _fixture.Create<byte>();
            processor._operand_Address = opAddress;
            processor._acc_Register = accRegister;

            processor.STA();

            processor.DataTransfer.Received(1).Write(opAddress, accRegister);
        }

        [TestCase((byte)0b10000000, (byte)0b00000010, (byte)0b10000000)]
        [TestCase((byte)0b00000000, (byte)0b10000000, (byte)0b00000010)]
        public void TAY_Test(byte accRegister, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._acc_Register = accRegister;
            processor._status_Register = initialStatusRegister;

            processor.TAY();

            processor._y_Register.Should().Be(accRegister);
            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0b10000000, (byte)0b00000010, (byte)0b10000000)]
        [TestCase((byte)0b00000000, (byte)0b10000000, (byte)0b00000010)]
        public void TYA_Test(byte yRegister, byte initialStatusRegister, byte finalStatusRegister)
        {
            var processor = GetInstance();
            processor._y_Register = yRegister;
            processor._status_Register = initialStatusRegister;

            processor.TYA();

            processor._acc_Register.Should().Be(yRegister);
            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0b00000000, (byte)0b00000001, (byte)0b00000001, (byte)0b00000000, (byte)0b00000010)]
        [TestCase((byte)0b00000000, (byte)0b10000001, (byte)0b00000001, (byte)0b10000000, (byte)0b10000010)]
        [TestCase((byte)0b01111111, (byte)0b01111111, (byte)0b00000001, (byte)0b11000000, (byte)0b11111111)]
        [TestCase((byte)0b10000000, (byte)0b10000001, (byte)0b00000001, (byte)0b01000001, (byte)0b00000010)]
        [TestCase((byte)0b00000001, (byte)0b11111111, (byte)0b00000000, (byte)0b00000011, (byte)0b00000000)]
        public void ADC_Test(byte accRegister, byte opValue, byte initialStatusRegister, byte finalStatusRegister, byte expectedAccRegister)
        {
            var processor = GetInstance();
            processor._acc_Register = accRegister;
            processor._operand_Value = opValue;
            processor._status_Register = initialStatusRegister;

            processor.ADC();

            processor._acc_Register.Should().Be(expectedAccRegister);
            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((byte)0b00000000, (byte)0b11111110, (byte)0b00000001, (byte)0b00000000, (byte)0b00000010)]
        [TestCase((byte)0b00000000, (byte)0b01111110, (byte)0b00000001, (byte)0b10000000, (byte)0b10000010)]
        [TestCase((byte)0b01111111, (byte)0b10000000, (byte)0b00000001, (byte)0b11000000, (byte)0b11111111)]
        [TestCase((byte)0b10000000, (byte)0b01111110, (byte)0b00000001, (byte)0b01000001, (byte)0b00000010)]
        [TestCase((byte)0b00000001, (byte)0b00000000, (byte)0b00000000, (byte)0b00000011, (byte)0b00000000)]
        public void SBC_Test(byte accRegister, byte opValue, byte initialStatusRegister, byte finalStatusRegister, byte expectedAccRegister)
        {
            var processor = GetInstance();
            processor._acc_Register = accRegister;
            processor._operand_Value = opValue;
            processor._status_Register = initialStatusRegister;

            processor.SBC();

            processor._acc_Register.Should().Be(expectedAccRegister);
            processor._status_Register.Should().Be(finalStatusRegister);
        }

        [TestCase((ushort)0x00f0, (sbyte)0x05, (byte)0b00000000, (ushort)0x00f5, (byte)1)]
        [TestCase((ushort)0x00ff, (sbyte)0x05, (byte)0b00000000, (ushort)0x0104, (byte)2)]
        [TestCase((ushort)0x00ff, (sbyte)-1, (byte)0b00000000, (ushort)0x00fe, (byte)1)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b00000000, (ushort)0x00ff, (byte)2)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b0000001, (ushort)0x0100, (byte)0)]
        public void BCC_Test(ushort pcRegister, sbyte offset, byte initialStatusRegister ,ushort expectedPcRegister, byte expectedCycles)
        {
            var processor = GetInstance();
            processor._pc_Register = pcRegister;
            processor._offset = offset;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.BCC();

            processor._pc_Register.Should().Be(expectedPcRegister);
            cycles.Should().Be(expectedCycles);
        }

        [TestCase((ushort)0x00f0, (sbyte)0x05, (byte)0b10000000, (ushort)0x00f5, (byte)1)]
        [TestCase((ushort)0x00ff, (sbyte)0x05, (byte)0b10000000, (ushort)0x0104, (byte)2)]
        [TestCase((ushort)0x00ff, (sbyte)-1, (byte)0b10000000, (ushort)0x00fe, (byte)1)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b10000000, (ushort)0x00ff, (byte)2)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b0000000, (ushort)0x0100, (byte)0)]
        public void BMI_Test(ushort pcRegister, sbyte offset, byte initialStatusRegister, ushort expectedPcRegister, byte expectedCycles)
        {
            var processor = GetInstance();
            processor._pc_Register = pcRegister;
            processor._offset = offset;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.BMI();

            processor._pc_Register.Should().Be(expectedPcRegister);
            cycles.Should().Be(expectedCycles);
        }

        [TestCase((ushort)0x00f0, (sbyte)0x05, (byte)0b00000000, (ushort)0x00f5, (byte)1)]
        [TestCase((ushort)0x00ff, (sbyte)0x05, (byte)0b00000000, (ushort)0x0104, (byte)2)]
        [TestCase((ushort)0x00ff, (sbyte)-1, (byte)0b00000000, (ushort)0x00fe, (byte)1)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b00000000, (ushort)0x00ff, (byte)2)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b01000000, (ushort)0x0100, (byte)0)]
        public void BVC_Test(ushort pcRegister, sbyte offset, byte initialStatusRegister, ushort expectedPcRegister, byte expectedCycles)
        {
            var processor = GetInstance();
            processor._pc_Register = pcRegister;
            processor._offset = offset;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.BVC();

            processor._pc_Register.Should().Be(expectedPcRegister);
            cycles.Should().Be(expectedCycles);
        }

        [TestCase((ushort)0x00f0, (sbyte)0x05, (byte)0b00000010, (ushort)0x00f5, (byte)1)]
        [TestCase((ushort)0x00ff, (sbyte)0x05, (byte)0b00000010, (ushort)0x0104, (byte)2)]
        [TestCase((ushort)0x00ff, (sbyte)-1, (byte)0b00000010, (ushort)0x00fe, (byte)1)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b00000010, (ushort)0x00ff, (byte)2)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b00000000, (ushort)0x0100, (byte)0)]
        public void BEQ_Test(ushort pcRegister, sbyte offset, byte initialStatusRegister, ushort expectedPcRegister, byte expectedCycles)
        {
            var processor = GetInstance();
            processor._pc_Register = pcRegister;
            processor._offset = offset;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.BEQ();

            processor._pc_Register.Should().Be(expectedPcRegister);
            cycles.Should().Be(expectedCycles);
        }

        [TestCase((ushort)0x00f0, (sbyte)0x05, (byte)0b00000000, (ushort)0x00f5, (byte)1)]
        [TestCase((ushort)0x00ff, (sbyte)0x05, (byte)0b00000000, (ushort)0x0104, (byte)2)]
        [TestCase((ushort)0x00ff, (sbyte)-1, (byte)0b00000000, (ushort)0x00fe, (byte)1)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b00000000, (ushort)0x00ff, (byte)2)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b10000000, (ushort)0x0100, (byte)0)]
        public void BPL_Test(ushort pcRegister, sbyte offset, byte initialStatusRegister, ushort expectedPcRegister, byte expectedCycles)
        {
            var processor = GetInstance();
            processor._pc_Register = pcRegister;
            processor._offset = offset;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.BPL();

            processor._pc_Register.Should().Be(expectedPcRegister);
            cycles.Should().Be(expectedCycles);
        }

        [TestCase((ushort)0x00f0, (sbyte)0x05, (byte)0b00000001, (ushort)0x00f5, (byte)1)]
        [TestCase((ushort)0x00ff, (sbyte)0x05, (byte)0b00000001, (ushort)0x0104, (byte)2)]
        [TestCase((ushort)0x00ff, (sbyte)-1, (byte)0b00000001, (ushort)0x00fe, (byte)1)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b00000001, (ushort)0x00ff, (byte)2)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b00000000, (ushort)0x0100, (byte)0)]
        public void BCS_Test(ushort pcRegister, sbyte offset, byte initialStatusRegister, ushort expectedPcRegister, byte expectedCycles)
        {
            var processor = GetInstance();
            processor._pc_Register = pcRegister;
            processor._offset = offset;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.BCS();

            processor._pc_Register.Should().Be(expectedPcRegister);
            cycles.Should().Be(expectedCycles);
        }

        [TestCase((ushort)0x00f0, (sbyte)0x05, (byte)0b00000000, (ushort)0x00f5, (byte)1)]
        [TestCase((ushort)0x00ff, (sbyte)0x05, (byte)0b00000000, (ushort)0x0104, (byte)2)]
        [TestCase((ushort)0x00ff, (sbyte)-1, (byte)0b00000000, (ushort)0x00fe, (byte)1)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b00000000, (ushort)0x00ff, (byte)2)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b00000010, (ushort)0x0100, (byte)0)]
        public void BNE_Test(ushort pcRegister, sbyte offset, byte initialStatusRegister, ushort expectedPcRegister, byte expectedCycles)
        {
            var processor = GetInstance();
            processor._pc_Register = pcRegister;
            processor._offset = offset;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.BNE();

            processor._pc_Register.Should().Be(expectedPcRegister);
            cycles.Should().Be(expectedCycles);
        }

        [TestCase((ushort)0x00f0, (sbyte)0x05, (byte)0b01000000, (ushort)0x00f5, (byte)1)]
        [TestCase((ushort)0x00ff, (sbyte)0x05, (byte)0b01000000, (ushort)0x0104, (byte)2)]
        [TestCase((ushort)0x00ff, (sbyte)-1, (byte)0b01000000, (ushort)0x00fe, (byte)1)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b01000000, (ushort)0x00ff, (byte)2)]
        [TestCase((ushort)0x0100, (sbyte)-1, (byte)0b00000000, (ushort)0x0100, (byte)0)]
        public void BVS_Test(ushort pcRegister, sbyte offset, byte initialStatusRegister, ushort expectedPcRegister, byte expectedCycles)
        {
            var processor = GetInstance();
            processor._pc_Register = pcRegister;
            processor._offset = offset;
            processor._status_Register = initialStatusRegister;

            var cycles = processor.BVS();

            processor._pc_Register.Should().Be(expectedPcRegister);
            cycles.Should().Be(expectedCycles);
        }

        [TestCase((ushort)0xabcd, (byte)0b00000000, (byte)0b00010100, (byte)0xab, (byte)0xce)]
        [TestCase((ushort)0x0000, (byte)0b11101011, (byte)0b11111111, (byte)0x00, (byte)0x01)]
        public void BRK_Test(ushort initialPCRegister, byte initialStatusRegister, byte writenStatusRegister, byte highPCRegister, byte lowPCRegister)
        {
            var processor = GetInstance();
            processor._pc_Register = initialPCRegister;
            processor._status_Register = initialStatusRegister;
            processor._sp_Register = 0xff;
            processor.DataTransfer.Read(0xffff).Returns((byte)0xab);
            processor.DataTransfer.Read(0xfffe).Returns((byte)0xcd);

            processor.BRK();

            processor.DataTransfer.Received(3).Write(Arg.Any<ushort>(), Arg.Any<byte>());
            processor.DataTransfer.Received(1).Write(0x01fe, highPCRegister);
            processor.DataTransfer.Received(1).Write(0x01fd, lowPCRegister);
            processor.DataTransfer.Received(1).Write(0x01fc, writenStatusRegister);
            processor._pc_Register.Should().Be(0xabcd);
        }

        [Test]
        public void JMP_Test()
        {
            var processor = GetInstance();
            processor._operand_Address = _fixture.Create<ushort>();

            processor.JMP();

            processor._pc_Register.Should().Be(processor._operand_Address);
        }

        [Test]
        public void JSR_Test()
        {
            var processor = GetInstance();
            processor._operand_Address = 0xabcd;

            processor.JSR();

            processor.DataTransfer.Received(1).Write(0x01fe, 0xab);
            processor.DataTransfer.Received(1).Write(0x01fd, 0xcd);
            processor._pc_Register.Should().Be(processor._operand_Address);
        }

        [Test]
        public void XXX_Test()
        {
            var processor = GetInstance();

            Action action = () => processor.XXX();

            action.Should().Throw<NotImplementedException>().WithMessage("Invalid instruction.");
        }

        private _6502 GetInstance() => new _6502(_dataTransfer);
    }
}
