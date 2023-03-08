using TerminalsManagerUI.Models;
using TerminalsManagerUI.Services;
using TerminalsManagerUI.Services.DataRepository;
using TerminalsManagerUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TerminalsManagerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var localSettingsSaver = new LocalSettingsSaver(new LocalSettingsUnpackaged());
            DataContext =  new ViewModel(new DialogService(), localSettingsSaver);
        }
    }
}
