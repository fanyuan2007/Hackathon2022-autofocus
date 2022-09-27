using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputarAutoLens
{
    class DevAddr
    {
        #region General Access
        public const ushort PROTOCOL_VERSION = 0x0000;
        public const ushort FIRMWARE_VERSION = 0x0004;
        public const ushort LENS_MODEL_NAME = 0x0010;
        public const ushort LENS_REVISION = 0x0030;
        public const ushort LENS_SERIAL_NUM = 0x0032;
        public const ushort LENS_ADDRESS = 0x0034;
        public const ushort CAPABILITIES = 0x0040;
        public const ushort STATUS1 = 0x0042;
        public const ushort STATUS2 = 0x0044;
        public const ushort TEMPERATURE_VAL = 0x0046;
        public const ushort TEMPERATURE_MIN = 0x0048;
        public const ushort TEMPERATURE_MAX = 0x004A;
        public const ushort USER_AREA = 0x0054;
        #endregion

        #region Zoom Control Address
        public const ushort ZOOM_POSITION_VAL = 0x1000;	// Zoom
        public const ushort ZOOM_STOP = 0x1004;
        public const ushort ZOOM_BACKLASH_CANCEL = 0x1006;
        public const ushort ZOOM_INITIALIZE = 0x1008;
        public const ushort ZOOM_INIT_POSITION = 0x1010;
        public const ushort ZOOM_MECH_STEP_MIN = 0x1012;
        public const ushort ZOOM_MECH_STEP_MAX = 0x1014;
        public const ushort ZOOM_POSITION_MIN = 0x1016;
        public const ushort ZOOM_POSITION_MAX = 0x1018;
        public const ushort ZOOM_SPEED_VAL = 0x101A;
        public const ushort ZOOM_SPEED_MIN = 0x101C;
        public const ushort ZOOM_SPEED_MAX = 0x101E;
        public const ushort ZOOM_COUNT_VAL = 0x1020;
        public const ushort ZOOM_COUNT_MAX = 0x1024;
        #endregion

        #region Focus Control Address
        public const ushort FOCUS_POSITION_VAL = 0x2000;	// Focus
        public const ushort FOCUS_STOP = 0x2004;
        public const ushort FOCUS_BACKLASH_CANCEL = 0x2006;
        public const ushort FOCUS_INITIALIZE = 0x2008;
        public const ushort FOCUS_INIT_POSITION = 0x2010;
        public const ushort FOCUS_MECH_STEP_MIN = 0x2012;
        public const ushort FOCUS_MECH_STEP_MAX = 0x2014;
        public const ushort FOCUS_POSITION_MIN = 0x2016;
        public const ushort FOCUS_POSITION_MAX = 0x2018;
        public const ushort FOCUS_SPEED_VAL = 0x201A;
        public const ushort FOCUS_SPEED_MIN = 0x201C;
        public const ushort FOCUS_SPEED_MAX = 0x201E;
        public const ushort FOCUS_COUNT_VAL = 0x2020;
        public const ushort FOCUS_COUNT_MAX = 0x2024;
        #endregion

        #region Iris Control Address
        public const ushort IRIS_POSITION_VAL = 0x3000;	// Iris
        public const ushort IRIS_STOP = 0x3004;
        public const ushort IRIS_BACKLASH_CANCEL = 0x3006;
        public const ushort IRIS_INITIALIZE = 0x3008;
        public const ushort IRIS_INIT_POSITION = 0x3010;
        public const ushort IRIS_MECH_STEP_MIN = 0x3012;
        public const ushort IRIS_MECH_STEP_MAX = 0x3014;
        public const ushort IRIS_POSITION_MIN = 0x3016;
        public const ushort IRIS_POSITION_MAX = 0x3018;
        public const ushort IRIS_SPEED_VAL = 0x301A;
        public const ushort IRIS_SPEED_MIN = 0x301C;
        public const ushort IRIS_SPEED_MAX = 0x301E;
        public const ushort IRIS_COUNT_VAL = 0x3020;
        public const ushort IRIS_COUNT_MAX = 0x3024;
        #endregion

        #region Optical Filter Control Address
        public const ushort OPT_FILTER_POSITION_VAL = 0x4000;  // Optical Filter
        public const ushort OPT_FILTER_INITIALIZE = 0x4008;
        public const ushort OPT_FILTER_INIT_POSITION = 0x4010;
        public const ushort OPT_FILTER_MECH_STEP_MIN = 0x4012;
        public const ushort OPT_FILTER_MECH_STEP_MAX = 0x4014;
        public const ushort OPT_FILTER_POSITION_MIN = 0x4016;
        public const ushort OPT_FILTER_POSITION_MAX = 0x4018;
        public const ushort OPT_FILTER_COUNT_VAL = 0x4020;
        public const ushort OPT_FILTER_COUNT_MAX = 0x4024;
        #endregion

        #region Program Update Address
        public const ushort GOTOUPDATE = 0xFFAA;
        public const ushort PROGRAMFILE_UPDATE = 0xFF00;
        #endregion

    }
}
