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
using System.Windows.Shapes;
using WpfAppUI.ViewModel;

namespace WpfAppUI.ViewAdmin
{
    /// <summary>
    /// Логика взаимодействия для RentalsWindow.xaml
    /// </summary>
    public partial class RentalsWindow : Window
    {
        public RentalsWindow()
        {
            InitializeComponent();
            DataContext = new RentalViewModel();
        }
    }
}
