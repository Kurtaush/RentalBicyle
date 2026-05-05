using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppUI.Model
{
    /// <summary>
    /// Класс платёж
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// код платежа
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// код аренды
        /// </summary>
        public int RentalId { get; set; }

        /// <summary>
        /// сумма платежа
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// время оплаты
        /// </summary>
        public DateTime PaidAt { get; set; }

        /// <summary>
        /// класс аренды для связи с сущностью Rental
        /// </summary>
        public virtual Rental Rental { get; set; }
    }
}
