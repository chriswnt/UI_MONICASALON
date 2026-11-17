using System.Globalization;
using Microsoft.Extensions.Logging;
using UAS_APP_MONICASALON.Controls;
using UraniumUI;
using UraniumUI.Icons.MaterialSymbols;
using UraniumUI.Material.Handlers;

namespace UAS_APP_MONICASALON
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Keep UraniumUI's date picker and the rest of the UI in Indonesian.
            var indonesian = CultureInfo.GetCultureInfo("id-ID");
            CultureInfo.DefaultThreadCurrentCulture = indonesian;
            CultureInfo.DefaultThreadCurrentUICulture = indonesian;
            Thread.CurrentThread.CurrentCulture = indonesian;
            Thread.CurrentThread.CurrentUICulture = indonesian;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureMauiHandlers(handlers =>
                {
                    // The shim derives from UraniumUI's ButtonView, so it needs the
                    // same handler to receive pointer/tap input.
                    handlers.AddHandler<ButtonView, ButtonViewHandler>();

#if WINDOWS
                    // Replace UraniumUI's StatefulButtonHandler for plain Buttons.
                    // Its disconnect path resets the visual state after MAUI has
                    // cleared the platform view, which crashes on navigation.
                    handlers.AddHandler<Button, Platforms.Windows.SafeStatefulButtonHandler>();
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("PlusJakartaSans-Regular.ttf", "PlusJakartaSansRegular");
                    fonts.AddFont("PlusJakartaSans-Medium.ttf", "PlusJakartaSansMedium");
                    fonts.AddFont("PlusJakartaSans-SemiBold.ttf", "PlusJakartaSansSemiBold");
                    fonts.AddFont("PlusJakartaSans-Bold.ttf", "PlusJakartaSansBold");
                    fonts.AddMaterialSymbolsFonts();
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
