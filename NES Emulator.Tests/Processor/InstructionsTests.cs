using AutoFixture;
using FluentAssertions;
using NES_Emulator.Core.Extensions;
using NES_Emulator.Core.Interfaces;
using NES_Emulator.Core.Processor;
using NSubstitute;
using NUnit.Framework;

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

        private _6502 GetInstance() => new _6502(_dataTransfer);
    }
}
