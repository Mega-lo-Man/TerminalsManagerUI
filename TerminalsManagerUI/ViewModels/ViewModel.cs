using Command;
using GongSolutions.Wpf.DragDrop;
using TerminalsManagerUI.Models;
using TerminalsManagerUI.Services;
using TerminalsManagerUI.Services.DataRepository;
using TerminalsManagerUI.ViewModels.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TerminalsManagerUI.ViewModels
{
    public class ViewModel : ViewModelBase
    {
        private const int AppJsonGenerateSuccess = 100;
        private readonly IDialogService _dialogService;
        private const string AssemblyJsonFileName = "c:\\Temp\\assembly.json";
        private readonly CollectionViewSource _perimeterDevicesCollection;

        #region Properties

        private System.Collections.ObjectModel.ObservableCollection<ViewModelAssembly> _perimeterDeviceList;
        public System.Collections.ObjectModel.ObservableCollection<ViewModelAssembly> PerimeterDeviceList
        {
            get => _perimeterDeviceList;
            set
            {
                _perimeterDeviceList = value;
                OnPropertyChanged();
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<string> _terminalsList;
        public System.Collections.ObjectModel.ObservableCollection<string> TerminalsList
        {
            get => _terminalsList;
            set
            {
                _terminalsList = value;
                OnPropertyChanged();
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<ViewModelCable> _cablesList;
        public System.Collections.ObjectModel.ObservableCollection<ViewModelCable> CablesList
        {
            get => _cablesList;
            set
            {
                _cablesList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ViewModelAssembly> _viewModelAssemblyList;
        public ObservableCollection<ViewModelAssembly> ViewModelAssemblyList
        {
            get => _viewModelAssemblyList;
            set
            {
                _viewModelAssemblyList = value;
                
                OnPropertyChanged();
            }
        }

        private ViewModelAssembly _selectedPerimeterDevice;
        public ViewModelAssembly SelectedPerimeterDevice
        {
            get => _selectedPerimeterDevice;
            set
            {
                _selectedPerimeterDevice = value;

                if (_selectedPerimeterDevice != null)
                {
                    TerminalsList.Clear();
                    foreach (var item in _selectedPerimeterDevice.TerminalList)
                    {
                        TerminalsList.Add(item);
                    }
                }
                if (_selectedPerimeterDevice?.ImagePath != null)
                {
                    DisplayedImagePath = _selectedPerimeterDevice.ImagePath;
                }
                OnPropertyChanged();
            }
        }

        private string _displayedImagePath;
        public string DisplayedImagePath
        {
            get => _displayedImagePath;
            set
            {
                _displayedImagePath = value;
                OnPropertyChanged();
            }
        }

        private ViewModelCable _selectedCable;
        public ViewModelCable SelectedCable
        {
            get => _selectedCable;
            set
            {
                _selectedCable = value;
                OnPropertyChanged();
            }
        }

        private ViewModelAssembly _selectedAssembly;
        public ViewModelAssembly SelectedAssembly
        {
            get => _selectedAssembly;
            set
            {
                _selectedAssembly = value;
                DisplayedImagePath = _selectedAssembly?.ImagePath;
                OnPropertyChanged();
            }
        }
        private string _selectedTerminal;

        public string SelectedTerminal
        {
            get => _selectedTerminal;
            set
            {
                _selectedTerminal = value;
                OnPropertyChanged();
            }
        }

        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                _perimeterDevicesCollection.View.Refresh();
                OnPropertyChanged();
            }
        }

        public ICollectionView SourceCollection => _perimeterDevicesCollection.View;

        #endregion Properties

        #region Commands

        // Add Cable to CableList Command
        private RelayCommand _addCableCommand;
        public RelayCommand AddCableCommand
        {
            get
            {
                return _addCableCommand ??= new RelayCommand(obj =>
                {
                    var dialog = new ViewModelCableWindow(/*GetCablesList()*/);
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

                    var dialog = new ViewModelAddDetectorToDB();
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
                    using (var unitOfWork = new UnitOfWork(new ModelDbContext()))
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

        //Edit Detector Command
        private RelayCommand _editDetectorCommand;
        public RelayCommand EditDetectorCommand
        {
            get
            {
                return _editDetectorCommand ??= new RelayCommand(obj =>
                {
                    var dialog = new ViewModelEditDetector();
                    var result = _dialogService.OpenDialog(dialog);
                    Debug.WriteLine("Dialog result: " + result.ToString());
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
                        var assemblySaver = new AssemblySaver
                        {
                            TargetFileName = AssemblyJsonFileName
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
                    
                });
            }
        }
        #endregion Commands

        #region Constructor
        public ViewModel( IDialogService dialogService )
        {
            _dialogService = dialogService;

            PerimeterDeviceList = new ObservableCollection<ViewModelAssembly>();
            TerminalsList = new System.Collections.ObjectModel.ObservableCollection<string>();
            CablesList = new ObservableCollection<ViewModelCable>();
            ViewModelAssemblyList = new ObservableCollection<ViewModelAssembly>();

            
            using (var unitOfWork = new UnitOfWork(new ModelDbContext()))
            {
                var deviceList = unitOfWork.PerimeterDevices.GetAll();
                foreach (var perimeterDevice in deviceList)
                {
                    var assy = new Assembly(perimeterDevice);
                    PerimeterDeviceList.Add(new ViewModelAssembly(assy));
                }
            }

            _perimeterDevicesCollection = new CollectionViewSource
            {
                Source = PerimeterDeviceList
            };
            _perimeterDevicesCollection.Filter += ApplyFilter;
        }
        #endregion Constructor   

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

        
    }
}
