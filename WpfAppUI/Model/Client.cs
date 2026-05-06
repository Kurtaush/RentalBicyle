using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppUI.Model
{
    /// <summary>
    /// Класс клиент
    /// </summary>
    public class Client
    {
        /// <summary>
        /// код клиента
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// код пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// полное имя клиента
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// телефон клиента
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// баланс клиента
        /// </summary>
        public decimal Balance { get; set; }

        public Client()
        {
            this.Rentals = new HashSet<Rental>();
        }

        /// <summary>
        /// класс пользователя для связи с сущностью User
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// коллекция Rentals для связи с классом Rental
        /// </summary>
        public virtual ICollection<Rental> Rentals { get; set; }
    }
}
