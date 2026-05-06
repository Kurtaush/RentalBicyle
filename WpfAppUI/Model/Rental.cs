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
        /// </summary>ф
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
        /// код тарифа
        /// </summary>
        public int TariffId { get; set; }

        /// <summary>
        /// сумма аренды
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// время оплаты
        /// </summary>
        public DateTime? PaidAt { get; set; }

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
        /// класс тарифа для связи с сущностью Tariff
        /// </summary>
        public virtual Tariff Tariff { get; set; }
    }
}