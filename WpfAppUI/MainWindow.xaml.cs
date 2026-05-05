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
using WpfAppUI.Model;
using WpfAppUI.View;
using static WpfAppUI.App;

namespace WpfAppUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ModelRentalBicycle _context = new ModelRentalBicycle();
        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
        }

        public void UpdateUI()
        {
                btnLogin.Visibility = Visibility.Collapsed;
                btnReg.Visibility = Visibility.Collapsed;
                textPass.Visibility = Visibility.Collapsed;
                btnBalance.Visibility = Visibility.Collapsed;
                btnHistory.Visibility = Visibility.Collapsed;
                textAbout.Visibility = Visibility.Collapsed;
                textInfo.Visibility = Visibility.Collapsed;
                btnExitAcc.Visibility = Visibility.Collapsed;
            

            if (CurrentUser.Identity != null)
            {
                if (CurrentUser.Identity.Role == "client")
                {
                    btnBalance.Visibility = Visibility.Visible;
                    btnHistory.Visibility = Visibility.Visible;
                    textAbout.Visibility = Visibility.Visible;
                    textInfo.Visibility = Visibility.Visible;
                    btnExitAcc.Visibility = Visibility.Visible;
                }
            }
            else
            {
                btnLogin.Visibility = Visibility.Visible;
                textPass.Visibility = Visibility.Visible;
                btnReg.Visibility = Visibility.Visible;
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Identity = null;
            UpdateUI();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Разработчики приложения: \n Деревцов Александр \n Карбушев Владислав");
        }

        private void Balance_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new ModelRentalBicycle())
            {
                var client = db.Clients.FirstOrDefault(c => c.UserId == CurrentUser.Identity.Id);
                if (client != null)
                {
                    int balance = client.Balance;
                    MessageBox.Show($"Ваш баланс равен: {balance}");
                }

            }
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Разработчики приложения: \n Деревцов Александр \n Карбушев Владислав");
        }

        private void Login_Click(object sender, RoutedEventArgs
e)
        {
            AuthRegWindow wLogin = new AuthRegWindow();
            wLogin.Auth.SelectedIndex = 0;   
            wLogin.Show();
        }

        private void Registr_Click(object sender, RoutedEventArgs
e)
        {
            AuthRegWindow wLogin = new AuthRegWindow();
            wLogin.Auth.SelectedIndex = 1;
            wLogin.Show();
        }

    }
}
