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

namespace TerminalsManagerUI.ViewModels
{
    public class ViewModel : ViewModelBase
    {
        private const int AppJsonGenerateSuccess = 100;
        private readonly IDialogService _dialogService;
        private const string JsonFileName = "assembly.json";
        private string AssemblyJsonFilePath;
        private CollectionViewSource _perimeterDevicesCollection = new();
        private string connectionString = @"Server=localhost;Database=acadBlocksDatabase;Trusted_Connection=True;";

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

        public ICollectionView SourceCollection
        {
            get => _perimeterDevicesCollection.View;
            set
            {
                OnPropertyChanged();
            }
        }
        

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
                    
                });
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
                    var newCable = new Cable() { Designation = IncreaseCableNumber(SelectedCable.Designation) };
                    var newViewModelCable = new ViewModelCable(newCable);
                    CablesList.Add(newViewModelCable);
                    SelectedCable = newViewModelCable;
                });
            }
        }

        private string IncreaseCableNumber(string designation)
        {
            if (StringUtils.TryIncreaseLastNumber(designation, out var increaseString))
            { 
                 return increaseString;
            }
            return designation;
        }

        #endregion Commands

        #region Constructor
        public ViewModel( IDialogService dialogService )
        {
            _dialogService = dialogService;
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
                }
            }

            _perimeterDevicesCollection.Source = PerimeterDeviceList;
            
            _perimeterDevicesCollection.Filter += ApplyFilter;

            OnPropertyChanged("SourceCollection");
        
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

        
    }
}
