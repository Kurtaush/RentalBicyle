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
                if (btnLogin != null) btnLogin.Visibility = Visibility.Collapsed;
                if (btnReg != null) btnReg.Visibility = Visibility.Collapsed;
                if (textPass != null) textPass.Visibility = Visibility.Collapsed;
                if (btnLogin != null) btnBalance.Visibility = Visibility.Collapsed;
                if (btnReg != null) btnHistory.Visibility = Visibility.Collapsed;
                if (btnTariff != null) btnTariff.Visibility = Visibility.Collapsed;
                if (textInfo != null) textInfo.Visibility = Visibility.Collapsed;
                if (btnExitAcc != null) btnExitAcc.Visibility = Visibility.Collapsed;
            

            if (CurrentUser.Identity != null)
            {
                if (CurrentUser.Identity.Role == "client")
                {
                    if (btnLogin != null) btnBalance.Visibility = Visibility.Visible;
                    if (btnReg != null) btnHistory.Visibility = Visibility.Visible;
                    if (btnTariff != null) btnTariff.Visibility = Visibility.Visible;
                    if (textInfo != null) textInfo.Visibility = Visibility.Visible;
                    if (btnExitAcc != null) btnExitAcc.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (btnLogin != null) btnLogin.Visibility = Visibility.Visible;
                if (textPass != null) textPass.Visibility = Visibility.Visible;
                if (btnReg != null) btnReg.Visibility = Visibility.Visible;
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Identity = null;
            UpdateUI();
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
