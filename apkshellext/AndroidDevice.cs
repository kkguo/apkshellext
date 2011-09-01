using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KKHomeProj.ShellExtInts;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

namespace KKHomeProj.Android
{
    public class AndroidDevice
    {
        public enum AndroidDeviceStatus
        {
            DEVICE,
            OFFLINE,
            BOOTLOADER,
        }
        public readonly string Serialno = "";

        public AndroidDeviceStatus Status
        {
            get
            {
                return m_status;
            }
        }
        public bool ConnectedFromWIFI
        {
            get
            {
                return NativeMethods.isIPAddress(Serialno);
            }
        }

        private AndroidDeviceStatus m_status;

        public static ArrayList GetAndroidDevices()
        {
            AndroidToolAdb adb = new AndroidToolAdb();
            adb.StartServer();
            return GetAndroidDevices(adb.Devices());
        }
        public static ArrayList GetAndroidDevices(Stream adbinfo)
        {
            ArrayList devices = new ArrayList();
            StreamReader sr = new StreamReader(adbinfo);
            
            Regex r1 = new Regex(@"^(\S*)\s+(device|offline|bootloader)$");

            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                if (r1.IsMatch(s))
                {
                    AndroidDevice d = new AndroidDevice(r1.Match(s).Groups[1].Value,
                                        string2state(r1.Match(s).Groups[2].Value));
                    devices.Add(d);
                }
            }
            return devices;
        }
        private static AndroidDeviceStatus string2state(string state)
        {
            if (state == "device")
            {
                return AndroidDeviceStatus.DEVICE;
            }
            else if (state == "bootloader")
            {
                return AndroidDeviceStatus.BOOTLOADER;
            }
            else
            {
                return AndroidDeviceStatus.OFFLINE;
            }
        }

        public AndroidDevice(string serialno, AndroidDeviceStatus status)
        {
            Serialno = serialno;
            m_status = status;
        }

        public AndroidDeviceStatus UpdateStatus()
        {
            m_status = string2state((new AndroidToolAdb()).GetState(Serialno));
            return m_status;
        }
    }
}
