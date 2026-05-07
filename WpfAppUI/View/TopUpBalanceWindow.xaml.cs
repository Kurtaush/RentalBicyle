using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using WpfAppUI.Model;

namespace WpfAppUI.View
{
    /// <summary>
    /// Логика взаимодействия для TopUpBalanceWindow.xaml
    /// </summary>
    public partial class TopUpBalanceWindow : Window
    {
        private readonly int _clientId;
        private readonly decimal _currentBalance;

        public decimal NewBalance { get; private set; }

        public TopUpBalanceWindow(int clientId)
        {
            InitializeComponent();

            _clientId = clientId;

            // Загружаем текущий баланс
            using (var db = new ModelRentalBicycle())
            {
                var client = db.Clients.Find(_clientId);
                if (client != null)
                {
                    _currentBalance = client.Balance;
                    tbCurrentBalance.Text = $"{_currentBalance:N2} руб.";
                }
            }

            tbAmount.Focus();
        }

        private void BtnTopUp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbAmount.Text))
            {
                MessageBox.Show("Введите сумму пополнения!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbAmount.Focus();
                return;
            }

            if (!decimal.TryParse(tbAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную положительную сумму!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                tbAmount.Focus();
                tbAmount.SelectAll();
                return;
            }

            // Обновляем баланс в базе данных
            using (var db = new ModelRentalBicycle())
            {
                var client = db.Clients.Find(_clientId);
                if (client != null)
                {
                    client.Balance += amount;
                    NewBalance = client.Balance;
                    db.SaveChanges();

                    MessageBox.Show($"Баланс успешно пополнен на {amount:N2} руб.\n" +
                                    $"Текущий баланс: {NewBalance:N2} руб.",
                                    "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Клиент не найден!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Разрешаем ввод только цифр, запятой и точки
        private void TbAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}