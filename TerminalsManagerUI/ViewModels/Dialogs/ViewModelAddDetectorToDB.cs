﻿using TerminalsManagerUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TerminalsManagerUI.ViewModels.Dialogs;
using Command;
using TerminalsManagerUI.Models;
using System.Windows;
using Microsoft.Win32;
using System.ComponentModel;
using TerminalsManagerUI.Services.DataRepository;

namespace TerminalsManagerUI.ViewModels.Dialogs
{
    public class ViewModelAddDetectorToDB : DialogViewModelBase<PerimeterDevice>
    {
        #region Properties
        public ICommand OKCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand PasteTerminalsCommand { get; set; }
        //public ICommand OpenFileCommand { get; set; }

        public string DeviceName { get; set; }
        public string DeviceDescription { get; set; }
        
        public string Terminals { get; set; }

        private string _blockRef;
        public string BlockRef
        {
            get => _blockRef;
            set
            {
                _blockRef = value;
                OnPropertyChanged();
            }
        }

        private string _imagePath;
        public string ImagePath 
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        private int _numberOfCables = 1;
        public int NumberOfCables
        {
            get => _numberOfCables;
            set
            {
                _numberOfCables = value;
                OnPropertyChanged();
            }
        }

        public string ConnectionString { get; set; }
#endregion Properties
        // Open Autocad Block Reference file (*.dwg) command
        private RelayCommand _openBlockRefCommand;
        public RelayCommand OpenBlockRefCommand
        {
            get
            {
                return _openBlockRefCommand ??= new RelayCommand(obj =>
                  {
                      var openFileDialog = new OpenFileDialog
                      {
                          Filter = "Text files (*.dwg)|*.dwg|All files (*.*)|*.*"
                      };
                      if (openFileDialog.ShowDialog() == true)
                      {
                          BlockRef = openFileDialog.FileName;
                      }
                  });
            }
        }

        // Open Autocad Block Reference file (*.dwg) command
        private RelayCommand _openImageCommand;
        public RelayCommand OpenImageCommand
        {
            get
            {
                return _openImageCommand ??= new RelayCommand(obj =>
                  {
                      var openFileDialog = new OpenFileDialog
                      {
                          Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
                      };
                      if (openFileDialog.ShowDialog() == true)
                      {
                          ImagePath = openFileDialog.FileName;
                      }
                  });
            }
        }

        public ViewModelAddDetectorToDB()
        {
            CancelCommand = new RelayCommand<IDialogWindow>(Cancel);
            OKCommand = new RelayCommand<IDialogWindow>(OK);
        }

        private void OK(IDialogWindow window)
        {
            if (Terminals?.Length > 0)
            {
                var newPerimeterDevice = new PerimeterDevice()
                {
                    DeviceName = DeviceName,
                    DeviceDescription = DeviceDescription,
                    TerminalsList = ConvertStringToList(Terminals),
                    NumbersOfCable = NumberOfCables,
                    BlockRef = BlockRef,
                    ImagePath = ImagePath
                };
                using (var unitOfWork = new UnitOfWork(new ModelDbContext(ConnectionString)))
                {
                    if(!unitOfWork.IsDbExists)
                    {
                        MessageBox.Show("Database is unavailable!");
                        return;
                    }
                    unitOfWork.PerimeterDevices.Add(newPerimeterDevice);
                    unitOfWork.Complete();
                }
                CloseDialogWithResult(window, newPerimeterDevice);
            }
            else
            {
                Cancel(window);
            }
        }

        private void Cancel(IDialogWindow window)
        {
            CloseDialogWithResult(window, null);
        }

        private static List<string> ConvertStringToList(string terminalsStr)
        {
            if (terminalsStr?.Length > 0)
            {
                var terminalsList = new List<string>();
                List<string> result = terminalsStr.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                foreach (var item in result)
                {
                    if (item.Length > 0)
                    {
                        terminalsList.Add(item);
                    }
                }
                return terminalsList;
            }
            return null;
        }
    }
}
