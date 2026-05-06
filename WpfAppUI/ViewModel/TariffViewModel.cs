using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfAppUI.Helper;
using WpfAppUI.Model;
using WpfAppUI.ViewAdmin;

namespace WpfAppUI.ViewModel
{
    public class TariffViewModel : INotifyPropertyChanged
    {
        private Tariff _selectedTariff;
        /// <summary>
        /// выделенный в списке тариф
        /// </summary>
        public Tariff SelectedTariff
        {
            get { return _selectedTariff; }
            set
            {
                _selectedTariff = value;
                OnPropertyChanged("SelectedTariff");
            }
        }

        /// <summary>
        /// коллекция тарифов
        /// </summary>
        public ObservableCollection<Tariff> ListTariff { get; set; }

        public TariffViewModel()
        {
            ListTariff = new ObservableCollection<Tariff>();
            ListTariff = GetTariffs();
        }

        private ObservableCollection<Tariff> GetTariffs()
        {
            using (var context = new ModelRentalBicycle())
            {
                var query = from t in context.Tariffs
                            orderby t.Id
                            select t;
                if (query.Count() != 0)
                {
                    foreach (var t in query)
                    {
                        ListTariff.Add(t);
                    }
                }
            }
            return ListTariff;
        }

        #region AddTariff
        /// <summary>
        /// добавление тарифа
        /// </summary>
        private RelayCommand _addTariff;
        public RelayCommand AddTariff
        {
            get
            {
                return _addTariff ??
                (_addTariff = new RelayCommand(obj =>
                {
                    Tariff newTariff = new Tariff();
                    NewTariffWindow wnTariff = new NewTariffWindow
                    {
                        Title = "Новый тариф",
                        DataContext = newTariff
                    };
                    wnTariff.ShowDialog();
                    if (wnTariff.DialogResult == true)
                    {
                        using (var context = new ModelRentalBicycle())
                        {
                            try
                            {
                                context.Tariffs.Add(newTariff);
                                context.SaveChanges();
                                ListTariff.Clear();
                                ListTariff = GetTariffs();
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

        #region EditTariff
        /// команда редактирования тарифа
        private RelayCommand _editTariff;
        public RelayCommand EditTariff
        {
            get
            {
                return _editTariff ??
                (_editTariff = new RelayCommand(obj =>
                {
                    Tariff editTariff = SelectedTariff;
                    NewTariffWindow wnTariff = new NewTariffWindow
                    {
                        Title = "Редактирование тарифа",
                        DataContext = editTariff
                    };
                    wnTariff.ShowDialog();
                    if (wnTariff.DialogResult == true)
                    {
                        using (var context = new ModelRentalBicycle())
                        {
                            Tariff tariff = context.Tariffs.Find(editTariff.Id);
                            if (tariff != null)
                            {
                                tariff.Name = editTariff.Name.Trim();
                                tariff.PricePerHour = editTariff.PricePerHour;
                                try
                                {
                                    context.SaveChanges();
                                    ListTariff.Clear();
                                    ListTariff = GetTariffs();
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
                        ListTariff.Clear();
                        ListTariff = GetTariffs();
                    }
                }, (obj) => SelectedTariff != null && ListTariff.Count > 0));
            }
        }
        #endregion

        #region DeleteTariff
        /// команда удаления тарифа
        private RelayCommand _deleteTariff;
        public RelayCommand DeleteTariff
        {
            get
            {
                return _deleteTariff ??
                (_deleteTariff = new RelayCommand(obj =>
                {
                    Tariff delTariff = SelectedTariff;
                    using (var context = new ModelRentalBicycle())
                    {
                        Tariff tariff = context.Tariffs.Find(delTariff.Id);
                        if (tariff != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить тариф: \n" + tariff.Name,
                                "Предупреждение", MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Tariffs.Remove(tariff);
                                    context.SaveChanges();
                                    ListTariff.Remove(delTariff);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedTariff != null && ListTariff.Count > 0));
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