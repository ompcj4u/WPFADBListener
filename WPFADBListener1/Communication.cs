using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ReadCounterOstan.Mobile
{
    public class Communication
    {

        #region CommandHelper

        // ro.boot.serialno                 // Serial Number
        // ro.build.version.release         // 6.0.1
        // ro.build.version.sdk             // 23
        // ro.semc.product.name             // Phone Name
        // ro.semc.product.model            // E6833
        // adb install test.apk
        // adb pull /sdcard/demo.mp4 e:\    Download
        // adb push d:\test.apk /sdcard     Upload

        // adb -s DEVICE_ID                 Choose One

        // adb shell dumpsys | grep -A18 "Package \[my.package\]"
        // adb shell dumpsys package my.package | grep versionName

        #endregion

        StringBuilder output = new StringBuilder();
        StringBuilder error = new StringBuilder();
        DataReceivedEventHandler dataErrorListener;
        DeviceErrorEventHandler deviceErrorHandler;
        // DeviceInfo deviceInfo = new DeviceInfo();

        public Communication()
        {
            dataErrorListener = (e, ef) =>
            {
                deviceErrorHandler("", new DeviceErrorEventArgs() { ErrorMessage = ef.Data });
            };
            startServer();
        }

        public bool CopyFileToPhone(string deviceID, string fileName, string destAddress)
        {
            checkFolderExistOnDevice(destAddress.Substring(0, destAddress.LastIndexOf("/")), deviceID);
            bool result = false;
            string data = "", error = "";
            string command = "-s " + deviceID + " push " + fileName + " " + destAddress;

            adbComand(command, (a, b) =>
            {
                data += b.Data;
                if (b.Data != null && b.Data.ToLower().Contains("kb/s"))
                    result = true;
            }, (a, b) =>
            {
                error += b.Data;
                if (b.Data != null && b.Data.ToLower().Contains("kb/s"))
                    result = true;
            });

            return result;
        }

        public bool CopyFileFromPhone(string deviceID, string fileAddressInDevice, string destAddress)
        {
            bool result = false;
            string data = "", error = "";
            string command = " -s {0} pull {1} {2}";
            command = string.Format(command, deviceID, fileAddressInDevice, destAddress);

            adbComand(command, (a, b) =>
            {
                data += b.Data;
                if (b.Data != null && b.Data.ToLower().Contains("kb/s"))
                    result = true;
            }, (a, b) =>
            {
                error += b.Data;
                if (b.Data != null && b.Data.ToLower().Contains("kb/s"))
                    result = true;
            });

            return result;
        }

        private void startServer()
        {
            adbComand("start-server", null, null);
        }

        public void StopServer()
        {
            adbComand("kill-server", null, null);
        }

        public List<string> Devices()
        {
            List<string> result = new List<string>();
            adbComand("devices", (e, ef) =>
            {
                if (ef != null && ef.Data != null &&
                    ef.Data.Contains("\tdevice"))
                    result.Add(ef.Data.Replace("\tdevice", ""));
            }, null);
            return result;
        }

        public string getModelName(string deviceID)
        {
            string adbCommand = "-s {0} shell getprop ro.product.model";
            string command = string.Format(adbCommand, deviceID);
            StringBuilder result = new StringBuilder();
            adbComand(command, (o, ef) => result.Append(ef.Data), null);
            return result.ToString();
        }

        public string getAppVersion(string deviceID, string packageName)
        {
            string adbCommand = " dumpsys package " + packageName + " | grep versionCode ";
            StringBuilder result = new StringBuilder();
            result.Append("");
            AdbShellCommand(adbCommand, deviceID, (o, ef) =>
            {
                if (!string.IsNullOrEmpty(ef.Data) && ef.Data.Contains("version"))
                    result.Append(ef.Data.Trim());
            }, null);
            string[] versions = result.ToString().Split(' ', '=');
            if (versions.Length > 1)
                return versions[1];

            return "0";
        }

        public bool InstallApplication(string deviceID, string packageName, string appAddress)
        {
            StringBuilder output = new StringBuilder();
            string command = " -s " + deviceID + " install -r " + appAddress;
            adbComand(command, (o, ef) =>
            {
                output.AppendLine(ef.Data);
            }, (o, ef) => { });

            bool result = output.ToString().ToLower().Contains("success");
            if (result)
            {
                //string runCommand = " monkey -p {0} -c android.intent.category.LAUNCHER 1";
                //runCommand = string.Format(runCommand, packageName);
                OpenApplication(packageName, deviceID);
            }
            return result;
        }

        private void checkFolderExistOnDevice(string folder, string deviceID)
        {
            string result = "";
            adbComand(" shell if [ -e " + folder + " ]; then echo 1; else echo 0; fi", (a, b) =>
            {
                if (!string.IsNullOrEmpty(b.Data))
                    result = b.Data;
            }, null);
            if (result == "0")
                AdbShellCommand(" mkdir -m 777 -p " + folder, deviceID
                , (a, b) => { string msg = b.Data; }
                , (a, b) => { string msg = b.Data; });
        }



        private void adbComand(string command, DataReceivedEventHandler outputHandler, DataReceivedEventHandler errorHandler)
        {
            var lcmdInfo1 = new ProcessStartInfo(" adb.exe ", command);
            lcmdInfo1.CreateNoWindow = true;
            lcmdInfo1.RedirectStandardOutput = true;
            lcmdInfo1.RedirectStandardError = true;
            lcmdInfo1.UseShellExecute = false;

            Process cmd = new Process();
            cmd.StartInfo = lcmdInfo1;
            if (outputHandler != null)
                cmd.OutputDataReceived += outputHandler;
            if (errorHandler != null)
                cmd.ErrorDataReceived += errorHandler;
            //(o, ef) => error.Append(ef.Data);

            cmd.Start();
            cmd.BeginOutputReadLine();
            cmd.BeginErrorReadLine();
            cmd.WaitForExit();
            cmd.Close();
            string lresulterr1 = error.ToString();
            string lresult1 = output.ToString();
            cmd.Dispose();
        }

        private void AdbShellCommand(string command, string deviceID, DataReceivedEventHandler outputHandler, DataReceivedEventHandler errorHandler)
        {
            ProcessStartInfo lcmdInfo = new ProcessStartInfo(" adb.exe ", " -s " + deviceID + " shell \r\n \r\n " + command);
            lcmdInfo.CreateNoWindow = true;
            lcmdInfo.RedirectStandardOutput = true;
            lcmdInfo.RedirectStandardError = true;
            lcmdInfo.UseShellExecute = false;
            Process cmd = new Process();
            cmd.StartInfo = lcmdInfo;
            if (outputHandler != null)
                cmd.OutputDataReceived += outputHandler;
            if (errorHandler != null)
                cmd.ErrorDataReceived += errorHandler;
            cmd.Start();
            cmd.BeginOutputReadLine();
            cmd.BeginErrorReadLine();
            cmd.WaitForExit();
            cmd.Close();
            string lresulterr1 = error.ToString();
            string lresult1 = output.ToString();
            cmd.Dispose();
        }

        public void CloseApplication(string packageName, string deviceID)
        {
            string command = " am force-stop " + packageName;
            AdbShellCommand(command, deviceID, null, null);
        }

        public void OpenApplication(string packageName, string deviceID)
        {
            string command = " monkey -p " + packageName + " -c android.intent.category.LAUNCHER 1";
            AdbShellCommand(command, deviceID, null, null);
        }

        public string Broadcast(DeviceInfo device, string p)
        {
            String result = "";
            AdbShellCommand(" am broadcast -a " + p ,device.DeviceID 
                , (a, b) => { result += b.Data; }
                , (a, b) => { result += b.Data; });
            if (!result.Contains("data="))
                return "";
            int startIndex = result.IndexOf("data=\"") + 6;
            int endIndex = result.Substring(startIndex).IndexOf("\"");
            string imei = result.Substring(startIndex, endIndex);
            return imei;
        }
    }
}
