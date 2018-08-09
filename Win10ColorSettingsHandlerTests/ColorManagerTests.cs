using System;
using Win10ColorSettingHandler;
using NUnit.Framework;

namespace Window10ColorSettingHandlerTests {
    [TestFixture]
    public class ColorManagerTests {
        private UInt32 Orange = 0x00606848;

        [Test]
        public void TestGettingAccentColor() {
            UInt32 currentColor = 0xffffffff;
            var res = ColorManager.GetAccentColor(out currentColor);

            Assert.IsTrue(currentColor != 0xffffffff && res == 0);
        }

        [Test]
        public void TestSettingAccentColor() {
            UInt32 oldColor = 0xffffffff;
            var getRes = ColorManager.GetAccentColor(out oldColor);

            Assert.AreEqual(0, getRes);

            UInt32 newColor = 0xffffffff;
            var setRes = ColorManager.SetAccentColor(0x000C63F7);

            Assert.AreEqual(0, setRes);

            getRes = ColorManager.GetAccentColor(out newColor);
            Assert.AreEqual(0, setRes);
            Assert.AreEqual((UInt32)0x000C63F7, newColor);

            setRes = ColorManager.SetAccentColor(oldColor);
            Assert.AreEqual(0, setRes);
        }

        [Test]
        public void TestSettingWindowsBorderColor() {
            UInt32 oldBorderColor = 0;
            var getCurRes = ColorManager.GetWindowBorderColors(out oldBorderColor);
            Assert.AreEqual(0, getCurRes);

            UInt32 oldAccentColor = 0;
            var getAccentRes = ColorManager.GetAccentColor(out oldAccentColor);
            Assert.AreEqual(0, getAccentRes);

            // This operations should always be done in sequence for border color
            // being set properly
            // ================================================================
            var setAccentRes = ColorManager.SetAccentColor((UInt32)0x000c63f7);
            Assert.AreEqual(0, setAccentRes);
            var setBCRes = ColorManager.SetWindowBorderColors((UInt32)0x004343FF);
            Assert.AreEqual(0, setBCRes);
            // ================================================================

            UInt32 borderColor = 0;
            var getRes = ColorManager.GetWindowBorderColors(out borderColor);
            Assert.AreEqual(0, getRes);
            Assert.AreEqual((UInt32)0x004343FF, borderColor);

            // Restore previous colors
            // ================================================================
            var restAccentColor = ColorManager.SetAccentColor(oldAccentColor);
            Assert.AreEqual(0, restAccentColor);
            var restoreRes = ColorManager.SetWindowBorderColors(oldBorderColor);
            Assert.AreEqual(0, restoreRes);
            // ================================================================
        }
    }
}
