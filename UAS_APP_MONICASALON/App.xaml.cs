namespace UAS_APP_MONICASALON
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Light;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

#if WINDOWS
            // Preview the mobile UI in a phone-sized window on desktop while
            // keeping it inside the usable screen area.
            var display = DeviceDisplay.Current.MainDisplayInfo;
            var screenWidth = display.Width / display.Density;
            var screenHeight = display.Height / display.Density;

            window.Width = Math.Min(430, screenWidth - 40);
            window.Height = Math.Min(820, screenHeight - 80);
            window.X = (screenWidth - window.Width) / 2;
            window.Y = (screenHeight - window.Height) / 2;
#endif

            return window;
        }
    }
}
