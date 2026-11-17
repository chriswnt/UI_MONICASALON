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
<<<<<<< HEAD
            Routing.RegisterRoute(nameof(CataloguePage), typeof(CataloguePage));
            Routing.RegisterRoute(nameof(ServiceDetailPage), typeof(ServiceDetailPage));
            Routing.RegisterRoute(nameof(SchedulePage), typeof(SchedulePage));
            Routing.RegisterRoute(nameof(ConfirmBookingPage), typeof(ConfirmBookingPage));
            Routing.RegisterRoute(nameof(BookingSuccessPage), typeof(BookingSuccessPage));
            Routing.RegisterRoute(nameof(BookingHistoryPage), typeof(BookingHistoryPage));
            Routing.RegisterRoute(nameof(BookingDetailsPage), typeof(BookingDetailsPage));
            Routing.RegisterRoute(nameof(BookingFeedbackPage), typeof(BookingFeedbackPage));
            Routing.RegisterRoute(nameof(HairstyleBookPage), typeof(HairstyleBookPage));
            Routing.RegisterRoute(nameof(HairstyleDetailPage), typeof(HairstyleDetailPage));
=======
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
>>>>>>> 4fb551ee537c1081a19e131aa6999af24d715ac3
        }
    }
}
