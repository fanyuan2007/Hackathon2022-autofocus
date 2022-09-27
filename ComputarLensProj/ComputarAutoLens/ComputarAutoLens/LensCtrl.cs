using System;
using System.Text;
using System.Threading;
using SLAB_HID_TO_SMBUS;

namespace ComputarAutoLens
{
    public class LensCtrl : UsbCtrl
    {
        private LensCtrl()
        {
        }
        public ushort zoomMaxAddr, zoomMinAddr, focusMaxAddr, focusMinAddr;
        public ushort irisMaxAddr, irisMinAddr, optFilMaxAddr;
        public ushort zoomCurrentAddr, focusCurrentAddr, irisCurrentAddr, optCurrentAddr;
        public ushort zoomSpeedPPS, focusSpeedPPS, irisSpeedPPS;
        public ushort status2;

        public ushort NoErrChk2BytesRead(ushort segmentOffset)
        {
            UsbRead(segmentOffset, ConfigVal.DATA_LENGTH);
            return UsbRead2Bytes();
        }

        public string StringRead(int retval)
        {
            if ((retval == CP2112_DLL.HID_SMBUS_SUCCESS) & (receivedData != null))
                return Encoding.ASCII.GetString(receivedData);
            return null;
        }
        public int ModelName(out string model)
        {
            int retval = UsbRead(DevAddr.LENS_MODEL_NAME, ConfigVal.LENSMODEL_LENGTH);
            model = StringRead(retval);
            return retval;
        }
        public int UserAreaRead(out string userName)
        {
            int retval = UsbRead(DevAddr.USER_AREA, ConfigVal.USERAREA_LENGTH);
            userName = StringRead(retval);
            return retval;
        }
        public string VersionRead(int retval)
        {
            if ((retval == CP2112_DLL.HID_SMBUS_SUCCESS) & (receivedData != null))
                return String.Format("{0}.{1:D2}", receivedData[0], receivedData[1]);
            return null;
        }
        public int FWVersion(out string version)
        {
            int retval = UsbRead(DevAddr.FIRMWARE_VERSION, ConfigVal.DATA_LENGTH);
            version = VersionRead(retval);
            return retval;
        }
        public int ProtocolVersion(out string version)
        {
            int retval = UsbRead(DevAddr.PROTOCOL_VERSION, ConfigVal.DATA_LENGTH);
            version = VersionRead(retval);
            return retval;
        }
        public int LensRevision(out int revision)
        {
            int retval = UsbRead(DevAddr.LENS_REVISION, ConfigVal.DATA_LENGTH);
            revision = UsbRead2Bytes();
            return retval;
        }
        public int LensAddress(out int i2cAddress)
        {
            int retval= UsbRead(DevAddr.LENS_ADDRESS, ConfigVal.LENSADDRESS_LENGTH);
            i2cAddress = receivedData[0];                 // Read only the 4th byte
            return retval;
        }

