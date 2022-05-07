using Command;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TerminalsManagerUI.Services;
using TerminalsManagerUI.Services.DataRepository;

namespace TerminalsManagerUI.ViewModels.Dialogs
{
    public class ViewModelDbConnectionString : DialogViewModelBase<string>
    {
        private string connectionString;
        public string ConnectionString 
        { 
            get => connectionString;
            set 
            {
                connectionString = value;
                OnPropertyChanged();
            }  
        }

        private string _buttonText = "OK";
        public string ButtonText 
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                OnPropertyChanged();
            }
        }

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ViewModelDbConnectionString()
        {
            CancelCommand = new RelayCommand<IDialogWindow>(Cancel);
            OkCommand = new RelayCommand<IDialogWindow>(OK);
        }

        private void OK(IDialogWindow window)
        {
            ButtonText = "Checking...";
            
            if (!CheckConnectionString())
            {
                MessageBox.Show("Database is unavailable!");
                ButtonText = "OK";
                return;
            }
                
            CloseDialogWithResult(window, connectionString);
        }

        private bool CheckConnectionString()
        {
            using var unitOfWork = new UnitOfWork(new ModelDbContext(connectionString));
            var isDbOk = unitOfWork.IsDbExists;
            if (!isDbOk)
                return false;

            return true;
        }

        private void Cancel(IDialogWindow window)
        {
            CloseDialogWithResult(window, null);
        }
    }
}
