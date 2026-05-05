using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppUI.Model
{
    /// <summary>
    /// Класс велосипед
    /// </summary>
    public class Bicycle
    {
        /// <summary>
        /// код велосипеда
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// модель велосипеда
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// статус велосипеда (free, rented, maintenance)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// код станции
        /// </summary>
        public int? StationId { get; set; }

        public Bicycle()
        {
            this.Rentals = new HashSet<Rental>();
        }

        /// <summary>
        /// класс станции для связи с сущностью Station
        /// </summary>
        public virtual Station Station { get; set; }

        /// <summary>
        /// коллекция Rentals для связи с классом Rental
        /// </summary>
        public virtual ICollection<Rental> Rentals { get; set; }
    }
}
