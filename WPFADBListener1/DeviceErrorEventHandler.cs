using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReadCounterOstan.Mobile
{
    public delegate void DeviceErrorEventHandler(string deviceID,DeviceErrorEventArgs e);

    public class DeviceErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; }
    }

}
