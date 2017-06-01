using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace ReadCounterOstan.AndroidCommunication
{
    public class USBListener
    {
        const int WM_DEVICECHANGE = 0x0219; 
        const int DBT_DEVICE_ATTACHED = 0x8000;
        const int DBT_DEVICE_REMOVED = 0x8004;
        const int DBT_DEVTYPVOLUME = 0x00000002;
        Thread threadBroadcast;
        public delegate void OnDeviceAttachedDetachedHandler();
        private OnDeviceAttachedDetachedHandler connectionListener;
        public event OnDeviceAttachedDetachedHandler DeviceConnectionListener
        {
            add { connectionListener += value; }
            remove { connectionListener = null; }
        }

        HwndSource hwndSource = null;
        HwndSourceHook hook;
        Window window;
        public USBListener(Window win)
        {
            window = win;
            window.Closing += win_Closing;
            window.Loaded += window_Loaded;
        }

        void window_Loaded(object sender, RoutedEventArgs e)
        {
            hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
            hook = new HwndSourceHook(WndProc);
            hwndSource.AddHook(hook);
            if (connectionListener != null)
                connectionListener();
        }

        void win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (hwndSource != null)
                hwndSource.RemoveHook(hook);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == 0x219) // device Change
            {
                if (wParam.ToInt32() == DBT_DEVICE_ATTACHED) // Add
                {
                    broadcastChanges();
                }
                else if (wParam.ToInt32() == DBT_DEVICE_REMOVED) // Remove
                {
                    broadcastChanges();
                }
            }
            return IntPtr.Zero;
        }

        private void broadcastChanges()
        {
            if (threadBroadcast == null)
                threadBroadcast = new Thread(broadcast);
            new Thread(broadcast).Start();
            //threadBroadcast.Start();

        }

        private void broadcast()
        {
            if (connectionListener == null)
                return;
            Thread.Sleep(2000);

            window.Dispatcher.Invoke(new Action(() =>
            {
                connectionListener();
            }));
        }

    }
}
