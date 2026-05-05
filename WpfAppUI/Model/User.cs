using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppUI.Model
{
    /// <summary>
    /// Класс пользователь системы
    /// </summary>
    public class User
    {
        /// <summary>
        /// код пользователя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// логин пользователя
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// хеш пароля
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// роль пользователя (admin, operator, client)
        /// </summary>
        public string Role { get; set; }

        public User()
        {
            this.Clients = new HashSet<Client>();
        }

        /// <summary>
        /// коллекция Clients для связи с классом Client
        /// </summary>
        public virtual ICollection<Client> Clients { get; set; }
    }
}
