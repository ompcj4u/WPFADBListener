using ReadCounterOstan.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ReadCounterOstan.Mobile
{
    public class DeviceMonitor
    {
        private const string internalStorageAddress = "/sdcard/YaldaCo/ReadCounter/in/in.db";
        private static string packageName = "com.yalda.readcounter";
        private Communication com;
        List<DeviceInfo> lstDevices;

        // it must be called in window_loaded
        public USBListener USBListener { get; set; }

        public DeviceMonitor(Window win)
        {
            com = new Communication();
            USBListener = new USBListener(win);
        }

        public List<DeviceInfo> DeviceList()
        {
            lstDevices = new List<DeviceInfo>();
            var deviceIDs = com.Devices();
            foreach (var deviceID in deviceIDs)
            {
                lstDevices.Add(GetDeviceDetails(deviceID));
            }
            return lstDevices;
        }

        public bool CopyDataBaseToDevice(string deviceID, string filePath)
        {
            bool result = com.CopyFileToPhone(deviceID, filePath, internalStorageAddress);
            return true;
        }

        public bool CopyDatabaseFromDevice(string deviceID, string destPath)
        {
            bool result = com.CopyFileFromPhone(deviceID, internalStorageAddress, destPath);
            return result;
        }

        public bool CopyFileFromDevice(string deviceID, string destPath, string sourceAddress)
        {
            bool result = com.CopyFileFromPhone(deviceID, sourceAddress, destPath);
            return result;
        }

        public void CloseApplication(string deviceID)
        {
            com.CloseApplication(packageName, deviceID);
        }

        public void CloseApplication(string deviceID, string packageName)
        {
            com.CloseApplication(packageName, deviceID);
        }

        public void OpenApplication(string deviceID)
        {
            com.OpenApplication(packageName, deviceID);
        }

        public void OpenApplication(string deviceID, string packageName)
        {
            com.OpenApplication(packageName, deviceID);
        }

        private DeviceInfo GetDeviceDetails(string deviceID)
        {
            DeviceInfo info = new DeviceInfo();
            info.DeviceID = deviceID;
            info.DeviceModel = com.getModelName(deviceID);
            info.ApplicationVersion = com.getAppVersion(deviceID, packageName);
            return info;
        }

        public bool InstallApplication(DeviceInfo deviceInfo, string appAddress = "apps\\app.apk")
        {
            try
            {
                bool result = com.InstallApplication(deviceInfo.DeviceID, packageName, appAddress);
                return result;
            }
            catch (Exception ex)
            {
                Log.Write(ex, "InstallApplication");
                return false;
            }
        }

        public string GetIMEIFromPhone(DeviceInfo device)
        {
            string result = "";
            result = com.Broadcast(device, "ir.co.yalda.GET_IMEI");

            return result;
        }

        public bool CopyFile(string deviceID, string fileSourceAddress, string fileDestAddress)
        {
            return com.CopyFileToPhone(deviceID, fileSourceAddress, fileDestAddress);
        }

        public string GetAppVersion(DeviceInfo deviceInfo)
        {
            string result = com.getAppVersion(deviceInfo.DeviceID, packageName);
            return result;
        }

        public string GetAppVersion(DeviceInfo deviceInfo, string packageName)
        {
            string result = com.getAppVersion(deviceInfo.DeviceID, packageName);
            return result;

        }
    }
}
