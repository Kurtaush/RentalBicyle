using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfAppUI.Helper;
using WpfAppUI.Model;
using WpfAppUI.ViewAdmin;

namespace WpfAppUI.ViewModel
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        private Client _selectedClient;
        /// <summary>
        /// выделенный в списке клиент
        /// </summary>
        public Client SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                OnPropertyChanged("SelectedClient");
            }
        }

        /// <summary>
        /// коллекция клиентов
        /// </summary>
        public ObservableCollection<Client> ListClient { get; set; }

        public ClientViewModel()
        {
            ListClient = new ObservableCollection<Client>();
            ListClient = GetClients();
        }

        private ObservableCollection<Client> GetClients()
        {
            using (var context = new ModelRentalBicycle())
            {
                var query = from c in context.Clients
                            .Include("User")
                            orderby c.Id
                            select c;
                if (query.Count() != 0)
                {
                    foreach (var c in query)
                    {
                        ListClient.Add(c);
                    }
                }
            }
            return ListClient;
        }

        #region EditClient
        /// команда редактирования клиента
        private RelayCommand _editClient;
        public RelayCommand EditClient
        {
            get
            {
                return _editClient ??
                (_editClient = new RelayCommand(obj =>
                {
                    Client editClient = SelectedClient;
                    NewClientWindow wnClient = new NewClientWindow
                    {
                        Title = "Редактирование клиента",
                        DataContext = editClient
                    };
                    wnClient.ShowDialog();
                    if (wnClient.DialogResult == true)
                    {
                        using (var context = new ModelRentalBicycle())
                        {
                            Client client = context.Clients.Find(editClient.Id);
                            if (client != null)
                            {
                                client.FullName = editClient.FullName;
                                client.Phone = editClient.Phone;
                                client.Balance = editClient.Balance;
                                try
                                {
                                    context.SaveChanges();
                                    ListClient.Clear();
                                    ListClient = GetClients();
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
                        ListClient.Clear();
                        ListClient = GetClients();
                    }
                }, (obj) => SelectedClient != null && ListClient.Count > 0));
            }
        }
        #endregion

        #region DeleteClient
        /// команда удаления клиента
        private RelayCommand _deleteClient;
        public RelayCommand DeleteClient
        {
            get
            {
                return _deleteClient ??
                (_deleteClient = new RelayCommand(obj =>
                {
                    Client delClient = SelectedClient;
                    using (var context = new ModelRentalBicycle())
                    {
                        Client client = context.Clients.Find(delClient.Id);
                        if (client != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить клиента: \n" + client.FullName,
                                "Предупреждение", MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Clients.Remove(client);
                                    context.SaveChanges();
                                    ListClient.Remove(delClient);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedClient != null && ListClient.Count > 0));
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