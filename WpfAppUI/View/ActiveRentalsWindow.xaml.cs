using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using WpfAppUI.Helper;
using WpfAppUI.Model;

namespace WpfAppUI.View
{
    /// <summary>
    /// Логика взаимодействия для ActiveRentalsWindow.xaml
    /// </summary>
    public partial class ActiveRentalsWindow : Window
    {
        public ActiveRentalsWindow()
        {
            InitializeComponent();
            DataContext = new ActiveRentalsViewModel();
        }
    }

    public class ActiveRentalsViewModel : INotifyPropertyChanged
    {
        private Rental _selectedRental;
        private Random _rnd = new Random();

        public Rental SelectedRental
        {
            get { return _selectedRental; }
            set
            {
                _selectedRental = value;
                OnPropertyChanged("SelectedRental");
            }
        }

        public ObservableCollection<Rental> ActiveRentals { get; set; }

        public ActiveRentalsViewModel()
        {
            ActiveRentals = new ObservableCollection<Rental>();
            LoadActiveRentals();
        }

        private void LoadActiveRentals()
        {
            ActiveRentals.Clear();
            using (var db = new ModelRentalBicycle())
            {
                var query = db.Rentals
                    .Include("Client")
                    .Include("Bicycle")
                    .Include("StartStation")
                    .Include("Tariff")
                    .Where(r => r.EndTime == null)
                    .OrderBy(r => r.StartTime)
                    .ToList();

                foreach (var rental in query)
                {
                    ActiveRentals.Add(rental);
                }
            }
        }

        #region EmergencyCloseCommand
        private ICommand _emergencyCloseCommand;
        public ICommand EmergencyCloseCommand
        {
            get
            {
                return _emergencyCloseCommand ??
                    (_emergencyCloseCommand = new RelayCommand(obj =>
                    {
                        if (SelectedRental == null)
                        {
                            MessageBox.Show("Выберите аренду для завершения!", "Предупреждение");
                            return;
                        }

                        MessageBoxResult result = MessageBox.Show(
                            $"Вы действительно хотите экстренно завершить аренду №{SelectedRental.Id}?\n" +
                            $"Клиент: {SelectedRental.Client.FullName}\n" +
                            $"Телефон: {SelectedRental.Client.Phone}",
                            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            using (var db = new ModelRentalBicycle())
                            {
                                var rental = db.Rentals
                                    .Include("Tariff")
                                    .FirstOrDefault(r => r.Id == SelectedRental.Id);

                                if (rental != null)
                                {
                                    // Случайная станция завершения
                                    var stations = db.Stations.ToList();
                                    var endStation = stations[_rnd.Next(stations.Count)];

                                    // Текущее время как время завершения
                                    rental.EndTime = DateTime.Now;
                                    rental.EndStationId = endStation.Id;

                                    // Расчёт стоимости
                                    var hours = Math.Ceiling((rental.EndTime.Value - rental.StartTime).TotalHours);
                                    var tariff = db.Tariffs.Find(rental.TariffId);
                                    if (tariff != null)
                                    {
                                        rental.Amount = (decimal)hours * tariff.PricePerHour;
                                    }

                                    // Оплата сейчас же
                                    rental.PaidAt = DateTime.Now;

                                    // Возвращаем велосипед на станцию
                                    var bicycle = db.Bicycles.Find(rental.BicycleId);
                                    if (bicycle != null)
                                    {
                                        bicycle.Status = "free";
                                        bicycle.StationId = endStation.Id;
                                    }

                                    db.SaveChanges();
                                    LoadActiveRentals();

                                    MessageBox.Show($"Аренда №{rental.Id} экстренно завершена.\n" +
                                        $"Сумма к оплате: {rental.Amount:N2} руб.\n" +
                                        $"Велосипед на станции: {endStation.Name}",
                                        "Операция выполнена", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                    }, obj => SelectedRental != null));
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
