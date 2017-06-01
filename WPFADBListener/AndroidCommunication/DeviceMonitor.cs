using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WPFADBListener.AndroidCommunication;

namespace ReadCounterOstan.AndroidCommunication
{
    public class DeviceMonitor
    {
        private Communication com;
        List<DeviceInfo> lstDevices;

        // it must be called in window_loaded
        public USBListener USBListener { get; set; }

        public DeviceMonitor(Window win)
        {
            com = new Communication();
            USBListener = new USBListener(win);
            win.Closing += Win_Closing;
        }

        private void Win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            com.StopServer();
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

        /// <summary>
        /// copy a file to device. 
        /// </summary>
        /// <param name="deviceID">device id</param>
        /// <param name="filePath">path to file (must not contain space in address)</param>
        /// <param name="destAddress">destination address must be like this '/sdcard/MyFiles/myfile.jpg'</param>
        /// <returns></returns>
        public bool CopyFileToDevice(string deviceID, string filePath,string destAddress)
        {
            return com.CopyFileToPhone(deviceID, filePath, destAddress);
            
        }

        /// <summary>
        /// copy a file from device to computer
        /// </summary>
        /// <param name="deviceID">device id</param>
        /// <param name="destPath">an address exclusive of 'space' character</param>
        /// <param name="deviceSourcePath">file address on device is usually like '/sdcard/MyFiles/myfile.jpg'</param>
        /// <returns></returns>
        public bool CopyFileFromDevice(string deviceID, string destPath, string deviceSourcePath)
        {
            return com.CopyFileFromPhone(deviceID, deviceSourcePath, destPath);
            
        }


        public void CloseApplication(string deviceID, string packageName)
        {
            com.CloseApplication(packageName, deviceID);
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
            return info;
        }

        public bool InstallApplication(string deviceID,  string packageName, string appAddress = "apps\\app.apk")
        {
            try
            {
                return com.InstallApplication(deviceID, packageName, appAddress);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CopyFile(string deviceID, string fileSourceAddress, string fileDestAddress)
        {
            return com.CopyFileToPhone(deviceID, fileSourceAddress, fileDestAddress);
        }

        /// <summary>
        /// get an application version (int), zero means not installed
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public string GetAppVersion(string deviceID, string packageName)
        {
            return com.getAppVersion(deviceID, packageName);

        }
    }
}
