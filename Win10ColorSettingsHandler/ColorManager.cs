using System;
using System.Runtime.InteropServices;

namespace Win10ColorSettingHandler {
    [StructLayout(LayoutKind.Sequential)]
    public struct DwmColorParams {
        public uint ColorizationColor;
        public uint ColorizationAfterglow;
        public uint ColorizationColorBalance;
        public uint ColorizationAfterglowBalance;
        public uint ColorizationBlurBalance;
        public uint ColorizationGlassReflectionIntensity;
        public uint ColorizationOpaqueBlend;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct ImmersiveColorPreference {
        public uint crStartColor;
        public uint crAccentColor;
    }

    public static class ColorManager {
        // Functions for changing Windows window border colors
        [DllImport("dwmapi.dll", EntryPoint = "#131", CallingConvention = CallingConvention.StdCall)]
        private static extern int DwmpSetColorizationParameters(ref DwmColorParams dcpParams, bool alwaysTrue);
        [DllImport("dwmapi.dll", EntryPoint = "#123")]
        private static extern int DwmGetColorizationColor(out uint color, out bool blend);
        [DllImport("dwmapi.dll", EntryPoint = "#127", CallingConvention = CallingConvention.StdCall)]
        private static extern int DwmpGetColorizationParameters(out DwmColorParams dcpParams);

        private static byte LowByte(UInt32 number) {
            UInt32 mask = 0xFF;
            UInt32 resNum = number & mask;

            return (byte)resNum;
        }

        private static byte GetRValue(UInt32 number) {
            byte rByte = LowByte(number);
            return rByte;
        }

        private static byte GetGValue(UInt32 number) {
            UInt32 shiftedUInt = number >> 8;
            byte rByte = LowByte(shiftedUInt);

            return rByte;
        }

        private static byte GetBValue(UInt32 number) {
            UInt32 shiftedUInt = number >> 16;
            byte rByte = LowByte(shiftedUInt);

            return rByte;
        }

        public static UInt32 RGB(byte r, byte g, byte b) {
            byte rVal = r;
            UInt32 gVal = ((UInt32)g) << 8;
            UInt32 bVal = ((UInt32)b) << 16;

            UInt32 finalColor = r | gVal | bVal;

            return finalColor;
        }

        public static int GetWindowBorderColors(out UInt32 color) {
            DwmColorParams colorParams = new DwmColorParams {
                ColorizationColor = 0,
                ColorizationAfterglow = 0,
                ColorizationColorBalance = 0,
                ColorizationAfterglowBalance = 0,
                ColorizationBlurBalance = 0,
                ColorizationGlassReflectionIntensity = 0,
                ColorizationOpaqueBlend = 0
            };

            var getRes = DwmpGetColorizationParameters(out colorParams);
            UInt32 revColor = colorParams.ColorizationColor & 0x00FFFFFF;
            UInt32 curColor = RGB(GetBValue(revColor), GetGValue(revColor), GetRValue(revColor));

            color = curColor;

            return getRes;
        }

        public static int SetWindowBorderColors(UInt32 color) {
            DwmColorParams colorParams = new DwmColorParams {
                ColorizationColor = 0,
                ColorizationAfterglow = 0x00007fff,
                ColorizationColorBalance = 0xedd816c5,
                ColorizationAfterglowBalance = 0x0000002b,
                ColorizationBlurBalance = 0x6987bf01,
                ColorizationGlassReflectionIntensity = 0,
                ColorizationOpaqueBlend = 0
            };

            var getRes = DwmpGetColorizationParameters(out colorParams);
            if (getRes != 0) {
                return getRes;
            }

            UInt32 rByte = (UInt32)GetRValue(color) << 16;
            UInt32 gByte = (UInt32)GetGValue(color) << 8;
            UInt32 bByte = GetBValue(color);

            UInt32 dwNewColor = (UInt32)((((UInt32)0xC4) << 24) | (rByte | gByte | bByte));
            colorParams.ColorizationColor = dwNewColor;
            colorParams.ColorizationAfterglow = dwNewColor;
            var setRes = DwmpSetColorizationParameters(ref colorParams, false);

            return setRes;
        }

        // Functions for changing Windows accent colors
        [DllImport("uxtheme.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetUserColorPreference(out ImmersiveColorPreference cpcpPreference, bool fForceReload);
        [DllImport("uxtheme.dll", EntryPoint = "#122", CallingConvention = CallingConvention.StdCall)]
        private static extern int SetUserColorPreference(ref ImmersiveColorPreference cpcpPrefere, bool fForceCommit);

        public static int GetAccentColor(out UInt32 color) {
            ImmersiveColorPreference colorPreference = new ImmersiveColorPreference { crStartColor = 0, crAccentColor = 0 };
            var res = GetUserColorPreference(out colorPreference, false);
            color = colorPreference.crAccentColor & 0x00FFFFFF;

            return res;
        }

        public static int SetAccentColor(UInt32 color) {
            ImmersiveColorPreference colorPreference = new ImmersiveColorPreference { crStartColor = 0, crAccentColor = 0 };

            color &= 0x00FFFFFF;
            colorPreference.crAccentColor = color;
            var res = SetUserColorPreference(ref colorPreference, true);

            return res;
        }
    }
}
