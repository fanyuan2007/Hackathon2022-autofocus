using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputarAutoLens
{
    class ConfigVal
    {
        #region LensConnect Parameters
        // for USB
        public const ushort VID = 0x10C4;   // USB VID
        public const ushort PID = 0x8D08;   // USB PID
        public const byte I2CSLAVEADDR = 0x20; // I2C SlaveAddress (7bitAddress)

        // for LC RegisterBit
        public const ushort INIT_RUN_BIT = 0x0001;
        public const ushort NEED_INIT_BIT = 0x0001;
        public const ushort ZOOM_MASK = 0x0002;
        public const ushort FOCUS_MASK = 0x0004;
        public const ushort IRIS_MASK = 0x0008;
        public const ushort OPT_FILTER_MASK = 0x0010;

        public const ushort OPT_FILTER_SPEED = 1;

        // for Usb Add Error
        public const int LOWHI_ERROR = 0x50;    // Status Bit 0 -> 1 Error
        public const int HILOW_ERROR = 0x51;    // Status Bit 1 -> 0 Error

        // for USB Read Length
        public const byte LENSMODEL_LENGTH = 24;    // Lens model name length
        public const byte USERAREA_LENGTH = 32;     // User area length
        public const byte LENSADDRESS_LENGTH = 4;   // Lens address length
        public const byte LENSCOUNT_LENGTH = 4;      // Count length
        public const byte SEGMENTOFFSET_LENGTH = 2; // SegmentOffset length
        public const byte DATA_LENGTH = 2;          // Data length

        // Status wait Time
        public const ushort LOW_HIGH_WAIT = 20;
        public const ushort WAIT_MAG = 1100;        // [ms] =1[s]=1000[ms]* 1.1times : Status wait magnification
        public const ushort MINIMUM_WAIT = 2000;

        #endregion

        #region Hid Configration Parameters
        // for HidSmbu_SetTimeouts
        public const uint RESPONSETIMEOUT = 0;  // 0 is Clock stretching

        // for  HidSmbus_SetSmbusConfig
        public const uint BITRATE = 100000;     // Default value 100KHz
        public const int AUTOREADRESPOND = 0;   // 0 is false.
        public const ushort WRITETIMEOUT = 1000; // [ms]   
        public const ushort READTIMEOUT = 1000;  // [ms]
        public const ushort SCLLOWTIMEOUT = 0;  // [ms]
        public const ushort TRANSFARRETRIES = 0;// [ms]

        // for HidSmbus_SetGpioConfig
        public const byte DIRECTION = 0xFF;     // HID_SMBUS_GPIO All Output(1) set
        public const byte MODE = 0x00;          // HID_SMBUS_MODE All OpenDrain(0) set 
        public const byte SPECIAL = 0x00;       // GPIO7 Sepcial function OFF(0) 
        public const byte CLKDIV = 0xFF;        // Not Use <if GPIO7 is ClkOut, 48MHz/(2*255)>
        #endregion

    }
}
