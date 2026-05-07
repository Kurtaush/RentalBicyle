using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using WpfAppUI.Model;
using static WpfAppUI.App;

namespace WpfAppUI.View
{
    /// <summary>
    /// Логика взаимодействия для AuthRegWindow.xaml
    /// </summary>
    public partial class AuthRegWindow : Window
    {
        public AuthRegWindow()
        {
            InitializeComponent();
        }

        private string GetHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginText.Text.Trim();
            string pass = PassText.Password.Trim();
            string hashedPass = GetHash(pass);

            using (var db = new ModelRentalBicycle())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == login && u.PasswordHash == hashedPass);
                if (user != null)
                {
                    CurrentUser.Identity = user;
                    ((MainWindow)Application.Current.MainWindow).UpdateUI();
                    this.Close();

                    if (user.Role == "admin")
                    {
                        AdminWindow wAdm = new AdminWindow();
                        wAdm.Closed += (sender1, args) =>
                        {
                            Application.Current.MainWindow.Show();
                            CurrentUser.Identity = null;
                            ((MainWindow)Application.Current.MainWindow).UpdateUI();
                        };
                        wAdm.Show();
                        Application.Current.MainWindow.Hide();
                    }
                }
                else MessageBox.Show("Неверный логин или пароль");
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (RegPassText.Password != RegPassRepeatText.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            using (var db = new ModelRentalBicycle())
            {
                if (db.Users.Any(u => u.Login == RegLoginText.Text))
                {
                    MessageBox.Show("Логин занят");
                    return;
                }

                db.Users.Add(new User
                {
                    Login = RegLoginText.Text,
                    PasswordHash = GetHash(RegPassText.Password),
                    Role = "client"
                });
                db.SaveChanges();
                MessageBox.Show("Успех! Теперь войдите.");
                Auth.SelectedIndex = 0;
            }
        }
    }
}
