using Command;
using TerminalsManagerUI.Models;
using TerminalsManagerUI.Services;
using TerminalsManagerUI.Services.DataRepository;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace TerminalsManagerUI.ViewModels.Dialogs
{
    public class ViewModelCableWindow : DialogViewModelBase<ViewModelCable>
    {

        private List<Cable> _cables;

        #region Properties
        public ICommand AddCommand { get; set; }

        public string ConnectionString { get; set; }

        public List<Cable> Cables
        { 
            get => _cables;
            set
            {
                _cables = value;
                OnPropertyChanged();
            }
        }

        private string _brand;
        public string Brand
        {
            get => _brand;
            set
            {
                _brand = value;
                OnPropertyChanged();
            }
        }

        private string _designation;
        public string Designation 
        {
            get => _designation;
            set
            {
                _designation = value;
                OnPropertyChanged();
            }
        }
        #endregion Properties

        #region Commands

        private RelayCommand _addNewCableToDbCommand;
        public RelayCommand AddNewCableToDbCommand
        {
            get
            {
                return _addNewCableToDbCommand ??
                  (_addNewCableToDbCommand = new RelayCommand(obj =>
                  {
                      MessageBox.Show("Не реализовано");
                  }));
            }
        }

        
        #endregion Commands

        public ViewModelCableWindow():base()
        {
            using (var unitOfWork = new UnitOfWork(new ModelDbContext(ConnectionString)))
            {
                _cables = (List<Cable>)unitOfWork.Cables.GetAllCables();
            }
            AddCommand = new RelayCommand<IDialogWindow>(Add);
        }

        private void Add(IDialogWindow window)
        {
            var newCable = new ViewModelCable(new Cable())
            {
                Designation = Designation,
                Brand = Brand,
            };
            CloseDialogWithResult(window, newCable);
        }
    }
}
