using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using Command;
using TerminalsManagerUI.Models;
using TerminalsManagerUI.Services;
using TerminalsManagerUI.Services.DataRepository;

namespace TerminalsManagerUI.ViewModels.Dialogs
{
    public class ViewModelEditDetector : DialogViewModelBase<bool>
    {

        private string _textBind;

        #region Properties

        public RelayCommand<IDialogWindow> OkCommand { get; }

        public string TextBind
        {
            get => _textBind;
            set
            {
                _textBind = value;
                OnPropertyChanged();
            }
        }

        private List<ViewModelPerimeterDevice> _perimeterDevicesViewModels;
        public List<ViewModelPerimeterDevice> PerimeterDevicesViewModels
        {
            get => _perimeterDevicesViewModels;
            set
            {
                _perimeterDevicesViewModels = value;
                OnPropertyChanged();
            }
        }

        public string ConnectionString { get; set; }
        #endregion Properties

        public ViewModelEditDetector()
        {
            OkCommand = new RelayCommand<IDialogWindow>(Ok);

            using (var unitOfWork = new UnitOfWork(new ModelDbContext(ConnectionString)))
            {
                var perimeterDevices = unitOfWork.PerimeterDevices.GetAllPerimeterDevices().ToList();
                PerimeterDevicesViewModels = new();
                foreach (var device in perimeterDevices)
                {
                    PerimeterDevicesViewModels.Add(new ViewModelPerimeterDevice(device));
                }
            }
            TextBind = "123";
        }

        

        private void Ok(IDialogWindow window)
        {
            using (var unitOfWork = new UnitOfWork(new ModelDbContext(ConnectionString)))
            {
                foreach (var item in PerimeterDevicesViewModels)
                {
                    if (item.WasChanged)
                    {
                        var device = unitOfWork.PerimeterDevices.Get(item.Id);
                        device.NumbersOfCable = item.NumbersOfCable;
                        device.BlockRef = item.BlockRef;
                        device.DeviceDescription = item.DeviceDescription;
                        device.DeviceName = item.DeviceName;
                        device.ImagePath = item.ImagePath;
                        device.TerminalString = item.TerminalString;
                        device.ErpCode = item.ErpCode;
                    }
                }
                unitOfWork.Complete();
            }
            CloseDialogWithResult(window, true);
        }
    }
}