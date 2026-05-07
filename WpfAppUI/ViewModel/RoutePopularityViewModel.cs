using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppUI.Model;

namespace WpfAppUI.ViewModel
{
    public class RoutePopularityViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<RouteStat> PopularRoutes { get; set; }

        public RoutePopularityViewModel()
        {
            PopularRoutes = new ObservableCollection<RouteStat>();
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new ModelRentalBicycle())
            {
                var query = from r in context.Rentals
                            where r.EndStationId != null  // только завершённые аренды
                            group r by new { r.StartStationId, r.EndStationId } into g
                            orderby g.Count() descending
                            select new
                            {
                                StartId = g.Key.StartStationId,
                                EndId = g.Key.EndStationId,
                                Count = g.Count()
                            };

                var stations = context.Stations.ToDictionary(s => s.Id, s => s.Name);

                foreach (var item in query)
                {
                    PopularRoutes.Add(new RouteStat
                    {
                        StartStation = stations[item.StartId],
                        EndStation = stations[item.EndId.Value],
                        TripCount = item.Count
                    });
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
