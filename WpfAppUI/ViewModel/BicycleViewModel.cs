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
    public class BicycleViewModel : INotifyPropertyChanged
    {
        private Bicycle _selectedBicycle;
        /// <summary>
        /// выделенный в списке велосипед
        /// </summary>
        public Bicycle SelectedBicycle
        {
            get { return _selectedBicycle; }
            set
            {
                _selectedBicycle = value;
                OnPropertyChanged("SelectedBicycle");
            }
        }

        /// <summary>
        /// коллекция велосипедов
        /// </summary>
        public ObservableCollection<Bicycle> ListBicycle { get; set; }

        public BicycleViewModel()
        {
            ListBicycle = new ObservableCollection<Bicycle>();
            ListBicycle = GetBicycles();
        }

        private ObservableCollection<Bicycle> GetBicycles()
        {
            using (var context = new ModelRentalBicycle())
            {
                var query = from b in context.Bicycles
                            .Include("Station")
                            orderby b.Id
                            select b;
                if (query.Count() != 0)
                {
                    foreach (var b in query)
                    {
                        ListBicycle.Add(b);
                    }
                }
            }
            return ListBicycle;
        }

        #region AddBicycle
        /// <summary>
        /// добавление велосипеда
        /// </summary>
        private RelayCommand _addBicycle;
        public RelayCommand AddBicycle
        {
            get
            {
                return _addBicycle ??
                (_addBicycle = new RelayCommand(obj =>
                {
                    Bicycle newBicycle = new Bicycle();
                    NewBicycleWindow wnBicycle = new NewBicycleWindow
                    {
                        Title = "Новый велосипед",
                        DataContext = newBicycle
                    };
                    wnBicycle.ShowDialog();
                    if (wnBicycle.DialogResult == true)
                    {
                        using (var context = new ModelRentalBicycle())
                        {
                            try
                            {
                                context.Bicycles.Add(newBicycle);
                                context.SaveChanges();
                                ListBicycle.Clear();
                                ListBicycle = GetBicycles();
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

        #region EditBicycle
        /// команда редактирования велосипеда
        private RelayCommand _editBicycle;
        public RelayCommand EditBicycle
        {
            get
            {
                return _editBicycle ??
                (_editBicycle = new RelayCommand(obj =>
                {
                    Bicycle editBicycle = SelectedBicycle;
                    NewBicycleWindow wnBicycle = new NewBicycleWindow
                    {
                        Title = "Редактирование велосипеда",
                        DataContext = editBicycle
                    };
                    wnBicycle.ShowDialog();
                    if (wnBicycle.DialogResult == true)
                    {
                        using (var context = new ModelRentalBicycle())
                        {
                            Bicycle bicycle = context.Bicycles.Find(editBicycle.Id);
                            if (bicycle != null)
                            {
                                bicycle.Model = editBicycle.Model;
                                bicycle.Status = editBicycle.Status;
                                bicycle.StationId = editBicycle.StationId;
                                try
                                {
                                    context.SaveChanges();
                                    ListBicycle.Clear();
                                    ListBicycle = GetBicycles();
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
                        ListBicycle.Clear();
                        ListBicycle = GetBicycles();
                    }
                }, (obj) => SelectedBicycle != null && ListBicycle.Count > 0));
            }
        }
        #endregion

        #region DeleteBicycle
        /// команда удаления велосипеда
        private RelayCommand _deleteBicycle;
        public RelayCommand DeleteBicycle
        {
            get
            {
                return _deleteBicycle ??
                (_deleteBicycle = new RelayCommand(obj =>
                {
                    Bicycle delBicycle = SelectedBicycle;
                    using (var context = new ModelRentalBicycle())
                    {
                        Bicycle bicycle = context.Bicycles.Find(delBicycle.Id);
                        if (bicycle != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить велосипед: \nМодель: " + bicycle.Model,
                                "Предупреждение", MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Bicycles.Remove(bicycle);
                                    context.SaveChanges();
                                    ListBicycle.Remove(delBicycle);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedBicycle != null && ListBicycle.Count > 0));
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