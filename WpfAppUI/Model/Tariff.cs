using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppUI.Model
{
    /// <summary>
    /// Класс тариф
    /// </summary>
    public class Tariff
    {
        /// <summary>
        /// код тарифа
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// название тарифа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// цена за час
        /// </summary>
        public decimal PricePerHour { get; set; }
    }
}
