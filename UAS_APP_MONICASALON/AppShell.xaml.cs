using UAS_APP_MONICASALON.Views;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(HomeCustomerPage), typeof(HomeCustomerPage));
            Routing.RegisterRoute(nameof(AdminDashboardPage), typeof(AdminDashboardPage));
            Routing.RegisterRoute(nameof(AdminBookingsPage), typeof(AdminBookingsPage));

            // Cashier (Kasir) module
            Routing.RegisterRoute(nameof(CashierPage), typeof(CashierPage));
            Routing.RegisterRoute(nameof(TransactionReviewPage), typeof(TransactionReviewPage));
            Routing.RegisterRoute(nameof(PaymentPage), typeof(PaymentPage));
            Routing.RegisterRoute(nameof(TransactionSuccessPage), typeof(TransactionSuccessPage));
            Routing.RegisterRoute(nameof(TransactionHistoryPage), typeof(TransactionHistoryPage));
            Routing.RegisterRoute(nameof(TransactionDetailsPage), typeof(TransactionDetailsPage));

            // Booking module
            Routing.RegisterRoute(nameof(AdminCreateBookingPage), typeof(AdminCreateBookingPage));
            Routing.RegisterRoute(nameof(AdminBookingDetailsPage), typeof(AdminBookingDetailsPage));
            Routing.RegisterRoute(nameof(AdminReschedulePage), typeof(AdminReschedulePage));
        }
    }
}
