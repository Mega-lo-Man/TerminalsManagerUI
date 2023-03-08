using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.ViewModels
{
    public partial class ViewModel
    {
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

        private System.Collections.ObjectModel.ObservableCollection<ViewModelAssembly> _perimeterDeviceListCache;
        public System.Collections.ObjectModel.ObservableCollection<ViewModelAssembly> PerimeterDeviceListCache
        { 
            get => _perimeterDeviceListCache;
            set
            {
                _perimeterDeviceListCache= value;
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
                UpdateDependentUiElements(_selectedPerimeterDevice);
                OnPropertyChanged();
            }
        }

        //SelectedPerimeterDeviceCache
        private ViewModelAssembly _selectedPerimeterDeviceCache;
        public ViewModelAssembly SelectedPerimeterDeviceCache
        {
            get => _selectedPerimeterDeviceCache;
            set
            {
                _selectedPerimeterDeviceCache = value;

                UpdateDependentUiElements(_selectedPerimeterDeviceCache);
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

        private string _filterTextCache;
        public string FilterTextCache
        {
            get => _filterTextCache;
            set
            {
                _filterTextCache = value;
                _perimeterDevicesCollectionCache.View.Refresh();
                OnPropertyChanged();
            }
        }

        public ICollectionView SourceCollectionCache
        {
            get => _perimeterDevicesCollectionCache.View;
            set
            {
                OnPropertyChanged();
            }
        }

        #endregion Properties
    }
}
