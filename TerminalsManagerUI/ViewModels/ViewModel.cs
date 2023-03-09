using Command;
using TerminalsManagerUI.Models;
using TerminalsManagerUI.Services;
using TerminalsManagerUI.Services.DataRepository;
using TerminalsManagerUI.ViewModels.Dialogs;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using TerminalsManagerUI.Utilities;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace TerminalsManagerUI.ViewModels
{
    public partial class ViewModel : ViewModelBase
    {
        private const int AppJsonGenerateSuccess = 100;
        private readonly IDialogService _dialogService;
        private readonly ILocalSettingsSaver _localSettingsSaver;
        private const string JsonFileName = "assembly.json";
        private string AssemblyJsonFilePath;
        private CollectionViewSource _perimeterDevicesCollection = new();
        private CollectionViewSource _perimeterDevicesCollectionCache = new();
        private string connectionString = @"Server=localhost;Database=acadBlocksDatabase;Trusted_Connection=True;";

        #region Commands

        // Add Cable to CableList Command
        private RelayCommand _addCableCommand;
        public RelayCommand AddCableCommand
        {
            get
            {
                return _addCableCommand ??= new RelayCommand(obj =>
                {
                    var dialog = new ViewModelCableWindow()
                    {
                        ConnectionString = connectionString
                    };
                    var result = _dialogService.OpenDialog(dialog);
                    if (result != null)
                    {
                        CablesList.Add(result);
                    }
                });
            }
        }

        // Add detector command
        private RelayCommand _addDetectorCommand;
        public RelayCommand AddDetectorCommand
        {
            get
            {
                return _addDetectorCommand ??= new RelayCommand(obj =>
                {

                    var dialog = new ViewModelAddDetectorToDB()
                    {
                        ConnectionString = connectionString
                    };
                    var result = _dialogService.OpenDialog(dialog);
                    if (result == null) return;
                    var newAssembly = new Assembly(result);
                    PerimeterDeviceList.Add(new ViewModelAssembly(newAssembly));
                });
            }
        }

        //Delete Detector Command
        private RelayCommand _deleteDetectorCommand;
        public RelayCommand DeleteDetectorCommand
        {
            get
            {
                return _deleteDetectorCommand ??= new RelayCommand(obj =>
                {
                    using (var unitOfWork = new UnitOfWork(new ModelDbContext(connectionString)))
                    {
                        var p = unitOfWork.PerimeterDevices.Get(SelectedPerimeterDevice.GetAssembly.Device.Id);

                        unitOfWork.PerimeterDevices.Remove(p);
                        unitOfWork.Complete();
                    }
                      
                    PerimeterDeviceList.Remove(SelectedPerimeterDevice);
                    TerminalsList.Clear();
                });
            }
        }

        //DeleteCacheDetectorCommand
        private RelayCommand _deleteCacheDetectorCommand;
        public RelayCommand DeleteCacheDetectorCommand
        {
            get
            {
                return _deleteCacheDetectorCommand ??= new RelayCommand(obj =>
                {
                    PerimeterDeviceListCache.Remove(SelectedPerimeterDeviceCache);
                    TerminalsList.Clear();
                });
            }
        }

        //Edit Detector Command
        private RelayCommand _editDetectorCommand;
        public RelayCommand EditDetectorCommand
        {
            get
            {
                return _editDetectorCommand ??= new RelayCommand(obj =>
                {
                    var dialog = new ViewModelEditDetector(connectionString);
                    
                    var result = _dialogService.OpenDialog(dialog);
                    Debug.WriteLine("Dialog result: " + result.ToString());
                    if(result == true)
                    {
                        LoadData();
                    }
                });
            }
        }

        // Delete Cable from Assembly command
        private RelayCommand _deleteCableFromAssemblyCommand;
        public RelayCommand DeleteCableFromAssemblyCommand
        {
            get
            {
                return _deleteCableFromAssemblyCommand ??= new RelayCommand(obj =>
                {
                    if (SelectedAssembly == null) return;
                    foreach (var cable in SelectedAssembly.VMCables)
                    {
                        CablesList.Add(cable);
                    }
                    SelectedAssembly.VMCables.Clear();
                    // Allow Drag and drop between different collections
                    SelectedAssembly.DragDropContext = "1";
                    SelectedAssembly.IsTarget = true;
                });
            }
        }

        // Delete all cached assemblies
        private RelayCommand _deleteAllCacheDetectorCommand;
        public RelayCommand DeleteAllCacheDetectorCommand
        {
            get
            {
                return _deleteAllCacheDetectorCommand ??= new RelayCommand(obj =>
                {
                    PerimeterDeviceListCache.Clear();
                });
            }
        }

        // Remove Assembly from ViewModelAssemblyList command
        private RelayCommand _removeAssemblyCommand;
        public RelayCommand RemoveAssemblyCommand
        {
            get
            {
                return _removeAssemblyCommand ??= new RelayCommand(obj =>
                {
                    if(SelectedAssembly != null)
                    {
                        foreach (var cable in SelectedAssembly.VMCables)
                        {
                            CablesList.Add(cable);
                        }
                          
                        ViewModelAssemblyList.Remove(SelectedAssembly);
                    }
                      
                    MessageBox.Show("RemoveAssemblyCommand");
                });
            }
        }

        // Delete Cable from CableList Command
        private RelayCommand _deleteCableCommand;
        public RelayCommand DeleteCableCommand
        {
            get
            {
                return _deleteCableCommand ??= new RelayCommand(obj =>
                {
                    if (SelectedCable == null) return;
                    CablesList.Remove(SelectedCable);
                    MessageBox.Show("DeleteCableCommand");
                });
            }
        }

        // Paste cable command
        private RelayCommand _pasteCableCommand;
        public RelayCommand PasteCableCommand
        {
            get
            {
                return _pasteCableCommand ??= new RelayCommand(obj =>
                {
                    var str = Clipboard.GetText();
                    var result = str.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                    foreach (var item in result.Where(item => item.Length > 0))
                    {
                        CablesList.Add(new ViewModelCable(new Cable()) { Designation = item });
                    }
                });
            }
        }

        // OpenAddCableToDBdialogCommand
        private RelayCommand _openAddCableToDBdialogCommand;
        public RelayCommand OpenAddCableToDBdialogCommand
        {
            get
            {
                return _openAddCableToDBdialogCommand ??= new RelayCommand(obj =>
                {
                    var dialog = new ViewModelAddCableToDB();
                    var result = _dialogService.OpenDialog(dialog);
                    if (result != null)
                    {
                        //PerimeterDeviceList.Add(new ViewModelAssembly(newAssy));
                    }
                });
            }
        }

        //OpenDBConnectionStringCommand
        private RelayCommand _openDBConnectionStringCommand;
        public RelayCommand OpenDBConnectionStringCommand
        {
            get
            {
                return _openDBConnectionStringCommand ??= new RelayCommand(obj =>
                {
                    var dialog = new ViewModelDbConnectionString
                    {
                        ConnectionString = connectionString
                    };
                    var result = _dialogService.OpenDialog(dialog);
                    if (!string.IsNullOrEmpty(result))
                    {
                        connectionString = result;
                        LoadData();
                    }
                });
            }
        }

        //GenerateJsonAndCloseWindow
        private RelayCommand _generateJsonAndCloseWindow;
        public RelayCommand GenerateJsonAndCloseWindow
        {
            get
            {
                return _generateJsonAndCloseWindow ??= new RelayCommand(obj =>
                {
                    var checker = new AssemblyComplitnessChecker(ViewModelAssemblyList);
                    var errorAssemblies = checker.GetErrors();
                    if ( errorAssemblies.Any() )
                    {
                        foreach (var errorAssembly in errorAssemblies)
                        {
                            errorAssembly.SetErrorColor();
                        }
                    }
                    else
                    {
                        SaveCacheDevices();
                        GenerateAndSaveJson();
                    }

                });
            }
        }

        private void GenerateAndSaveJson()
        {
            var lst = new List<int>();
            foreach (var assemblyCache in PerimeterDeviceListCache)
            {
                lst.Add(assemblyCache.GetAssembly.Device.Id);
            }
            _localSettingsSaver.SetPerimeterDevicesIdSetting(lst);
            var assemblySaver = new AssemblySaver
            {
                TargetFileName = AssemblyJsonFilePath
            };
            if (assemblySaver.Save(ViewModelAssemblyList))
            {
                Environment.Exit(AppJsonGenerateSuccess);
            }
            else
            {
                MessageBox.Show("Failed to save file.");
            }
        }

        //OnKeyDownHandler
        private RelayCommand _onEnterKeyDownHandler;
        public RelayCommand OnEnterKeyDownHandler
        {
            get
            {
                return _onEnterKeyDownHandler ??= new RelayCommand(obj =>
                {
                    if(SelectedCable?.Designation == null)
                    { 
                        return; 
                    }
                    var newCable = new Cable() { Designation = IncreaseCableNumber(SelectedCable.Designation) };
                    var newViewModelCable = new ViewModelCable(newCable);
                    CablesList.Add(newViewModelCable);
                    SelectedCable = newViewModelCable;
                });
            }
        }

        #endregion Commands

        #region Constructor
        public ViewModel( IDialogService dialogService, ILocalSettingsSaver localSettingsSaver )
        {
            _dialogService = dialogService;
            _localSettingsSaver = localSettingsSaver;
            AssemblyJsonFilePath = System.IO.Path.GetTempPath() + JsonFileName;
            PerimeterDeviceList = new ObservableCollection<ViewModelAssembly>();
            TerminalsList = new System.Collections.ObjectModel.ObservableCollection<string>();
            var firstVievModelCable = new ViewModelCable(new Cable() { Designation = "K1" });
            CablesList = new()
            {
                firstVievModelCable
            };
            SelectedCable = firstVievModelCable;
            ViewModelAssemblyList = new ObservableCollection<ViewModelAssembly>();

            LoadData();
            SetAddDeviceCacheEvent();
        }

        private void SetAddDeviceCacheEvent()
        {
            //We must add items in second collection too
            _viewModelAssemblyList.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var senderCollection = (ICollection<ViewModelAssembly>)sender;
                    var lastElement = senderCollection.Last();
                    foreach (var deviceCache in PerimeterDeviceListCache)
                    {
                        if (deviceCache.DeviceName.Equals(lastElement.DeviceName))
                            return;
                    }
                    PerimeterDeviceListCache.Add(senderCollection.Last());
                }
            };
        }
        #endregion Constructor   


        private void LoadData()
        {
            _perimeterDevicesCollection.SortDescriptions.Clear();
            PerimeterDeviceList.Clear();

            using (var unitOfWork = new UnitOfWork(new ModelDbContext(connectionString)))
            {
                if (!unitOfWork.IsDbExists)
                    return;
                var deviceList = unitOfWork.PerimeterDevices.GetAll();
                foreach (var perimeterDevice in deviceList)
                {
                    var assy = new Assembly(perimeterDevice);
                    PerimeterDeviceList.Add(new ViewModelAssembly(assy));

                    AddCacheDevices(PerimeterDeviceList);
                }
            }

            _perimeterDevicesCollection.Source = PerimeterDeviceList;           
            _perimeterDevicesCollection.Filter += ApplyFilter;

            _perimeterDevicesCollectionCache.Source = PerimeterDeviceListCache;
            _perimeterDevicesCollectionCache.Filter += ApplyFilterCache;

            OnPropertyChanged("SourceCollection");
            OnPropertyChanged("SourceCollectionCache");

        }

        private void AddCacheDevices(System.Collections.ObjectModel.ObservableCollection<ViewModelAssembly> perimeterDeviceList)
        {
            var cacheIds = _localSettingsSaver.GetPerimeterDevicesIdSetting();
            PerimeterDeviceListCache = new (perimeterDeviceList
                .Where(x => cacheIds.Contains(x.GetAssembly.Device.Id)).ToList());
        }

        private void SaveCacheDevices()
        {
            var ids = PerimeterDeviceListCache.Select(x => x.GetAssembly.Device.Id).ToList();
            _localSettingsSaver.SetPerimeterDevicesIdSetting(ids);
        }

        private void ApplyFilter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                e.Accepted = true;
                return;
            }

            var assy = e.Item as ViewModelAssembly;
            e.Accepted = assy.DeviceName.ToUpper().Contains(FilterText.ToUpper());
        }

        private void ApplyFilterCache(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterTextCache))
            {
                e.Accepted = true;
                return;
            }

            var assy = e.Item as ViewModelAssembly;
            e.Accepted = assy.DeviceName.ToUpper().Contains(FilterTextCache.ToUpper());
        }

        private string IncreaseCableNumber(string designation)
        {
            if (StringUtils.TryIncreaseLastNumber(designation, out var increaseString))
            {
                return increaseString;
            }
            return designation;
        }

        private void UpdateDependentUiElements(ViewModelAssembly viewModelAssembly)
        {
            if (viewModelAssembly != null)
            {
                TerminalsList.Clear();
                foreach (var item in viewModelAssembly.TerminalList)
                {
                    TerminalsList.Add(item);
                }
            }
            if (viewModelAssembly?.ImagePath != null)
            {
                DisplayedImagePath = viewModelAssembly.ImagePath;
            }
        }
    }
}
