using System;
using System.IO;
using SLAB_HID_TO_SMBUS;

namespace ComputarAutoLens
{
    public class Program
    {
        private const string lensConfigFolder = @"C:\ProgramData\AutoFocus\LensConfig\";
        private const string lensConfigFilename = @"lensConfiguration.txt";
        private const ushort defaultLensValue = 4090; // Range: 2535 - 5055
        // 0: NOT using default lens value
        // 1: Using default lens value
        private const uint defaultLensConfigure = 0;

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Error: Invalid number of parameters.");
                return -1;
            }

            // Setup the lens device
            if (DeviceSetup() != 0)
            {
                Console.WriteLine("Error: Failed to setup the device.");
                return -1;
            }

            // Read a file param to check if set lens focus to a default value
            bool useDefaultValue = false;
            try
            {
                useDefaultValue = useDefaultLensValue();
            }
            catch
            {
                Console.WriteLine("Error: Failed to load the configuration file.");
                return -1;
            }

            // Focus move
            if (useDefaultValue)
            {
                LensCtrl.Instance.FocusInit();
                LensCtrl.Instance.FocusMove(defaultLensValue);
            }
            else
            {
                if (ushort.TryParse(args[0], out ushort changeVal))
                {
                    LensCtrl.Instance.FocusInit();
                    LensCtrl.Instance.FocusMove(changeVal);
                }
            }

            return 0;
        }

        static int DeviceSetup()
        {
            // Connect USB
            LensCtrl.Instance.UsbGetNumDevices(out uint numDevices);
            LensCtrl.Instance.UsbGetSnDevice(0, out string snString);
            uint deviceNumber = 0;
            if (LensCtrl.Instance.UsbOpen(deviceNumber) != 0)
            {
                Console.WriteLine("Error: Cannot open the usb lens.");
                LensCtrl.Instance.UsbClose();
                return -1;
            }

            if (LensCtrl.Instance.UsbSetConfig() != 0)
            {
                Console.WriteLine("Error: Failed to config the usb.");
                LensCtrl.Instance.UsbClose();
                return -1;
            }

            if (LensCtrl.Instance.CapabilitiesRead(out ushort Capabilities) != 0)
            {
                Console.WriteLine("Error: Capabilities read failed.");
                LensCtrl.Instance.UsbClose();
                return -1;
            }

            if (LensCtrl.Instance.Status2ReadSet() != 0)
            {
                Console.WriteLine("Error: Failed to retrieve the read status.");
                LensCtrl.Instance.UsbClose();
                return -1;
            }
            ushort InitializeStatus = LensCtrl.Instance.status2;

            if ((Capabilities & ConfigVal.FOCUS_MASK) == ConfigVal.FOCUS_MASK)
            {
                LensCtrl.Instance.FocusParameterReadSet();
                // Determine if it has been Focus Initialized
                if ((InitializeStatus & ConfigVal.FOCUS_MASK) == 0)
                {
                    LensCtrl.Instance.FocusCurrentAddrReadSet();
                }
            }
            else
            {
                return -1;
            }

            return 0;
        }

        static bool useDefaultLensValue()
        {
            // If directory doesn't exist, create the directory
            if (!Directory.Exists(lensConfigFolder))
            {
                Directory.CreateDirectory(lensConfigFolder);
            }

            // If file doesn't exist, create the file with default value 0 (Not using)
            string filepath = lensConfigFolder + lensConfigFilename;
            if (!File.Exists(filepath))
            {
                using (var sw = new StreamWriter(filepath, false))
                {
                    sw.WriteLine(defaultLensConfigure);
                }
            }

            using (var sw = new StreamReader(filepath))
            {
                var val = sw.ReadLine();
                return val.Equals("1");
            }
        }
    }
}

