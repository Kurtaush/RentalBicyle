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
    public class StationViewModel : INotifyPropertyChanged
    {
        private Station _selectedStation;
        /// <summary>
        /// выделенная в списке станция
        /// </summary>
        public Station SelectedStation
        {
            get { return _selectedStation; }
            set
            {
                _selectedStation = value;
                OnPropertyChanged("SelectedStation");
            }
        }

        /// <summary>
        /// коллекция станций
        /// </summary>
        public ObservableCollection<Station> ListStation { get; set; }

        public StationViewModel()
        {
            ListStation = new ObservableCollection<Station>();
            ListStation = GetStations();
        }

        private ObservableCollection<Station> GetStations()
        {
            using (var context = new ModelRentalBicycle())
            {
                var query = from s in context.Stations
                            orderby s.Id
                            select s;
                if (query.Count() != 0)
                {
                    foreach (var s in query)
                    {
                        ListStation.Add(s);
                    }
                }
            }
            return ListStation;
        }

        #region AddStation
        /// <summary>
        /// добавление станции
        /// </summary>
        private RelayCommand _addStation;
        public RelayCommand AddStation
        {
            get
            {
                return _addStation ??
                (_addStation = new RelayCommand(obj =>
                {
                    Station newStation = new Station();
                    NewStationWindow wnStation = new NewStationWindow
                    {
                        Title = "Новая станция",
                        DataContext = newStation
                    };
                    wnStation.ShowDialog();
                    if (wnStation.DialogResult == true)
                    {
                        using (var context = new ModelRentalBicycle())
                        {
                            try
                            {
                                context.Stations.Add(newStation);
                                context.SaveChanges();
                                ListStation.Clear();
                                ListStation = GetStations();
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

        #region EditStation
        /// команда редактирования станции
        private RelayCommand _editStation;
        public RelayCommand EditStation
        {
            get
            {
                return _editStation ??
                (_editStation = new RelayCommand(obj =>
                {
                    Station editStation = SelectedStation;
                    NewStationWindow wnStation = new NewStationWindow
                    {
                        Title = "Редактирование станции",
                        DataContext = editStation
                    };
                    wnStation.ShowDialog();
                    if (wnStation.DialogResult == true)
                    {
                        using (var context = new ModelRentalBicycle())
                        {
                            Station station = context.Stations.Find(editStation.Id);
                            if (station != null)
                            {
                                station.Name = editStation.Name.Trim();
                                station.Address = editStation.Address.Trim();
                                try
                                {
                                    context.SaveChanges();
                                    ListStation.Clear();
                                    ListStation = GetStations();
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
                        ListStation.Clear();
                        ListStation = GetStations();
                    }
                }, (obj) => SelectedStation != null && ListStation.Count > 0));
            }
        }
        #endregion

        #region DeleteStation
        /// команда удаления станции
        private RelayCommand _deleteStation;
        public RelayCommand DeleteStation
        {
            get
            {
                return _deleteStation ??
                (_deleteStation = new RelayCommand(obj =>
                {
                    Station delStation = SelectedStation;
                    using (var context = new ModelRentalBicycle())
                    {
                        Station station = context.Stations.Find(delStation.Id);
                        if (station != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить станцию: \n" + station.Name,
                                "Предупреждение", MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Stations.Remove(station);
                                    context.SaveChanges();
                                    ListStation.Remove(delStation);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedStation != null && ListStation.Count > 0));
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