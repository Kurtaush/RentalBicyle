using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppUI.Model
{
    /// <summary>
    /// Класс аренда
    /// </summary>
    public class Rental
    {
        /// <summary>
        /// код аренды
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// код клиента
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// код велосипеда
        /// </summary>
        public int BicycleId { get; set; }

        /// <summary>
        /// код станции начала аренды
        /// </summary>
        public int StartStationId { get; set; }

        /// <summary>
        /// время начала аренды
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// время окончания аренды
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// код станции окончания аренды
        /// </summary>
        public int? EndStationId { get; set; }

        /// <summary>
        /// стоимость аренды
        /// </summary>
        public decimal? Cost { get; set; }

        public Rental()
        {
            this.Payments = new HashSet<Payment>();
        }

        /// <summary>
        /// класс клиента для связи с сущностью Client
        /// </summary>
        public virtual Client Client { get; set; }

        /// <summary>
        /// класс велосипеда для связи с сущностью Bicycle
        /// </summary>
        public virtual Bicycle Bicycle { get; set; }

        /// <summary>
        /// класс станции начала для связи с сущностью Station
        /// </summary>
        public virtual Station StartStation { get; set; }

        /// <summary>
        /// класс станции окончания для связи с сущностью Station
        /// </summary>
        public virtual Station EndStation { get; set; }

        /// <summary>
        /// коллекция Payments для связи с классом Payment
        /// </summary>
        public virtual ICollection<Payment> Payments { get; set; }

        /// <summary>
        /// код тарифа
        /// </summary>
        public int TariffId { get; set; }

        /// <summary>
        /// класс тарифа для связи с сущностью Tariff
        /// </summary>
        public virtual Tariff Tariff { get; set; }
    }
}
