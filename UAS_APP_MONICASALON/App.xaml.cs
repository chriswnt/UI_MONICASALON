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
<<<<<<< HEAD
            // Keep the mobile-first layout usable when the desktop window is
            // resized by hand; below this width the form becomes unreadable.
            window.MinimumWidth = 360;

            // Use the native work area so the window fits above the taskbar at
            // any display scaling, while keeping the mobile-first layout narrow.
            window.Created += (_, _) => DesktopWindowSizer.Apply(window);
=======
            // Preview the mobile UI in a phone-sized window on desktop while
            // keeping it inside the usable screen area.
            var display = DeviceDisplay.Current.MainDisplayInfo;
            var screenWidth = display.Width / display.Density;
            var screenHeight = display.Height / display.Density;

            window.Width = Math.Min(430, screenWidth - 40);
            window.Height = Math.Min(820, screenHeight - 80);
            window.X = (screenWidth - window.Width) / 2;
            window.Y = (screenHeight - window.Height) / 2;
>>>>>>> 4fb551ee537c1081a19e131aa6999af24d715ac3
#endif

            return window;
        }
    }
}
