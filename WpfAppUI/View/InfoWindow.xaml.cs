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
using WpfAppUI.Model;
using static WpfAppUI.App;

namespace WpfAppUI.View
{
    /// <summary>
    /// Логика взаимодействия для InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
            LoadInfo();
        }


        private void SaveInfo_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new ModelRentalBicycle())
            {
                var currentClient = db.Clients.FirstOrDefault(u => u.UserId == CurrentUser.Identity.Id);
                if (currentClient != null) 
                {
                    currentClient.FullName = NameText.Text;
                    currentClient.Phone = PhoneText.Text;
                    db.SaveChanges();
                    ((MainWindow)System.Windows.Application.Current.MainWindow).LoadName();
                }
                else
                {
                    db.Clients.Add(new Client
                    {
                        FullName = NameText.Text,
                        Phone = PhoneText.Text,
                        UserId = CurrentUser.Identity.Id,
                        Balance = 1000
                    });
                    db.SaveChanges();
                    ((MainWindow)System.Windows.Application.Current.MainWindow).LoadName();
                } 
            }
            this.Close();
        }

        private void LoadInfo()
        {
            using (var db = new ModelRentalBicycle())
            {
                var newInfo = db.Clients.FirstOrDefault(u => u.UserId == CurrentUser.Identity.Id);
                if (newInfo != null)
                {
                    NameText.Text = newInfo.FullName;
                    PhoneText.Text = newInfo.Phone;
                }
            }
        }
    }
}
