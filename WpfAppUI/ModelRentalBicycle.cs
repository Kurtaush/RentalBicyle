    using System;
    using System.Data.Entity;
    using System.Linq;
    using WpfAppUI.Model;
    using System.Text;
    using System.Security.Cryptography;
    using WpfAppUI.View;

    namespace WpfAppUI
    {
        public class ModelRentalBicycle : DbContext
        {
            // Контекст настроен для использования строки подключения "ModelRentalBicycle" из файла конфигурации  
            // приложения (App.config или Web.config). По умолчанию эта строка подключения указывает на базу данных 
            // "WpfAppUI.ModelRentalBicycle" в экземпляре LocalDb. 
            // 
            // Если требуется выбрать другую базу данных или поставщик базы данных, измените строку подключения "ModelRentalBicycle" 
            // в файле конфигурации приложения.
            public ModelRentalBicycle()
                : base("name=ModelRentalBicycle")
            {
            if (!this.Users.Any(u => u.Role == "admin"))
            {
                this.Users.Add(new User
                {
                    Login = "admin",
                    PasswordHash = GetHash("admin123"),
                    Role = "admin"
                });
                this.SaveChanges();
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

        // Добавьте DbSet для каждого типа сущности, который требуется включить в модель. Дополнительные сведения 
        // о настройке и использовании модели Code First см. в статье http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Bicycle> Bicycles { get; set; }
            public virtual DbSet<Client> Clients { get; set; }
            public virtual DbSet<Payment> Payments { get; set; }
            public virtual DbSet<Rental> Rentals { get; set; }
            public virtual DbSet<Station> Stations { get; set; }
            public virtual DbSet<Tariff> Tariffs { get; set; }
            public virtual DbSet<User> Users { get; set; }
        }

        //public class MyEntity
        //{
        //    public int Id { get; set; }
        //    public string Name { get; set; }
        //}
    }