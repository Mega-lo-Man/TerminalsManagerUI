using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using TerminalsManagerUI.Models;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;


namespace TerminalsManagerUI.ViewModels
{
    public class ViewModelAssembly : ViewModelBase, ICloneable
    {
        private Assembly _assembly;

        public bool IsSurgeProtection
        {
            get => _assembly.IsSourgeProtection;
            set
            {
                _assembly.IsSourgeProtection = value;
                OnPropertyChanged();
            }
        }

        public string DeviceName 
        {
            get => _assembly.Device.DeviceName; 
            set
            {
                _assembly.Device.DeviceName = value;
                OnPropertyChanged();
            }
        }

        public string DeviceDescription
        {
            get => _assembly.Device.DeviceDescription;
            set
            {
                _assembly.Device.DeviceDescription = value;
                OnPropertyChanged();
            }
        }

        private Brush _viewModelColor;
        public Brush ViewModelColor
        {
            get => _viewModelColor;
            set
            {
                _viewModelColor = value;
                OnPropertyChanged();
            }
        }

        public List<string> TerminalList
        {
            get => _assembly.Device.TerminalsList;
            private set
            {
                _assembly.Device.TerminalsList = value;
                OnPropertyChanged();
            }
        }

        public string ImagePath
        {
            get => _assembly.Device.ImagePath;
            set
            {
                _assembly.Device.ImagePath = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ViewModelCable> _vMCables = new();
        public ObservableCollection<ViewModelCable> VMCables
        {
            get => _vMCables;
            set
            {
                _vMCables = value;
                //ChangeColor();
                OnPropertyChanged();
            }
        }

        private bool _isTarget;

        public bool IsTarget
        {
            get => _isTarget;
            set
            {
                _isTarget = value;
                OnPropertyChanged();
            }
        }

        private string _dragDropContext;

        public string DragDropContext
        {
            get => _dragDropContext;
            set
            {
                _dragDropContext = value;
                OnPropertyChanged();
            }
        }
        // If the number of available cables in the assembly is equal to zero, then it is necessary to extinguish the cable list
        public Visibility GetVisibility
        {
            get
            {
                return GetAssembly.Device.NumbersOfCable < 1 ? Visibility.Hidden : Visibility.Visible;
            }
        }
        /*
        public void ChangeColor()
        {
            if (GetAssembly.Device.NumbersOfCable == VMCables.Count())
            {
                ViewModelColor = Brushes.LightGreen;
            }
            else
            {
                ViewModelColor = Brushes.Lavender;
            }
        }
        */
        public void SetErrorColor()
        {
            ViewModelColor = Brushes.Red;
        }

        public Assembly GetAssembly
        {
            get
            {
                _assembly.PerimeterCables.Clear();
                foreach (var vmCable in _vMCables)
                {
                    _assembly.PerimeterCables.Add(vmCable.GetCable);
                }
                return _assembly;
            }
        }

        public ViewModelAssembly(Assembly assembly)
        {
            IsTarget = true;
            // Allow Drag and drop between different collections
            DragDropContext = "1";
            _assembly = (Assembly)assembly.Clone();
            _assembly.PerimeterCables = new List<Cable>();
            //ChangeColor();
        }
   
        public object Clone()
        {
            var newAssemblyViewModel = new ViewModelAssembly((Assembly)_assembly.Clone());
            return newAssemblyViewModel;
        }

        public override void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is not ViewModelCable viewModelCable) 
                return;
            //base.Drop(dropInfo);
            
            
            if (dropInfo.IsSameDragDropContextAsSource)
            {
                base.Drop(dropInfo);
            }
            
            if (GetAssembly.Device.NumbersOfCable <= VMCables.Count())
            {
                // Deny Drag and drop between different collections
                DragDropContext = "2";
            }
            /*
            if (GetAssembly.Device.NumbersOfCable == VMCables.Count())
            {
                ViewModelColor = Brushes.LightGreen;
            }*/
            // If the added cable is redundant, we must return one of the cables from the target to the source.
            /*
            else
            {
                ((IList) dropInfo.DragInfo.SourceCollection).Add(VMCables.First());
                ((IList) dropInfo.DragInfo.SourceCollection).RemoveAt(0);
                VMCables.RemoveAt(0);
                VMCables.Add(viewModelCable);
                
            }
            */
            //var target = VMCables;
            //ChangeColor();
        }

        
    }
}
