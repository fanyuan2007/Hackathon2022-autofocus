using System;
using System.Text;
using SLAB_HID_TO_SMBUS;

namespace ComputarAutoLens
{
    public class UsbCtrl
    {
        public IntPtr connectedDevice;
        public byte i2cAddr = ConfigVal.I2CSLAVEADDR * 2;
        public byte[] receivedData;

        public int UsbGetNumDevices(out uint numDevices)
        {
            numDevices = 0;
            int retval = CP2112_DLL.HidSmbus_GetNumDevices(ref numDevices, 
                ConfigVal.VID, ConfigVal.PID);
            return retval;
        }
        public int UsbGetSnDevice(uint index, out string SnString)
        {
            StringBuilder serialString = new StringBuilder(" ", 260);
            int retval = CP2112_DLL.HidSmbus_GetString(index, 
                ConfigVal.VID, ConfigVal.PID, 
                serialString, CP2112_DLL.HID_SMBUS_GET_SERIAL_STR);
            SnString = serialString.ToString();
            return retval;
        }
        public int UsbOpen(uint deviceNumber)
        {
            int retval = CP2112_DLL.HidSmbus_Open(ref connectedDevice, 
                deviceNumber, ConfigVal.VID, ConfigVal.PID);
            return retval;
        }

        public void UsbClose()
        {
            CP2112_DLL.HidSmbus_Close(connectedDevice);
        }
        public int UsbSetConfig()
        {
            int retval = CP2112_DLL.HidSmbus_SetSmbusConfig(connectedDevice, 
                ConfigVal.BITRATE, i2cAddr, ConfigVal.AUTOREADRESPOND,
                ConfigVal.WRITETIMEOUT, ConfigVal.READTIMEOUT,
                ConfigVal.SCLLOWTIMEOUT, ConfigVal.TRANSFARRETRIES);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;

            retval = CP2112_DLL.HidSmbus_SetGpioConfig(connectedDevice,
                ConfigVal.DIRECTION, ConfigVal.MODE,
                ConfigVal.SPECIAL, ConfigVal.CLKDIV);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;

            retval = CP2112_DLL.HidSmbus_SetTimeouts(connectedDevice, 
                ConfigVal.RESPONSETIMEOUT);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;

            return retval;
        }

        public int UsbRead(ushort segmentOffset, ushort receiveSize)
        {
            byte[] sendData = new byte[ConfigVal.SEGMENTOFFSET_LENGTH];
            sendData[0] = (byte)(segmentOffset >> 8);
            sendData[1] = (byte)segmentOffset;
            byte sendSize = (byte)sendData.Length;

            int retval = CP2112_DLL.HidSmbus_WriteRequest(connectedDevice, i2cAddr, 
                sendData, sendSize);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;

            retval = CP2112_DLL.HidSmbus_ReadRequest(connectedDevice, i2cAddr, receiveSize);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;

            retval = CP2112_DLL.HidSmbus_ForceReadResponse(connectedDevice, receiveSize);
            if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                return retval;

            byte[] receiveData = new byte[62];
            byte status = 0;
            byte totalBytesRead = 0;
            var buffer = new byte[62];
            byte bufferSize = 62;
            byte bytesRead = 0;
            do
            {
                retval = CP2112_DLL.HidSmbus_GetReadResponse(connectedDevice, ref status, 
                    buffer, bufferSize, ref bytesRead);
                if (retval != CP2112_DLL.HID_SMBUS_SUCCESS)
                    return retval;

                Buffer.BlockCopy(buffer, 0, receiveData, totalBytesRead, bytesRead);
                totalBytesRead += bytesRead;

            } while (totalBytesRead < receiveSize);
            Array.Resize(ref receiveData, totalBytesRead);
            receivedData = receiveData;

            return retval;
        }
        public ushort UsbRead2Bytes()
        {
            return (ushort)((receivedData[0] << 8) | receivedData[1]);
        }
        public int CountRead()
        {
            return ((receivedData[0] << 24) | (receivedData[1] << 16) | 
                (receivedData[2] << 8) | (receivedData[3]));
        }
        public int UsbWrite(ushort segmentOffset, ushort writeData)
        {
            byte[] sendData = new byte[ConfigVal.SEGMENTOFFSET_LENGTH + ConfigVal.DATA_LENGTH]; // 2+2
            sendData[0] = (byte)(segmentOffset >> 8);
            sendData[1] = (byte)segmentOffset;
            sendData[2] = (byte)(writeData >> 8);
            sendData[3] = (byte)writeData;
            byte sendSize = (byte)sendData.Length;
            int retval = CP2112_DLL.HidSmbus_WriteRequest(connectedDevice, i2cAddr, 
                sendData, sendSize);
            return retval;
        }
    }
}
