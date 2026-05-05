using System;
using System.Data.Entity;
using System.Linq;
using WpfAppUI.Model;

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