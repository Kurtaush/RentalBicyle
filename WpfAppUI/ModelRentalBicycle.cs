using System;
using System.Data.Entity;
using System.Linq;
using WpfAppUI.Model;
using System.Text;
using System.Security.Cryptography;

namespace WpfAppUI
{
    public class ModelRentalBicycle : DbContext
    {
        public ModelRentalBicycle()
            : base("name=ModelRentalBicycle")
        {
            // База создаётся однократно и больше не удаляется
            Database.SetInitializer(new CreateDatabaseIfNotExists<ModelRentalBicycle>());

            // Заполнение начальными данными, если ещё нет администратора
            if (!this.Users.Any(u => u.Role == "admin"))
            {
                SeedData();
            }
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

        private void SeedData()
        {
            // ========== ТАРИФЫ (3 тарифа) ==========
            var tariff1 = new Tariff { Name = "Стандарт", PricePerHour = 100 };
            var tariff2 = new Tariff { Name = "Премиум", PricePerHour = 200 };
            var tariff3 = new Tariff { Name = "Спорт", PricePerHour = 300 };
            this.Tariffs.AddRange(new[] { tariff1, tariff2, tariff3 });
            this.SaveChanges();

            // ========== СТАНЦИИ (6 станций) ==========
            var stations = new[]
            {
                new Station { Name = "Центральная", Address = "ул. Ленина, 1" },
                new Station { Name = "Вокзал", Address = "Привокзальная пл., 2" },
                new Station { Name = "Парк Горького", Address = "ул. Парковая, 15" },
                new Station { Name = "Университет", Address = "пр. Студенческий, 10" },
                new Station { Name = "ТЦ Мега", Address = "ш. Космонавтов, 45" },
                new Station { Name = "Стадион", Address = "ул. Спортивная, 8" }
            };
            this.Stations.AddRange(stations);
            this.SaveChanges();

            // ========== ВЕЛОСИПЕДЫ (15 штук) ==========
            var bicycleModels = new[] { "Stels", "Forward", "Trek", "Giant", "Merida", "Cube", "Author", "GT", "Scott", "Specialized" };
            var rnd = new Random();

            for (int i = 0; i < 15; i++)
            {
                var model = bicycleModels[rnd.Next(bicycleModels.Length)];
                var stationIndex = rnd.Next(6);
                var statuses = new[] { "free", "free", "free", "free", "rented", "maintenance" };
                var status = statuses[rnd.Next(statuses.Length)];

                var bicycle = new Bicycle
                {
                    Model = $"{model} {rnd.Next(100, 999)}",
                    Status = status,
                    StationId = (status == "free" || status == "maintenance") ? stations[stationIndex].Id : (int?)null,
                    TariffId = (status == "free" || status == "rented")
                        ? new[] { tariff1.Id, tariff2.Id, tariff3.Id }[rnd.Next(3)]
                        : (int?)null
                };
                this.Bicycles.Add(bicycle);

                if (status == "rented")
                {
                    bicycle.StationId = stations[stationIndex].Id;
                }
            }
            this.SaveChanges();

            // ========== ПОЛЬЗОВАТЕЛИ И КЛИЕНТЫ (10 клиентов) ==========
            var adminUser = new User
            {
                Login = "admin",
                PasswordHash = GetHash("admin123"),
                Role = "admin"
            };
            this.Users.Add(adminUser);

            var operator1 = new User
            {
                Login = "operator1",
                PasswordHash = GetHash("oper123"),
                Role = "operator"
            };
            var operator2 = new User
            {
                Login = "operator2",
                PasswordHash = GetHash("oper123"),
                Role = "operator"
            };
            this.Users.AddRange(new[] { operator1, operator2 });

            var clientNames = new[]
            {
                "Иванов Иван Иванович",
                "Петров Петр Петрович",
                "Сидорова Анна Михайловна",
                "Козлов Дмитрий Сергеевич",
                "Морозова Елена Викторовна",
                "Волков Алексей Николаевич",
                "Соколова Мария Андреевна",
                "Лебедев Сергей Павлович",
                "Зайцева Ольга Игоревна",
                "Медведев Артем Денисович"
            };
            var phones = new[]
            {
                "+7(900)111-22-33", "+7(900)222-33-44", "+7(900)333-44-55",
                "+7(900)444-55-66", "+7(900)555-66-77", "+7(900)666-77-88",
                "+7(900)777-88-99", "+7(900)888-99-00", "+7(900)999-00-11",
                "+7(900)000-11-22"
            };

            for (int i = 0; i < 10; i++)
            {
                var clientUser = new User
                {
                    Login = $"client{i + 1}",
                    PasswordHash = GetHash("client123"),
                    Role = "client"
                };
                this.Users.Add(clientUser);
                this.SaveChanges();

                var client = new Client
                {
                    UserId = clientUser.Id,
                    FullName = clientNames[i],
                    Phone = phones[i],
                    Balance = rnd.Next(0, 5000)
                };
                this.Clients.Add(client);
            }
            this.SaveChanges();

            // ========== АРЕНДЫ (20 записей) ==========
            var clientsList = this.Clients.ToList();
            var freeBicycles = this.Bicycles.Where(b => b.Status == "free").ToList();
            var rentedBicycles = this.Bicycles.Where(b => b.Status == "rented").ToList();

            for (int i = 0; i < 20; i++)
            {
                var client = clientsList[rnd.Next(clientsList.Count)];
                var startStation = stations[rnd.Next(stations.Length)];
                var endStation = stations[rnd.Next(stations.Length)];

                Bicycle bicycle;
                if (i < 5)
                {
                    bicycle = rentedBicycles[rnd.Next(rentedBicycles.Count)];
                }
                else
                {
                    bicycle = freeBicycles[rnd.Next(freeBicycles.Count)];
                }

                var startTime = DateTime.Now.AddDays(-rnd.Next(1, 7)).AddHours(-rnd.Next(0, 24));
                var tariff = this.Tariffs.Find(bicycle.TariffId) ?? tariff1;

                var rental = new Rental
                {
                    ClientId = client.Id,
                    BicycleId = bicycle.Id,
                    StartStationId = startStation.Id,
                    StartTime = startTime,
                    TariffId = tariff.Id
                };

                if (i >= 5)
                {
                    var endTime = startTime.AddHours(rnd.Next(1, 72));
                    var hours = Math.Ceiling((endTime - startTime).TotalHours);
                    rental.EndTime = endTime;
                    rental.EndStationId = endStation.Id;
                    rental.Amount = (decimal)hours * tariff.PricePerHour;
                    rental.PaidAt = endTime.AddMinutes(rnd.Next(5, 60));
                }

                this.Rentals.Add(rental);
            }

            this.SaveChanges();
        }

        public virtual DbSet<Bicycle> Bicycles { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Rental> Rentals { get; set; }
        public virtual DbSet<Station> Stations { get; set; }
        public virtual DbSet<Tariff> Tariffs { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}