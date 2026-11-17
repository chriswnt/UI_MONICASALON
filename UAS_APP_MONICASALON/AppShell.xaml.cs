using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
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
        }
    }
}