        public int CapabilitiesRead(out ushort capabilities)
        {
            int retval = UsbRead(DevAddr.CAPABILITIES, ConfigVal.DATA_LENGTH);
            capabilities = UsbRead2Bytes();
            return retval;
        }
        public int Status1Read(out ushort status1)
        {
            int retval = UsbRead(DevAddr.STATUS1, ConfigVal.DATA_LENGTH);
            status1 = UsbRead2Bytes();
            return retval;
        }
        public int Status2ReadSet()
        {
            int retval = UsbRead(DevAddr.STATUS2, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            status2 = UsbRead2Bytes();
            return retval;
        }
        public int TempKelvin(out int kelvinValue)
        {
            int retval = UsbRead(DevAddr.TEMPERATURE_VAL, 2);
            kelvinValue = UsbRead2Bytes();
            return retval;
        }
        public int UserAreaWrite(byte[] userName)
        {
            byte[] sendData;
            byte userNameSize = (byte)userName.Length;
            byte sendSize = (byte)(userNameSize + ConfigVal.SEGMENTOFFSET_LENGTH);

            ushort SegmentOffset = DevAddr.USER_AREA;
            sendData = new byte[ConfigVal.USERAREA_LENGTH + ConfigVal.SEGMENTOFFSET_LENGTH];                    // 2byte SegmentOffset + 32byte userName
            sendData[0] = (byte)(SegmentOffset >> 8);
            sendData[1] = (byte)SegmentOffset;
            userName.CopyTo(sendData, 2);
            if (userNameSize < ConfigVal.USERAREA_LENGTH)                          // Fill unused space with 0
            {
                int space = ConfigVal.USERAREA_LENGTH - userNameSize;
                for (int i = 0; i < space; i++)
                {
                    sendData[sendSize + i] = 0;
                }
            }
            sendSize = (byte)sendData.Length;
            int retval = CP2112_DLL.HidSmbus_WriteRequest(connectedDevice, i2cAddr, 
                sendData, sendSize);
            return retval;
        }
        int WaitCalc(ushort moveValue, int speedPPS)
        {
            int waitTime = ConfigVal.WAIT_MAG * moveValue / speedPPS;
            if (2000 > waitTime)
                waitTime = 2000;
            return waitTime;
        }
        public int ZoomCurrentAddrReadSet()
        {
            int retval = UsbRead(DevAddr.ZOOM_POSITION_VAL, ConfigVal.DATA_LENGTH);
            zoomCurrentAddr = UsbRead2Bytes();
            return retval;
        }
        public int ZoomParameterReadSet()
        {
            int retval = UsbRead(DevAddr.ZOOM_POSITION_MIN, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            zoomMinAddr = UsbRead2Bytes();

            retval = UsbRead(DevAddr.ZOOM_POSITION_MAX, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            zoomMaxAddr = UsbRead2Bytes();

            retval = UsbRead(DevAddr.ZOOM_SPEED_VAL, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            zoomSpeedPPS = UsbRead2Bytes();

            return retval;
        }
        public int ZoomBacklashRead(out ushort flag)
        {
            int retval = UsbRead(DevAddr.ZOOM_BACKLASH_CANCEL, ConfigVal.DATA_LENGTH);
            flag = UsbRead2Bytes();
            return retval;
        }
        public int ZoomBacklashWrite(ushort flag)
        {
            int retval =  UsbWrite(DevAddr.ZOOM_BACKLASH_CANCEL, flag);
            return retval;
        }
        public int ZoomSpeedMinRead(out ushort speedPPS)
        {
            int retval = UsbRead(DevAddr.ZOOM_SPEED_MIN, ConfigVal.DATA_LENGTH);
            speedPPS = UsbRead2Bytes();
            return retval;
        }
        public int ZoomSpeedMaxRead(out ushort speedPPS)
        {
            int retval = UsbRead(DevAddr.ZOOM_SPEED_MAX, ConfigVal.DATA_LENGTH);
            speedPPS = UsbRead2Bytes();
            return retval;
        }
        public int ZoomSpeedWrite(ushort speedPPS)
        {
            int retval = UsbWrite(DevAddr.ZOOM_SPEED_VAL, speedPPS);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            Thread.Sleep(1);
            retval = UsbRead(DevAddr.ZOOM_SPEED_VAL, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            zoomSpeedPPS = UsbRead2Bytes();
            return retval;
        }
        public int ZoomCountValRead(out int count)
        {
            int retval = UsbRead(DevAddr.ZOOM_COUNT_VAL, ConfigVal.LENSCOUNT_LENGTH);
            count = CountRead();
            return retval;
        }
        public int ZoomCountMaxRead(out int count)
        {
            int retval = UsbRead(DevAddr.ZOOM_COUNT_MAX, ConfigVal.LENSCOUNT_LENGTH);
            count = CountRead();
            return retval;
        }
        public int ZoomInit()   // When using it, do ZoomParameterReadSet
        {
            int waitTime = WaitCalc((ushort)(zoomMaxAddr - zoomMinAddr), zoomSpeedPPS);
            int retval = UsbWrite(DevAddr.ZOOM_INITIALIZE, ConfigVal.INIT_RUN_BIT);
            if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
            {
                retval = StatusWait(DevAddr.STATUS1, ConfigVal.ZOOM_MASK, waitTime);
                if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                {
                    retval = UsbRead(DevAddr.ZOOM_POSITION_VAL, ConfigVal.DATA_LENGTH);
                    if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                    {
                        zoomCurrentAddr = UsbRead2Bytes();
                        Status2ReadSet();
                        return retval;    // General Return Code (May be Success)
                    }
                }
            }
            return retval;
        }
        public int ZoomMove(ushort addrData)
        {
            int moveValue = Math.Abs(addrData - zoomCurrentAddr);
            int waitTime = WaitCalc((ushort)moveValue, zoomSpeedPPS);
            int retval = DeviceMove(DevAddr.ZOOM_POSITION_VAL, ref addrData, 
                ConfigVal.ZOOM_MASK, waitTime);
            if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                zoomCurrentAddr = addrData;
            return retval;
        }
        public int FocusCurrentAddrReadSet()
        {
            int retval = UsbRead(DevAddr.FOCUS_POSITION_VAL, ConfigVal.DATA_LENGTH);
            focusCurrentAddr = UsbRead2Bytes();
            return retval;
        }
        public int FocusParameterReadSet()
        {
            int retval = UsbRead(DevAddr.FOCUS_MECH_STEP_MIN, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            focusMinAddr = UsbRead2Bytes();

            retval = UsbRead(DevAddr.FOCUS_MECH_STEP_MAX, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            focusMaxAddr = UsbRead2Bytes();

            retval = UsbRead(DevAddr.FOCUS_SPEED_VAL, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            focusSpeedPPS = UsbRead2Bytes();

            return retval;
        }
        public int FocusBacklashRead(out ushort flag)
        {
            int retval = UsbRead(DevAddr.FOCUS_BACKLASH_CANCEL, 2);
            flag = UsbRead2Bytes();
            return retval;
        }
        public int FocusBacklashWrite(ushort flag)
        {
            int retval = UsbWrite(DevAddr.FOCUS_BACKLASH_CANCEL, flag);
            return retval;
        }
        public int FocusSpeedMinRead(out ushort speedPPS)
        {
            int retval = UsbRead(DevAddr.FOCUS_SPEED_MIN, ConfigVal.DATA_LENGTH);
            speedPPS = UsbRead2Bytes();
            return retval;
        }
        public int FocusSpeedMaxRead(out ushort speedPPS)
        {
            int retval = UsbRead(DevAddr.FOCUS_SPEED_MAX, ConfigVal.DATA_LENGTH);
            speedPPS = UsbRead2Bytes();
            return retval;
        }
        public int FocusSpeedWrite(ushort speedPPS)
        {
            int retval = UsbWrite(DevAddr.FOCUS_SPEED_VAL, speedPPS);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            Thread.Sleep(1);
            retval = UsbRead(DevAddr.FOCUS_SPEED_VAL, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            focusSpeedPPS = UsbRead2Bytes();
            return retval;
        }
        public int FocusCountValRead(out int count)
        {
            int retval = UsbRead(DevAddr.FOCUS_COUNT_VAL, ConfigVal.LENSCOUNT_LENGTH);
            count = CountRead();
            return retval;
        }
        public int FocusCountMaxRead(out int count)
        {
            int retval = UsbRead(DevAddr.FOCUS_COUNT_MAX, ConfigVal.LENSCOUNT_LENGTH);
            count = CountRead();
            return retval;
        }
        public int FocusInit()  // When using it, do FocusParameterReadSet
        {
            int waitTime = WaitCalc((ushort)(focusMaxAddr - focusMinAddr), focusSpeedPPS);
            int retval = UsbWrite(DevAddr.FOCUS_INITIALIZE, ConfigVal.INIT_RUN_BIT);
            if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
            {
                retval = StatusWait(DevAddr.STATUS1, ConfigVal.FOCUS_MASK, waitTime);
                if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                {
                    retval = UsbRead(DevAddr.FOCUS_POSITION_VAL, ConfigVal.DATA_LENGTH);
                    if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                    {
                        focusCurrentAddr = UsbRead2Bytes();
                        Status2ReadSet();
                        return retval;    // General Return Code (May be Success)
                    }
                }
            }
            return retval;
        }
        public int FocusMove(ushort addrData)
        {
            int moveValue = Math.Abs(addrData - focusCurrentAddr);
            int waitTime = WaitCalc((ushort)moveValue, focusSpeedPPS);
            int retval = DeviceMove(DevAddr.FOCUS_POSITION_VAL, ref addrData, 
                ConfigVal.FOCUS_MASK, waitTime);
            if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                focusCurrentAddr = addrData;
            return retval;
        }
        public int IrisCurrentAddrReadSet()
        {
            int retval = UsbRead(DevAddr.IRIS_POSITION_VAL, ConfigVal.DATA_LENGTH);
            irisCurrentAddr = UsbRead2Bytes();
            return retval;
        }
        public int IrisParameterReadSet()
        {
            int retval = UsbRead(DevAddr.IRIS_MECH_STEP_MIN, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            irisMinAddr = UsbRead2Bytes();
            
            retval = UsbRead(DevAddr.IRIS_MECH_STEP_MAX, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            irisMaxAddr = UsbRead2Bytes();

            retval = UsbRead(DevAddr.IRIS_SPEED_VAL, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            irisSpeedPPS = UsbRead2Bytes();

            return retval;
        }
        public int IrisBacklashRead(out ushort flag)
        {
            int retval = UsbRead(DevAddr.IRIS_BACKLASH_CANCEL, ConfigVal.DATA_LENGTH);
            flag = UsbRead2Bytes();
            return retval;
        }
        public int IrisBacklashWrite(ushort flag)
        {
            int retval = UsbWrite(DevAddr.IRIS_BACKLASH_CANCEL, flag);
            return retval;
         }
        public int IrisSpeedMinRead(out ushort speedPPS)
        {
            int retval = UsbRead(DevAddr.IRIS_SPEED_MIN, ConfigVal.DATA_LENGTH);
            speedPPS = UsbRead2Bytes();
            return retval;
        }
        public int IrisSpeedMaxRead(out ushort speedPPS)
        {
            int retval = UsbRead(DevAddr.IRIS_SPEED_MAX, ConfigVal.DATA_LENGTH);
            speedPPS = UsbRead2Bytes();
            return retval;
        }
        public int IrisSpeedWrite(ushort speedPPS)
        {
            int retval = UsbWrite(DevAddr.IRIS_SPEED_VAL, speedPPS);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            Thread.Sleep(1);
            retval = UsbRead(DevAddr.IRIS_SPEED_VAL, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            irisSpeedPPS = UsbRead2Bytes();
            return retval;
        }
        public int IrisCountValRead(out int count)
        {
            int retval = UsbRead(DevAddr.IRIS_COUNT_VAL, ConfigVal.LENSCOUNT_LENGTH);
            count = CountRead();
            return retval;
        }
        public int IrisCountMaxRead(out int count)
        {
            int retval = UsbRead(DevAddr.IRIS_COUNT_MAX, ConfigVal.LENSCOUNT_LENGTH);
            count = CountRead();
            return retval;
        }
        public int IrisInit()   // When using it, do IrisParameterReadSet
        {
            int waitTime = WaitCalc((ushort)(irisMaxAddr - irisMinAddr), irisSpeedPPS);
            int retval = UsbWrite(DevAddr.IRIS_INITIALIZE, ConfigVal.INIT_RUN_BIT);
            if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
            {
                retval = StatusWait(DevAddr.STATUS1, ConfigVal.IRIS_MASK, waitTime);
                if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                {
                    retval = UsbRead(DevAddr.IRIS_POSITION_VAL, ConfigVal.DATA_LENGTH);
                    if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                    {
                        irisCurrentAddr = UsbRead2Bytes();
                        Status2ReadSet();
                        return retval;    // General Return Code (May be Success)
                    }
                }
            }
            return retval;
        }
        public int IrisMove(ushort addrData)
        {
            int moveValue = Math.Abs(addrData - irisCurrentAddr);
            int waitTime = WaitCalc((ushort)moveValue, irisSpeedPPS);
            int retval = DeviceMove(DevAddr.IRIS_POSITION_VAL, ref addrData, 
                ConfigVal.IRIS_MASK, waitTime);
            if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                irisCurrentAddr = addrData;
            return retval;
        }
        public int OptFilterCurrentAddrReadSet()
        {
            int retval = UsbRead(DevAddr.OPT_FILTER_POSITION_VAL, ConfigVal.DATA_LENGTH);
            optCurrentAddr = UsbRead2Bytes();
            return retval;
        }
        public int OptFilterParameterReadSet()
        {
            int retval = UsbRead(DevAddr.OPT_FILTER_MECH_STEP_MAX, ConfigVal.DATA_LENGTH);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;
            optFilMaxAddr = UsbRead2Bytes();

            retval = OptFilterCurrentAddrReadSet();
            return retval;
        }
        public int OptFilterCountValRead(out int count)
        {
            int retval = UsbRead(DevAddr.OPT_FILTER_COUNT_VAL, ConfigVal.LENSCOUNT_LENGTH);
            count = CountRead();
            return retval;
        }
        public int OptFilterCountMaxRead(out int count)
        {
            int retval = UsbRead(DevAddr.OPT_FILTER_COUNT_MAX, ConfigVal.LENSCOUNT_LENGTH);
            count = CountRead();
            return retval;
        }
        public int OptFilterInit()  // When using it, do OptFilterParameterReadSet
        {
            int waitTime = WaitCalc((ushort)(optFilMaxAddr + 1), ConfigVal.OPT_FILTER_SPEED);
            int retval = UsbWrite(DevAddr.OPT_FILTER_INITIALIZE, ConfigVal.INIT_RUN_BIT);
            if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
            {
                retval = StatusWait(DevAddr.STATUS1, ConfigVal.OPT_FILTER_MASK, waitTime);
                if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                {
                    retval = UsbRead(DevAddr.OPT_FILTER_POSITION_VAL, ConfigVal.DATA_LENGTH);
                    if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                    {
                        optCurrentAddr = UsbRead2Bytes();
                        Status2ReadSet();
                        return retval;    // General Return Code (May be Success)
                    }
                }
            }
            return retval;
        }
        public int OptFilterMove(ushort addrData)
        {
            int moveValue = Math.Abs(addrData - optCurrentAddr);
            int waitTime = WaitCalc((ushort)moveValue, ConfigVal.OPT_FILTER_SPEED);
            int retval = DeviceMove(DevAddr.OPT_FILTER_POSITION_VAL, ref addrData, 
                ConfigVal.OPT_FILTER_MASK, waitTime);
            if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                optCurrentAddr = addrData;
            return retval;
        }
        public int DeviceMove(ushort segmentOffset, ref ushort addrData, ushort mask, int waitTime)
        {
            int retval = UsbWrite(segmentOffset, addrData);   // Write the target address
            if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
            {
                retval = StatusWait(DevAddr.STATUS1, mask, waitTime);    // Check that device bit of status1 goes Hi & Low
                if (retval == CP2112_DLL.HID_SMBUS_SUCCESS)
                {
                    retval = UsbRead(segmentOffset, ConfigVal.DATA_LENGTH);
                    if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                        return retval;
                    addrData = UsbRead2Bytes();
                    return retval;    // General Return Code (May be Success)
                }
                return retval;    // Move Wait Error
            }
            return retval;    // USB Write Error
        }
        public int StatusWait(ushort segmentOffset, ushort statusMask, int waitTime)   // statusMask bit check
        {
            int tmp = 0;
            ushort readStatus;
            int retval;
            do
            {
                retval = UsbRead(segmentOffset, ConfigVal.DATA_LENGTH);
                if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                    return retval;

                readStatus = UsbRead2Bytes();
                tmp += 1;
                if (tmp >= ConfigVal.LOW_HIGH_WAIT)
                    return ConfigVal.LOWHI_ERROR;

            } while ((readStatus & statusMask) != statusMask);  // Check that statusMask bit 0->1

            tmp = 0;
            do
            {
                retval = UsbRead(segmentOffset, ConfigVal.DATA_LENGTH);
                if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                    return retval;

                readStatus = UsbRead2Bytes();
                tmp += 1;
                if (tmp >= waitTime)
                    return ConfigVal.HILOW_ERROR;

                Thread.Sleep(1);
                //Application.DoEvents();
            } while ((readStatus & statusMask) != 0);           // Check that statusMask bit 1->0

            return retval;
        }
        public string ErrorTxt(int returnCode)
        {
            switch (returnCode)
            {
                case 0x01:
                    return "Device not found.";
                case 0x02:
                    return "Invalid handle.";
                case 0x03:
                    return "Invalid device object.";
                case 0x04:
                    return "Invalid parameter.";
                case 0x05:
                    return "Invalid request length.";
                case 0x10:
                    return "Read error.";
                case 0x11:
                    return "Write error.";
                case 0x12:
                    return "Read time out.";
                case 0x13:
                    return "Write time out.";
                case 0x14:
                    return "Device IO failed.";
                case 0x15:
                    return "Device access error.\r\nThe device may already be running.";
                case 0x16:
                    return "Device not supported.";
                case 0x50:
                    return "Status bit high error.";
                case 0x51:
                    return "Status bit low error.";
           }
            return "";
        }
        public static LensCtrl Instance = new LensCtrl();
    }
}
