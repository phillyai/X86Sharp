using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace X86Sharp.Tests
{
    [TestClass]
    public class VMTests
    {
        [TestMethod, TestCategory("Instruction")]
        public void LoadInstructionsTests()
        {
            VM vm = new VM();
            try
            {
                vm.Instructions.GetInstructionFromType<InstructionCallback0args>(InstructionType.Nop)();
            }
            catch (KeyNotFoundException)
            {
                Assert.Fail("Fail to load nop instruction");
            }
        }

        [TestMethod, TestCategory("Instruction")]
        public void RegisterTests()
        {
            VM vm = new VM();
            vm.Registers.EAX = 42;
            Assert.AreEqual((uint)42, vm.Registers.EAX);

            ref var eax = ref vm.Registers.EAX;
            eax = 24;
            Assert.AreEqual((uint)24, vm.Registers.EAX);

            uint num = 42;
            var mov = vm.Instructions.GetInstructionFromType<InstructionCallback2args>(InstructionType.Mov);
            mov(ref eax, ref num);
            Assert.AreEqual((uint)42, vm.Registers.EAX);
        }

        [TestMethod, TestCategory("Memory")]
        public void MemorySpanTests()
        {
            VM vm = new VM();

            unsafe
            {
                var dword0to4 = vm.Memory.GetValue(new Address(displacement: 0), 4);
                var mov = vm.Instructions.GetInstructionFromType<InstructionCallback2args>(InstructionType.Mov);
                ref var refptr = ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref dword0to4[0]));
                uint num = 0x12345678;
                mov(ref refptr, ref num);

                var res = BitConverter.ToUInt32(new byte[4] { dword0to4[0], dword0to4[1], dword0to4[2], dword0to4[3] }, 0);
                Assert.AreEqual(num, res);

                System.Diagnostics.Debugger.Break();
            }
        }
    }
}
