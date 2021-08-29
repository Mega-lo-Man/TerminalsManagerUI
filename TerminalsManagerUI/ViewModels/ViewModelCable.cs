using TerminalsManagerUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.ViewModels
{
    public class ViewModelCable : ViewModelBase, ICloneable
    {
        private Cable _cable;

        public string Designation
        {
            get => _cable.Designation;//_designation;
            set
            {
                _cable.Designation = value;
                OnPropertyChanged();
            }
        }

        public string Brand
        {
            get => _cable.Brand;
            set
            {
                _cable.Brand = value;
                OnPropertyChanged();
            }
        }

        public int WiresNumber
        {
            get => _cable.WiresNumber;
            set
            {
                _cable.WiresNumber = value;
                OnPropertyChanged();
            }
        }

        public bool IsArmoured
        {
            get => _cable.IsArmoured;
            set
            {
                _cable.IsArmoured = value;
                OnPropertyChanged();
            }
        }

        public string ErpCode
        {
            get => _cable.ErpCode;
            set
            {
                _cable.ErpCode = value;
                OnPropertyChanged();
            }
        }

        
        public Cable GetCable => _cable;

        private ViewModelCable()
        {

        }
        
        public ViewModelCable(Cable cable)
        {
            _cable = cable;
            /*
            Designation = GetCable.Designation;
            Brand = GetCable.Brand;
            WiresNumber = GetCable.WiresNumber;
            IsArmoured = GetCable.IsArmoured;
            ErpCode = GetCable.ErpCode;
            */
        }
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
