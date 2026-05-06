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

namespace WpfAppUI.ViewAdmin
{
    /// <summary>
    /// Логика взаимодействия для NewBicycleWindow.xaml
    /// </summary>
    public partial class NewBicycleWindow : Window
    {
        public NewBicycleWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbStatus.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус велосипеда!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Получаем объект Bicycle из DataContext
            if (DataContext is Model.Bicycle bicycle)
            {
                // Если велосипед в аренде, блокируем ComboBox
                if (bicycle.Status == "rented")
                {
                    cbStatus.IsEnabled = false;
                }
                // Если статус не задан (новый велосипед), ставим "free"
                else if (string.IsNullOrEmpty(bicycle.Status))
                {
                    cbStatus.SelectedIndex = 0; // free
                }
                else
                {
                    // Выбираем текущий статус в ComboBox
                    foreach (ComboBoxItem item in cbStatus.Items)
                    {
                        if (item.Content.ToString() == bicycle.Status)
                        {
                            item.IsSelected = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
