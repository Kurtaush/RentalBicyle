using System.Windows;
using WpfAppUI.ViewModel;

namespace WpfAppUI
{
    public partial class HistoryWindow : Window
    {
        private readonly RentalViewModel _viewModel;

        public HistoryWindow(int userId)
        {
            InitializeComponent();
            _viewModel = new RentalViewModel(userId);
            this.DataContext = _viewModel;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
