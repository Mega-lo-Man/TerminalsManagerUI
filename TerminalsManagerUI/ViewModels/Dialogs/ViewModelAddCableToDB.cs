using Command;
using TerminalsManagerUI.Models;
using TerminalsManagerUI.Services;
using TerminalsManagerUI.Services.DataRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;

namespace TerminalsManagerUI.ViewModels.Dialogs
{
    public class ViewModelAddCableToDB : DialogViewModelBase<IList<Cable>>, IDataErrorInfo
    {
        private Dictionary<string, string> errors = new();
        private IUnitOfWork _unitOfWork;
        private List<Cable> _cablesList;

        public ICommand OkCommand { get; private set; }

        #region Properties

        private ObservableCollection<ViewModelCable> _cablesListObservable;

        public string ConnectionString { get; set; }

        public ObservableCollection<ViewModelCable> CablesListObservable
        {
            get => _cablesListObservable;
            set
            {
                _cablesListObservable = value;
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
                CableBrand = _selectedCable.Brand;
                WiresNumber = _selectedCable.WiresNumber;
                IsArmoured = _selectedCable.IsArmoured;
                ErpCode = _selectedCable.ErpCode;
                OnPropertyChanged();
            }
        }

        private string _cableBrand;
        public string CableBrand
        {
            get => _cableBrand;
            set
            {
                _cableBrand = value;
                if (_cableBrand?.Length == 0)
                {
                    errors["WiresNumber"] = "Cable brand must be define";
                }
                else
                {
                    errors["WiresNumber"] = null;
                }
                OnPropertyChanged();
            }
        }

        private int _wiresNumber = 4;
        public int WiresNumber
        {
            get => _wiresNumber;
            set
            {
                _wiresNumber = value;
                if(_wiresNumber < 1)
                {
                    errors["WiresNumber"] = "Number if wire must greater than 0";
                }
                else
                {
                    errors["WiresNumber"] = null;
                }
                OnPropertyChanged();
            }
        }

        private bool _isArmoured = false;
        public bool IsArmoured
        { 
            get => _isArmoured;
            set
            {
                _isArmoured = value;
                OnPropertyChanged();
            } 
        }

        //ErpCode
        private string _erpCode;
        public string ErpCode
        {
            get => _erpCode;
            set
            {
                _erpCode = value;
                OnPropertyChanged();
            }
        }

        public string Error => throw new NotImplementedException();

        public string this[string columnName] => errors.ContainsKey(columnName) ? errors[columnName] : null;


        #endregion Properties

        #region Commands

        //AddCableToDbCommand
        private RelayCommand _addCableToDbCommand;
        public RelayCommand AddCableToDbCommand
        {
            get
            {
                return _addCableToDbCommand ??= new RelayCommand(obj =>
                  {
                      if (CableBrand != null)
                      {
                          var newCable = new Cable
                          {
                              Brand = CableBrand,
                              WiresNumber = WiresNumber,
                              IsArmoured = IsArmoured,
                              ErpCode = ErpCode
                          };
                          _unitOfWork.Cables.Add(newCable);
                          CablesListObservable.Add(new ViewModelCable(newCable));
                      }
                  },
                  (obj) => !errors.Values.Any(x => x != null));
            }
        }

        //DeleteCableFromDBCommand
        private RelayCommand _deleteCableFromDBCommand;
        public RelayCommand DeleteCableFromDBCommand
        {
            get
            {
                return _deleteCableFromDBCommand ??= new RelayCommand(obj =>
                {
                    if (SelectedCable != null)
                    {
                        _unitOfWork.Cables.Remove(SelectedCable.GetCable);
                        CablesListObservable.Remove(SelectedCable);
                    }
                });
            }
        }

        #endregion Commands

        public ViewModelAddCableToDB()
        {
            using (var unitOfWork = new UnitOfWork(new ModelDbContext(ConnectionString)))
            {
                if (!unitOfWork.IsDbExists)
                {
                    MessageBox.Show("Database is unavailable!");
                    return;
                }
                _cablesList = (List<Cable>)_unitOfWork.Cables.GetAllCables();
            }
            OkCommand = new RelayCommand<IDialogWindow>(Ok);

            CablesListObservable = new ObservableCollection<ViewModelCable>();
            foreach (var cable in _cablesList)
            {
                CablesListObservable.Add(new ViewModelCable(cable));
            }
        }

        private void Ok(IDialogWindow window)
        {
            using (var unitOfWork = new UnitOfWork(new ModelDbContext(ConnectionString)))
            {
                if (!unitOfWork.IsDbExists)
                {
                    MessageBox.Show("Database is unavailable!");
                    return;
                }
                unitOfWork.Complete();
            }
            CloseDialogWithResult(window, _cablesList);
        }
    }
}
