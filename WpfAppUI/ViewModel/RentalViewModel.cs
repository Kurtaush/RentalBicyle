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
        /// Команда отмены аренды (завершение аренды, освобождение велосипеда и списание средств)
        /// </summary>
        private RelayCommand _cancelRental;
        public RelayCommand CancelRental
        {
            get
            {
                return _cancelRental ?? (_cancelRental = new RelayCommand(obj =>
                {
                    var result = MessageBox.Show("Вы действительно хотите завершить аренду?",
                        "Подтверждение", MessageBoxButton.YesNo);

                    if (result != MessageBoxResult.Yes) return;

                    using (var db = new ModelRentalBicycle())
                    {
                        try
                        {
                            // Загружаем аренду вместе с данными велосипеда, тарифа, клиента и станций
                            var rental = db.Rentals
                                .Include(r => r.Bicycle)
                                .Include(r => r.Tariff)
                                .Include(r => r.Client)
                                .FirstOrDefault(r => r.Id == SelectedRental.Id);

                            if (rental != null)
                            {
                                // 1. Устанавливаем время окончания
                                rental.EndTime = DateTime.Now;

                                // 2. Рассчитываем стоимость по тарифу
                                if (rental.Tariff != null && rental.StartTime != null)
                                {
                                    var hours = Math.Ceiling((rental.EndTime.Value - rental.StartTime).TotalHours);
                                    rental.Amount = (decimal)hours * rental.Tariff.PricePerHour;
                                }

                                // 3. Устанавливаем станцию возврата (ту же, где начали)
                                rental.EndStationId = rental.StartStationId;

                                // 4. Списываем средства с баланса клиента
                                if (rental.Client != null && rental.Amount.HasValue)
                                {
                                    var client = rental.Client;
                                    decimal totalAmount = rental.Amount.Value;

                                    if (client.Balance >= totalAmount)
                                    {
                                        // Полная оплата — снимаем всю сумму
                                        client.Balance -= totalAmount;
                                        rental.PaidAt = DateTime.Now;
                                    }
                                    else
                                    {
                                        // Частичная оплата — снимаем остаток баланса
                                        decimal availableBalance = client.Balance;
                                        client.Balance = 0;
                                        rental.PaidAt = null; // аренда остаётся неоплаченной

                                        MessageBox.Show(
                                            $"Недостаточно средств на балансе!\n" +
                                            $"Сумма аренды: {totalAmount:N2} руб.\n" +
                                            $"Списано: {availableBalance:N2} руб.\n" +
                                            $"Остаток долга: {(totalAmount - availableBalance):N2} руб.\n" +
                                            $"Аренда завершена, но осталась неоплаченной.",
                                            "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    }
                                }
                                    
                                // 5. Меняем статус велосипеда на free и возвращаем на станцию
                                if (rental.Bicycle != null)
                                {
                                    rental.Bicycle.Status = "free";
                                    rental.Bicycle.StationId = rental.EndStationId;
                                }

                                db.SaveChanges();

                                // 6. Обновляем данные в интерфейсе
                                Refresh();

                                if (rental.PaidAt != null)
                                {
                                    MessageBox.Show($"Аренда завершена!\nСписано с баланса: {rental.Amount:N2} руб.",
                                        "Операция выполнена", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
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

                // Сортировка по ID по возрастанию
                var list = query.OrderBy(r => r.Id).ToList();

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
