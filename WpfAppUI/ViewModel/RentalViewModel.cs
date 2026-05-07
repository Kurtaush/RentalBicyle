using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfAppUI.Helper;
using WpfAppUI.Model;

namespace WpfAppUI.ViewModel
{
    public class RentalViewModel : INotifyPropertyChanged
    {
        private Rental _selectedRental;
        private bool _canUserRent;
        private readonly int? _clientId;

        public Rental SelectedRental
        {
            get => _selectedRental;
            set { _selectedRental = value; OnPropertyChanged(); }
        }

        public bool CanUserRent
        {
            get => _canUserRent;
            set { _canUserRent = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Rental> ListRental { get; set; } = new ObservableCollection<Rental>();

        public RentalViewModel(int? clientId = null)
        {
            _clientId = clientId;
            Refresh();
        }

        /// <summary>
        /// Команда отмены аренды (удаление записи и освобождение велосипеда)
        /// </summary>
        private RelayCommand _cancelRental;
        public RelayCommand CancelRental
        {
            get
            {
                return _cancelRental ?? (_cancelRental = new RelayCommand(obj =>
                {
                    var result = MessageBox.Show("Вы действительно хотите отменить аренду? Запись будет удалена, а велосипед освобожден.",
                        "Подтверждение", MessageBoxButton.YesNo);

                    if (result != MessageBoxResult.Yes) return;

                    using (var db = new ModelRentalBicycle())
                    {
                        try
                        {
                            // Загружаем аренду вместе с данными велосипеда
                            var rental = db.Rentals.Include(r => r.Bicycle)
                                                   .FirstOrDefault(r => r.Id == SelectedRental.Id);

                            if (rental != null)
                            {
                                // 1. Меняем статус велосипеда на Free (или Свободен)
                                if (rental.Bicycle != null)
                                {
                                    rental.Bicycle.Status = "Free";
                                }

                                // 2. Удаляем запись об аренде
                                db.Rentals.Remove(rental);

                                db.SaveChanges();

                                // 3. Обновляем данные в интерфейсе
                                Refresh();
                                MessageBox.Show("Аренда успешно отменена!");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при выполнении операции: {ex.Message}", "Ошибка");
                        }
                    }
                }, _ => SelectedRental != null && SelectedRental.EndTime == null));
            }
        }

        /// <summary>
        /// Загрузка истории аренд
        /// </summary>
        private void LoadRentals()
        {
            using (var context = new ModelRentalBicycle())
            {
                var query = context.Rentals
                    .Include(r => r.Bicycle)
                    .Include(r => r.Tariff)
                    .AsQueryable();

                if (_clientId.HasValue)
                {
                    query = query.Where(r => r.ClientId == _clientId.Value);
                }

                var list = query.OrderByDescending(r => r.StartTime).ToList();

                ListRental.Clear();
                foreach (var r in list)
                {
                    ListRental.Add(r);
                }
            }
        }

        /// <summary>
        /// Проверка возможности новой аренды
        /// </summary>
        public void CheckCanRent()
        {
            if (!_clientId.HasValue)
            {
                CanUserRent = false;
                return;
            }

            using (var context = new ModelRentalBicycle())
            {
                // Если есть хоть одна запись без EndTime — арендовать нельзя
                bool hasActive = context.Rentals
                    .Any(r => r.ClientId == _clientId.Value && r.EndTime == null);

                CanUserRent = !hasActive;
            }
        }

        /// <summary>
        /// Полное обновление данных
        /// </summary>
        public void Refresh()
        {
            LoadRentals();
            CheckCanRent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
