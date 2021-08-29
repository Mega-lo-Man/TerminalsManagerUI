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
        public ICommand AddCommand { get; set; }

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

        public ViewModelCableWindow(/*IList<Cable> cables*/):base()
        {
            using (var unitOfWork = new UnitOfWork(new ModelDbContext()))
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
