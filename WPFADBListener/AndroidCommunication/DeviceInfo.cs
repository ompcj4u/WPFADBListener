using System.ComponentModel;

namespace WPFADBListener.AndroidCommunication
{
    public class DeviceInfo : INotifyPropertyChanged
    {

        private string deviceID;
        private string deviceModel;
        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; onPropertyChanged("Status"); }
        }

        public bool IsInstallingApplication { get; set; }

        public string DeviceID
        {
            get { return deviceID; }
            set { deviceID = value; onPropertyChanged("DeviceID"); }
        }
        public string DeviceModel
        {
            get { return deviceModel; }
            set { deviceModel = value; onPropertyChanged("DeviceModel"); }
        }


        public void onPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

}
