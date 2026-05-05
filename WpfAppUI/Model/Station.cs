using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppUI.Model
{
    /// <summary>
    /// Класс станция проката
    /// </summary>
    public class Station
    {
        /// <summary>
        /// код станции
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// название станции
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// адрес станции
        /// </summary>
        public string Address { get; set; }

        public Station()
        {
            this.Bicycles = new HashSet<Bicycle>();
            this.StartRentals = new HashSet<Rental>();
            this.EndRentals = new HashSet<Rental>();
        }

        /// <summary>
        /// коллекция Bicycles для связи с классом Bicycle
        /// </summary>
        public virtual ICollection<Bicycle> Bicycles { get; set; }

        /// <summary>
        /// коллекция Rentals для связи с началом аренды
        /// </summary>
        public virtual ICollection<Rental> StartRentals { get; set; }

        /// <summary>
        /// коллекция Rentals для связи с окончанием аренды
        /// </summary>
        public virtual ICollection<Rental> EndRentals { get; set; }
    }
}
