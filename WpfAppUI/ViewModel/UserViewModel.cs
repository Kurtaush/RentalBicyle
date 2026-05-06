using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using WpfAppUI.Helper;
using WpfAppUI.Model;
using WpfAppUI.ViewAdmin;

namespace WpfAppUI.ViewModel
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private User _selectedUser;
        /// <summary>
        /// выделенный в списке пользователь
        /// </summary>
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        /// <summary>
        /// коллекция пользователей
        /// </summary>
        public ObservableCollection<User> ListUser { get; set; }

        public UserViewModel()
        {
            ListUser = new ObservableCollection<User>();
            ListUser = GetUsers();
        }

        private ObservableCollection<User> GetUsers()
        {
            using (var context = new ModelRentalBicycle())
            {
                var query = from u in context.Users
                            orderby u.Id
                            select u;
                if (query.Count() != 0)
                {
                    foreach (var u in query)
                    {
                        ListUser.Add(u);
                    }
                }
            }
            return ListUser;
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

        #region AddUser
        /// <summary>
        /// добавление пользователя
        /// </summary>
        private RelayCommand _addUser;
        public RelayCommand AddUser
        {
            get
            {
                return _addUser ??
                (_addUser = new RelayCommand(obj =>
                {
                    User newUser = new User();
                    NewUserWindow wnUser = new NewUserWindow
                    {
                        Title = "Новый пользователь",
                        DataContext = newUser
                    };
                    wnUser.ShowDialog();
                    if (wnUser.DialogResult == true)
                    {
                        using (var context = new ModelRentalBicycle())
                        {
                            try
                            {
                                newUser.PasswordHash = GetHash(newUser.PasswordHash);
                                context.Users.Add(newUser);
                                context.SaveChanges();
                                ListUser.Clear();
                                ListUser = GetUsers();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("\nОшибка добавления данных!\n" + ex.Message, "Предупреждение");
                            }
                        }
                    }
                }, (obj) => true));
            }
        }
        #endregion

        #region EditUser
        /// команда редактирования пользователя
        private RelayCommand _editUser;
        public RelayCommand EditUser
        {
            get
            {
                return _editUser ??
                (_editUser = new RelayCommand(obj =>
                {
                    User editUser = SelectedUser;
                    string oldPassword = editUser.PasswordHash;
                    NewUserWindow wnUser = new NewUserWindow
                    {
                        Title = "Редактирование пользователя",
                        DataContext = editUser
                    };
                    wnUser.ShowDialog();
                    if (wnUser.DialogResult == true)
                    {
                        using (var context = new ModelRentalBicycle())
                        {
                            User user = context.Users.Find(editUser.Id);
                            if (user != null)
                            {
                                user.Login = editUser.Login.Trim();
                                if (user.PasswordHash != editUser.PasswordHash)
                                    user.PasswordHash = GetHash(editUser.PasswordHash);
                                user.Role = editUser.Role.Trim();
                                try
                                {
                                    context.SaveChanges();
                                    ListUser.Clear();
                                    ListUser = GetUsers();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка редактирования данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                    else
                    {
                        ListUser.Clear();
                        ListUser = GetUsers();
                    }
                }, (obj) => SelectedUser != null && ListUser.Count > 0));
            }
        }
        #endregion

        #region DeleteUser
        /// команда удаления пользователя
        private RelayCommand _deleteUser;
        public RelayCommand DeleteUser
        {
            get
            {
                return _deleteUser ??
                (_deleteUser = new RelayCommand(obj =>
                {
                    User delUser = SelectedUser;
                    using (var context = new ModelRentalBicycle())
                    {
                        User user = context.Users.Find(delUser.Id);
                        if (user != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить пользователя: \n" + user.Login,
                                "Предупреждение", MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Users.Remove(user);
                                    context.SaveChanges();
                                    ListUser.Remove(delUser);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedUser != null && ListUser.Count > 0));
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}