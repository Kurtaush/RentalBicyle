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

namespace WpfAppUI
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public ClientWindow(string stationId)
        {
            InitializeComponent();
            LoadStation(stationId);
            DataContext = new RentalViewModel();
        }

        private void LoadStation(string id)
        {
            Title = "Станция №" + id;
        }

        private void Rental_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
