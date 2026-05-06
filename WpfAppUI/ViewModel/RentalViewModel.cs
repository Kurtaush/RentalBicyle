using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using WpfAppUI.Model;

namespace WpfAppUI.ViewModel
{
    public class RentalViewModel : INotifyPropertyChanged
    {
        private Rental _selectedRental;
        /// <summary>
        /// выделенная в списке аренда
        /// </summary>
        public Rental SelectedRental
        {
            get { return _selectedRental; }
            set
            {
                _selectedRental = value;
                OnPropertyChanged("SelectedRental");
            }
        }

        /// <summary>
        /// коллекция аренд
        /// </summary>
        public ObservableCollection<Rental> ListRental { get; set; }

        public RentalViewModel()
        {
            ListRental = new ObservableCollection<Rental>();
            ListRental = GetRentals();
        }

        private ObservableCollection<Rental> GetRentals()
        {
            using (var context = new ModelRentalBicycle())
            {
                var query = from r in context.Rentals
                            .Include("Client")
                            .Include("Bicycle")
                            .Include("StartStation")
                            .Include("EndStation")
                            .Include("Tariff")
                            orderby r.Id
                            select r;
                if (query.Count() != 0)
                {
                    foreach (var r in query)
                    {
                        ListRental.Add(r);
                    }
                }
            }
            return ListRental;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}