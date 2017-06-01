using ReadCounterOstan.AndroidCommunication;
using System.Collections.ObjectModel;
using System.Windows;
using WPFADBListener.AndroidCommunication;

namespace WPFADBListener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<DeviceInfo> devices = new ObservableCollection<DeviceInfo>();
        DeviceMonitor deviceMonitor;
        private string remotePicLocation = "/sdcard/DCIM/Camera/";
        private string packageName = "com.google.android.youtube";

        public MainWindow()
        {
            InitializeComponent();
            datagridDevices.DataContext = devices;
            // this must be called in constructor
            deviceMonitor = new DeviceMonitor(this);
            deviceMonitor.USBListener.DeviceConnectionListener += USBListener_DeviceConnectionListener;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void USBListener_DeviceConnectionListener()
        {
            try
            {
                devices.Clear();
                foreach (var device in deviceMonitor.DeviceList())
                {
                    devices.Add(device);
                }  
            }
            catch
            {

            }
        }

        private void btnGetAppVersion_Click(object sender, RoutedEventArgs e)
        {
            DeviceInfo deviceInfo = datagridDevices.SelectedItem as DeviceInfo;
            if (deviceInfo == null)
                return;
            MessageBox.Show(deviceMonitor.GetAppVersion(deviceInfo.DeviceID, packageName));
        }

        private void btnOpenApplication_Click(object sender, RoutedEventArgs e)
        {
            DeviceInfo deviceInfo = datagridDevices.SelectedItem as DeviceInfo;
            if (deviceInfo == null)
                return;
            deviceMonitor.OpenApplication(deviceInfo.DeviceID, packageName);
        }

        private void btnCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            DeviceInfo deviceInfo = datagridDevices.SelectedItem as DeviceInfo;
            if (deviceInfo == null)
                return;
            deviceMonitor.CloseApplication(deviceInfo.DeviceID, packageName);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            USBListener_DeviceConnectionListener();
        }
    }
}
