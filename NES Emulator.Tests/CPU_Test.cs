using AutoFixture;
using FluentAssertions;
using NES_Emulator.Core.Extensions;
using NES_Emulator.Core.Interfaces;
using NES_Emulator.Core.Processor;
using NSubstitute;
using NUnit.Framework;
using System;

namespace NES_Emulator.Tests
{
    public class CPU_Test
    {
        private Fixture _fixture;
        private IDataTransfer _dataTransfer;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _dataTransfer = Substitute.For<IDataTransfer>();
        }

        /// <summary>
        /// Check that DataTransfer Read method is called when _6502 Read method is called.
        /// </summary>
        [Test]
        public void DataTransferRead_IsCalled_When_CUPReadIsCalled()
        {
            ushort address = _fixture.Create<ushort>();
            _6502 cpu = GetInstance();

            cpu.Read(address);
        
            _dataTransfer.Received().Read(address);
        }

        /// <summary>
        /// Check that DataTransfer Write method is called when _6502 Write method is called.
        /// </summary>
        [Test]
        public void DataTransferWrite_IsCalled_When_CUPWriteIsCalled()
        {
            ushort address = _fixture.Create<ushort>();
            byte data = _fixture.Create<byte>();
            _6502 cpu = GetInstance();

            cpu.Write(address, data);

            _dataTransfer.Received().Write(address, data);
        }

        /// <summary>
        /// Check that IsNegative method returns true whem MSB is equal to 1.
        /// </summary>
        [Test]
        public void _6502IsNegative_ReturnsTrue_When_MSBIs1()
        {
            byte data = 0xFF;
            _6502 cpu = GetInstance();

            bool result = data.IsNegative();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Check that IsNegative method returns false whem MSB is equal to 0.
        /// </summary>
        [Test]
        public void _6502IsNegative_ReturnsFalse_When_MSBIs0()
        {
            byte data = 0x4F;
            _6502 cpu = GetInstance();

            bool result = data.IsNegative();

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Check constructor throws NullArgumentException when DataTransfer is null.
        /// </summary>
        [Test]
        public void Constructor_ThrowArgumentNullExceptione_When_DataTransferIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new _6502(null));
        }

        /// <summary>
        /// Check the status register is initialized with value 0x00 on cpu creation.
        /// </summary>
        [Test]
        public void StatusRegisterEqualToZero_When_6502IsCreated()
        {
            _6502 cpu = GetInstance();

            byte negative = cpu.ReadStatusRegister(Flags6502.Negative);
            byte overflow = cpu.ReadStatusRegister(Flags6502.Overflow);
            byte unused = cpu.ReadStatusRegister(Flags6502.Unused);
            byte _break = cpu.ReadStatusRegister(Flags6502.Break);
            byte decimalMode = cpu.ReadStatusRegister(Flags6502.DecimalMode);
            byte iRQDisable = cpu.ReadStatusRegister(Flags6502.IRQDisable);
            byte zero = cpu.ReadStatusRegister(Flags6502.Zero);
            byte carry = cpu.ReadStatusRegister(Flags6502.Carry);

            Assert.AreEqual(negative, 0);
            Assert.AreEqual(overflow, 0);
            Assert.AreEqual(unused, 0);
            Assert.AreEqual(_break, 0);
            Assert.AreEqual(decimalMode, 0);
            Assert.AreEqual(iRQDisable, 0);
            Assert.AreEqual(zero, 0);
            Assert.AreEqual(carry, 0);
        }

        [Test]
        public void StatusRegisterSetToOne_When_SetFlagMethodISCalled()
        {
            _6502 cpu = GetInstance();

            cpu.SetFlag(Flags6502.Negative);
            cpu.SetFlag(Flags6502.Overflow);
            cpu.SetFlag(Flags6502.Unused);
            cpu.SetFlag(Flags6502.Break);
            cpu.SetFlag(Flags6502.DecimalMode);
            cpu.SetFlag(Flags6502.IRQDisable);
            cpu.SetFlag(Flags6502.Zero);
            cpu.SetFlag(Flags6502.Carry);
            byte negative = cpu.ReadStatusRegister(Flags6502.Negative);
            byte overflow = cpu.ReadStatusRegister(Flags6502.Overflow);
            byte unused = cpu.ReadStatusRegister(Flags6502.Unused);
            byte _break = cpu.ReadStatusRegister(Flags6502.Break);
            byte decimalMode = cpu.ReadStatusRegister(Flags6502.DecimalMode);
            byte iRQDisable = cpu.ReadStatusRegister(Flags6502.IRQDisable);
            byte zero = cpu.ReadStatusRegister(Flags6502.Zero);
            byte carry = cpu.ReadStatusRegister(Flags6502.Carry);

            Assert.AreEqual(negative, 1);
            Assert.AreEqual(overflow, 1);
            Assert.AreEqual(unused, 1);
            Assert.AreEqual(_break, 1);
            Assert.AreEqual(decimalMode, 1);
            Assert.AreEqual(iRQDisable, 1);
            Assert.AreEqual(zero, 1);
            Assert.AreEqual(carry, 1);
        }
        public void ZP0_ConstructsAddressWithZeroHighByte()
        {
            _6502 cpu = GetInstance();
            cpu.DataTransfer = Substitute.For<IDataTransfer>();
            cpu.DataTransfer.Read(Arg.Any<ushort>()).Returns((byte)0xff);
            var result = cpu.AM_ZP0();
            result.Should().Be(0);
        }

        private _6502 GetInstance() => new _6502(_dataTransfer);
    }
}
