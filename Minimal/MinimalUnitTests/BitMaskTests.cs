using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Interfaces;
using Minimal.Utility;
using System;

namespace MinimalUnitTests
{
    [TestClass]
    public class BitMaskTests
    {
        [Flags]
        private enum Bits
        {
            Bit0, Bit1, Bit2, Bit3, Bit4, Bit5, Bit6, Bit7,
            Bit8, Bit9, Bit10, Bit11, Bit12, Bit13, Bit14, Bit15,
            Bit16, Bit17, Bit18, Bit19, Bit20, Bit21, Bit22, Bit23,
            Bit24, Bit25, Bit26, Bit27, Bit28, Bit29, Bit30, Bit31
        }

        [TestMethod]
        public void BitMask_ConstructorTest()
        {
            var bMask = BitMask.Factory();
            Assert.IsNotNull(bMask);
        }

        [TestMethod]
        public void BitMask_SetBitTest()
        {
            var bMask = BitMask.Factory();
            bMask.SetBit((int)Bits.Bit0);
            Assert.IsTrue(bMask.GetBitValue((int)Bits.Bit0));
        }

        [TestMethod]
        public void BitMask_ClearBitTest()
        {
            var bMask = BitMask.Factory();
            bMask.SetBit((int)Bits.Bit30);
            Assert.IsTrue(bMask.GetBitValue((int)Bits.Bit30), "SetBit failed.");
            bMask.ClearBit((int)Bits.Bit30);
            Assert.IsFalse(bMask.GetBitValue((int)Bits.Bit30), "ClearBit failed.");
        }

        [TestMethod]
        public void BitMask_SetAllBitsTest()
        {
            var bMask = BitMask.Factory();
            bMask.ClearAll();
            TestAllBits(bMask, false);
            bMask.SetAll();
            TestAllBits(bMask, true);
        }

        [TestMethod]
        public void BitMask_ClearAllBitsTest()
        {
            var bMask = BitMask.Factory();
            bMask.SetAll();
            TestAllBits(bMask, true);
            bMask.ClearAll();
            TestAllBits(bMask, false);
        }

        private void TestAllBits(IBitMask mask, bool value)
        {
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit0), value, "Bit0 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit1), value, "Bit1 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit2), value, "Bit2 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit3), value, "Bit3 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit4), value, "Bit4 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit5), value, "Bit5 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit6), value, "Bit6 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit7), value, "Bit7 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit8), value, "Bit8 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit9), value, "Bit9 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit10), value, "Bit10 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit11), value, "Bit11 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit12), value, "Bit12 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit13), value, "Bit13 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit14), value, "Bit14 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit15), value, "Bit15 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit16), value, "Bit16 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit17), value, "Bit17 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit18), value, "Bit18 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit19), value, "Bit19 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit20), value, "Bit20 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit21), value, "Bit21 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit22), value, "Bit22 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit23), value, "Bit23 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit24), value, "Bit24 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit25), value, "Bit25 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit26), value, "Bit26 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit27), value, "Bit27 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit28), value, "Bit28 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit29), value, "Bit29 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit30), value, "Bit30 check failed.");
            Assert.AreEqual(mask.GetBitValue((int)Bits.Bit31), value, "Bit31 check failed.");
        }
    }
}
