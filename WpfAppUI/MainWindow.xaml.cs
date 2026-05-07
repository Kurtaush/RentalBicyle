using System;
using System.Collections.Generic;
using System.Globalization;
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

        public void LoadName()
        {
            if (CurrentUser.Identity == null)
            {
                return;
            }
            using (var db = new ModelRentalBicycle())
            {
                var currentClient = db.Clients.FirstOrDefault(c => c.UserId == CurrentUser.Identity.Id);
                textInfo.DataContext = currentClient;
            }
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
                btnInfo.Visibility = Visibility.Collapsed;
                btnRentals.Visibility = Visibility.Collapsed;
            btnLogin.Visibility = Visibility.Collapsed;
            btnReg.Visibility = Visibility.Collapsed;
            textPass.Visibility = Visibility.Collapsed;
            btnBalance.Visibility = Visibility.Collapsed;
            btnHistory.Visibility = Visibility.Collapsed;
            textAbout.Visibility = Visibility.Collapsed;
            textInfo.Visibility = Visibility.Collapsed;
            btnExitAcc.Visibility = Visibility.Collapsed;
            btnInfo.Visibility = Visibility.Collapsed;

            if (CurrentUser.Identity != null)
            {
                if (CurrentUser.Identity.Role == "client")
                {
                    btnBalance.Visibility = Visibility.Visible;
                    btnHistory.Visibility = Visibility.Visible;
                    textAbout.Visibility = Visibility.Visible;
                    textInfo.Visibility = Visibility.Visible;
                    btnExitAcc.Visibility = Visibility.Visible;
                    btnInfo.Visibility = Visibility.Visible;
                    LoadName();
                }
                if (CurrentUser.Identity.Role == "operator")
                {
                    btnExitAcc.Visibility = Visibility.Visible;
                    btnRentals.Visibility = Visibility.Visible;
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
                    TopUpBalanceWindow wTopUp = new TopUpBalanceWindow(client.Id);
                    wTopUp.Owner = this;
                    wTopUp.ShowDialog();

                    // Обновляем отображение баланса после закрытия окна
                    if (wTopUp.DialogResult == true)
                    {
                        LoadName(); // обновляет информацию о клиенте (баланс)
                        MessageBox.Show($"Текущий баланс: {wTopUp.NewBalance:N2} руб.",
                            "Баланс", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Клиент не найден!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow wInfo = new InfoWindow();
            wInfo.Show();
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверяем авторизацию сразу
            if (CurrentUser.Identity == null)
            {
                MessageBox.Show("Ошибка: вы не авторизованы!");
                return;
            }
            using (var db = new ModelRentalBicycle())
            {
                var client = db.Clients.FirstOrDefault(c => c.UserId == CurrentUser.Identity.Id);

                if (client != null)
                {
                    var historyWindow = new HistoryWindow(client.Id);
                    historyWindow.Owner = this;
                    historyWindow.ShowDialog();
                }
            }
        }


        private void Login_Click(object sender, RoutedEventArgs
e)
        {
            AuthRegWindow wLogin = new AuthRegWindow();
            wLogin.Auth.SelectedIndex = 0;   
            wLogin.Show();
        }

        private void Registr_Click(object sender, RoutedEventArgs e)
        {
            AuthRegWindow wLogin = new AuthRegWindow();
            wLogin.Auth.SelectedIndex = 1;
            wLogin.Show();
        }
        
        private void Client_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что пользователь авторизован
            if (CurrentUser.Identity == null)
            {
                MessageBox.Show("Необходимо авторизоваться!",
                    "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentUser.Identity.Role == "client")
            {
                using (var db = new ModelRentalBicycle())
                {
                    var client = db.Clients.FirstOrDefault(c => c.UserId == CurrentUser.Identity.Id);
                    if (client != null)
                    {
                        var activeRental = db.Rentals.FirstOrDefault(r => r.ClientId == client.Id && r.EndTime == null);
                        if (activeRental != null)
                        {
                            MessageBox.Show("У вас уже есть активная аренда. Верните велосипед перед тем, как взять новый.",
                                "Аренда уже активна", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                }

                // Если все проверки пройдены — открываем окно
                Button clickedButton = (Button)sender;
                string stationId = clickedButton.Tag.ToString();
                ClientWindow wClient = new ClientWindow(stationId);
                wClient.Show();
            }
            
            if (CurrentUser.Identity.Role == "operator")
            {
                Button clickedButton = (Button)sender;
                string stationId = clickedButton.Tag.ToString();
                OperatorWindow wOperator = new OperatorWindow(int.Parse(stationId));
                wOperator.Show();
            }
        }

        private void Rentals_Click(object sender, RoutedEventArgs e)
        {
            ActiveRentalsWindow wActiveRentals = new ActiveRentalsWindow();
            wActiveRentals.Show();
        }
    }
}
