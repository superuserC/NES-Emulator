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

            processor.Reset();

            processor._pc_Register.Should().Be(0xcdab);
            processor._acc_Register.Should().Be(0);
            processor._x_Register.Should().Be(0);
            processor._y_Register.Should().Be(0);
            processor._sp_Register.Should().Be(0);
            processor._status_Register.Should().Be(0);
        }

        private _6502 GetInstance() => new _6502(_dataTransfer);
    }
}
