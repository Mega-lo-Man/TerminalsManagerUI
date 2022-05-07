using TerminalsManagerUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services
{
    public abstract class DialogViewModelBase<T> : INotifyPropertyChanged
    {
        private T _perimeterDevice;

        public string Title { get; set; }
        public string Message { get; set; }

        public T DialogResult { get; set; }

        
        public DialogViewModelBase() : this(string.Empty, string.Empty) { }
        public DialogViewModelBase(string title) : this(title, string.Empty) { }
        public DialogViewModelBase(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public void CloseDialogWithResult(IDialogWindow dialog, T result)
        {
            DialogResult = result;
            if (dialog != null)
                dialog.DialogResult = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
