using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfAppUI.Model;
using WpfAppUI.ViewModel;

namespace WpfAppUI
{
    public partial class ClientWindow : Window
    {
        public int StationId { get; private set; }
        private BicycleViewModel _bikeVm;
        private RentalViewModel _rentalVm;

        public ClientWindow(string stationId)
        {
            InitializeComponent();
            LoadStation(stationId);

            if (!int.TryParse(stationId, out int id))
            {
                MessageBox.Show("Неверный номер станции");
                Close();
                return;
            }
            StationId = id;

            // 🔹 Явно берём Id текущего пользователя
            int? userId = App.CurrentUser.Identity?.Id;

            _bikeVm = new BicycleViewModel(StationId);
            _rentalVm = new RentalViewModel(userId);

            DataContext = _bikeVm;
            UpdateRentButton();
        }

        private void LoadStation(string id) => Title = $"Станция №{id}";

        /// <summary>
        /// Управляет доступностью кнопки "Арендовать"
        /// </summary>
        private void UpdateRentButton()
        {
            if (FindName("btnRent") is Button btn)
            {
                btn.IsEnabled = _rentalVm?.CanUserRent == true;
                btn.ToolTip = btn.IsEnabled
                    ? "Арендовать выбранный велосипед"
                    : "У вас уже есть активная аренда";
            }
        }

        private void Rental_Click(object sender, RoutedEventArgs e)
        {

            var user = App.CurrentUser.Identity;
            if (user == null)
            {
                MessageBox.Show("Ошибка: вы не авторизованы!");
                return;
            }

            using (var db = new ModelRentalBicycle())
            {
                // 1. Ищем профиль клиента
                var client = db.Clients.FirstOrDefault(c => c.UserId == user.Id);
                if (client == null)
                {
                    MessageBox.Show("Ошибка: профиль клиента не найден!");
                    return;
                }

                // 2. ЖЕСТКАЯ ПРОВЕРКА: только 1 аренда на человека
                bool hasActive = db.Rentals.Any(r => r.ClientId == client.Id && r.EndTime == null);
                if (hasActive)
                {
                    MessageBox.Show("У вас уже есть активная аренда! Сначала верните велосипед.", "Внимание", MessageBoxButton.OK);
                    return;
                }

                // 3. Проверка выбора велосипеда
                var selectedBike = _bikeVm?.SelectedBicycle;
                if (selectedBike == null)
                {
                    MessageBox.Show("Выберите велосипед из списка!");
                    return;
                }

                var bikeInDb = db.Bicycles.Find(selectedBike.Id);

                if (bikeInDb.Status == "rented")
                {
                    MessageBox.Show("Данные велик уже Арендован, выберите другой!");
                    return;
                }

                // 4. Создание аренды
                var rental = new Rental
                {
                    ClientId = client.Id,
                    BicycleId = selectedBike.Id,
                    StartStationId = StationId, // Берется из текущего контекста страницы
                    StartTime = DateTime.Now,
                    TariffId = selectedBike.TariffId ?? 1 // Ставим ID тарифа (или дефолтный 1)
                };

                db.Rentals.Add(rental);

                // 5. Меняем статус велосипеда в БД


                if (bikeInDb != null)
                {
                    bikeInDb.Status = "rented";
                }


                db.SaveChanges();

                // ТО САМОЕ СООБЩЕНИЕ
                MessageBox.Show($"Велосипед {selectedBike.Model} успешно арендован!", "Успех", MessageBoxButton.OK);
            }

            // 6. Обновляем интерфейс
            _bikeVm.Refresh();     // Чтобы велик исчез из списка доступных
            _rentalVm?.Refresh();   // Чтобы заблокировалась кнопка и обновилась история
        }

    }
}