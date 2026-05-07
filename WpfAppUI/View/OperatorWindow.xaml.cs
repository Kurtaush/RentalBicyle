using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAppUI.Helper;
using WpfAppUI.Model;

namespace WpfAppUI.View
{
    /// <summary>
    /// Логика взаимодействия для OperatorWindow.xaml
    /// </summary>
    public partial class OperatorWindow : Window
    {
        private readonly int _stationId;

        public OperatorWindow(int stationId)
        {
            InitializeComponent();
            _stationId = stationId;
            DataContext = new OperatorViewModel(stationId);
            LoadStationName();
        }

        private void LoadStationName()
        {
            using (var db = new ModelRentalBicycle())
            {
                var station = db.Stations.Find(_stationId);
                if (station != null)
                {
                    lblStationName.Content = $"Станция: {station.Name}";
                }
            }
        }

    }

    public class OperatorViewModel : INotifyPropertyChanged
    {
        private readonly int _stationId;
        private Bicycle _selectedBicycle;

        public Bicycle SelectedBicycle
        {
            get { return _selectedBicycle; }
            set
            {
                _selectedBicycle = value;
                OnPropertyChanged("SelectedBicycle");
            }
        }

        public ObservableCollection<Bicycle> Bicycles { get; set; }

        public OperatorViewModel(int stationId)
        {
            _stationId = stationId;
            Bicycles = new ObservableCollection<Bicycle>();
            LoadBicycles();
        }

        private void LoadBicycles()
        {
            Bicycles.Clear();
            using (var db = new ModelRentalBicycle())
            {
                var query = db.Bicycles
                    .Include("Tariff")
                    .Where(b => b.StationId == _stationId && b.Status != "rented")
                    .OrderBy(b => b.Id)
                    .ToList();

                foreach (var bike in query)
                {
                    Bicycles.Add(bike);
                }
            }
        }

        #region SetMaintenanceCommand
        private ICommand _setMaintenanceCommand;
        public ICommand SetMaintenanceCommand
        {
            get
            {
                return _setMaintenanceCommand ??
                    (_setMaintenanceCommand = new RelayCommand(obj =>
                    {
                        if (SelectedBicycle == null)
                        {
                            MessageBox.Show("Выберите велосипед!", "Предупреждение");
                            return;
                        }

                        if (SelectedBicycle.Status == "rented")
                        {
                            MessageBox.Show("Нельзя перевести арендованный велосипед на обслуживание!", "Ошибка");
                            return;
                        }

                        using (var db = new ModelRentalBicycle())
                        {
                            var bike = db.Bicycles.Find(SelectedBicycle.Id);
                            if (bike != null)
                            {
                                bike.Status = "maintenance";
                                db.SaveChanges();
                                LoadBicycles();
                            }
                        }
                    }, obj => SelectedBicycle != null));
            }
        }
        #endregion

        #region SetFreeCommand
        private ICommand _setFreeCommand;
        public ICommand SetFreeCommand
        {
            get
            {
                return _setFreeCommand ??
                    (_setFreeCommand = new RelayCommand(obj =>
                    {
                        if (SelectedBicycle == null)
                        {
                            MessageBox.Show("Выберите велосипед!", "Предупреждение");
                            return;
                        }

                        if (SelectedBicycle.Status == "rented")
                        {
                            MessageBox.Show("Нельзя изменить статус арендованного велосипеда!", "Ошибка");
                            return;
                        }

                        using (var db = new ModelRentalBicycle())
                        {
                            var bike = db.Bicycles.Find(SelectedBicycle.Id);
                            if (bike != null)
                            {
                                bike.Status = "free";
                                db.SaveChanges();
                                LoadBicycles();
                            }
                        }
                    }, obj => SelectedBicycle != null));
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
