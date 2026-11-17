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
            // Keep the mobile-first layout usable when the desktop window is
            // resized by hand; below this width the form becomes unreadable.
            window.MinimumWidth = 360;

            // Use the native work area so the window fits above the taskbar at
            // any display scaling, while keeping the mobile-first layout narrow.
            window.Created += (_, _) => DesktopWindowSizer.Apply(window);
#endif

            return window;
        }
    }
}
