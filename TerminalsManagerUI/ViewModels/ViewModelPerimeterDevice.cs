using TerminalsManagerUI.Models;

namespace TerminalsManagerUI.ViewModels
{
    public class ViewModelPerimeterDevice : ViewModelBase
    {
        private string _erpCode;
        public string ErpCode
        {
            get => _erpCode;
            set
            {
                if (string.Equals(_erpCode, value)) return;
                _erpCode = value;
                WasChanged = true;
                OnPropertyChanged();
            }
        }

        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName;
            set
            {
                if (string.Equals(_deviceName, value)) return;
                _deviceName = value;
                WasChanged = true;
                OnPropertyChanged();

            }
        }

        private string _deviceDescription; 
        public string DeviceDescription
        {
            get => _deviceDescription;
            set
            {
                if (string.Equals(_deviceDescription, value)) return;
                _deviceDescription = value;
                WasChanged = true;
                OnPropertyChanged();
            }
        }

        private string _terminalString;
        public string TerminalString
        {
            get => _terminalString;
            set
            {
                if (string.Equals(_terminalString, value)) return;
                _terminalString = value;
                WasChanged = true;
                OnPropertyChanged();
            }
        }


        private int _numberOfCable;
        public int NumbersOfCable
        {
            get => _numberOfCable;
            set
            {
                if (string.Equals(_numberOfCable, value)) return;
                _numberOfCable = value;
                WasChanged = true;
                OnPropertyChanged();
            }
        }

        private string _blockRef;
        public string BlockRef
        {
            get => _blockRef;
            set
            {
                if (string.Equals(_blockRef, value)) return;
                _blockRef = value;
                WasChanged = true;
                OnPropertyChanged();
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (string.Equals(_imagePath, value)) return;
                _imagePath = value;
                WasChanged = true;
                OnPropertyChanged();
            }
        }

        public int Id { get; }

        public bool WasChanged { get; set; }

        public PerimeterDevice GetPerimeterDevice { get; }

        #region Constructor

        public ViewModelPerimeterDevice(PerimeterDevice perimeterDevice)
        {
            GetPerimeterDevice = perimeterDevice;

            Id = perimeterDevice.Id;
            ErpCode = perimeterDevice.ErpCode;
            DeviceName = perimeterDevice.DeviceName;
            DeviceDescription = perimeterDevice.DeviceDescription;
            TerminalString = perimeterDevice.TerminalString;
            NumbersOfCable = perimeterDevice.NumbersOfCable;
            BlockRef = perimeterDevice.BlockRef;
            ImagePath = perimeterDevice.ImagePath;
        }

        #endregion Constructor
    }
}