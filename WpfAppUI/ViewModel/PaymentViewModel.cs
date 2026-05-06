using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using WpfAppUI.Model;

namespace WpfAppUI.ViewModel
{
    public class PaymentViewModel : INotifyPropertyChanged
    {
        private Payment _selectedPayment;
        /// <summary>
        /// выделенный в списке платёж
        /// </summary>
        public Payment SelectedPayment
        {
            get { return _selectedPayment; }
            set
            {
                _selectedPayment = value;
                OnPropertyChanged("SelectedPayment");
            }
        }

        /// <summary>
        /// коллекция платежей
        /// </summary>
        public ObservableCollection<Payment> ListPayment { get; set; }

        public PaymentViewModel()
        {
            ListPayment = new ObservableCollection<Payment>();
            ListPayment = GetPayments();
        }

        private ObservableCollection<Payment> GetPayments()
        {
            using (var context = new ModelRentalBicycle())
            {
                var query = from p in context.Payments
                            .Include("Rental")
                            orderby p.Id
                            select p;
                if (query.Count() != 0)
                {
                    foreach (var p in query)
                    {
                        ListPayment.Add(p);
                    }
                }
            }
            return ListPayment;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}