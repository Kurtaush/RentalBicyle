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
using WpfAppUI.ViewAdmin;
using static WpfAppUI.App;

namespace WpfAppUI
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void Bicycle_Click(object sender, RoutedEventArgs e)
        {
            BicyclesWindow wBicycle = new BicyclesWindow();
            wBicycle.Show();
        }

        private void Client_Click(object sender, RoutedEventArgs e)
        {
            ClientsWindow wClient = new ClientsWindow();
            wClient.Show();
        }

        private void Rental_Click(object sender, RoutedEventArgs e)
        {
            RentalsWindow wRental = new RentalsWindow();
            wRental.Show();
        }

        private void Station_Click(object sender, RoutedEventArgs e)
        {
            StationsWindow wStation = new StationsWindow();
            wStation.Show();
        }

        private void Tariff_Click(object sender, RoutedEventArgs e)
        {
            TariffsWindow wTariff = new TariffsWindow();
            wTariff.Show();
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            UsersWindow wUser = new UsersWindow();
            wUser.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.MainWindow.Show();
            CurrentUser.Identity = null;
            ((MainWindow)Application.Current.MainWindow).UpdateUI();
        }
    }
}
