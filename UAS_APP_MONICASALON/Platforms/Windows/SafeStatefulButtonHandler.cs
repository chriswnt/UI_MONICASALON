using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MauiButton = Microsoft.Maui.Controls.Button;
using WinButton = Microsoft.UI.Xaml.Controls.Button;
using MauiVisualStateManager = Microsoft.Maui.Controls.VisualStateManager;

namespace UAS_APP_MONICASALON.Platforms.Windows;

/// <summary>
/// Windows handler for regular <see cref="Microsoft.Maui.Controls.Button"/>
/// controls.
///
/// UraniumUI 3.0 registers its own <c>StatefulButtonHandler</c> for every
/// Button. That handler calls <c>MauiVisualStateManager.GoToState(..., "Normal")</c>
/// from its <c>DisconnectHandler</c>, but MAUI has already cleared the platform
/// view at that point. Re-applying the style setters then throws
/// "PlatformView cannot be null here" and crashes the app when a pressed
/// button is removed from the navigation stack.
///
/// This handler keeps the same hover/pressed visual behaviour but skips the
/// visual-state reset while the handler is being disconnected.
/// </summary>
public class SafeStatefulButtonHandler : ButtonHandler
{
    private bool _disconnecting;

    private void ResetState()
    {
        if (_disconnecting)
        {
            return;
        }

        if (VirtualView is VisualElement element)
        {
            MauiVisualStateManager.GoToState(element, "Normal");
        }
    }

    private void VirtualView_Unloaded(object? sender, EventArgs e) => ResetState();

    protected override void ConnectHandler(WinButton platformView)
    {
        base.ConnectHandler(platformView);
        _disconnecting = false;

        if (VirtualView is MauiButton virtualButton)
        {
            virtualButton.Unloaded -= VirtualView_Unloaded;
            virtualButton.Unloaded += VirtualView_Unloaded;
        }

        platformView.PointerCanceled += NativeView_PointerCanceled;
        platformView.PointerCaptureLost += NativeView_PointerCaptureLost;
        platformView.Unloaded += NativeView_Unloaded;
    }

    protected override void DisconnectHandler(WinButton platformView)
    {
        _disconnecting = true;

        platformView.PointerCanceled -= NativeView_PointerCanceled;
        platformView.PointerCaptureLost -= NativeView_PointerCaptureLost;
        platformView.Unloaded -= NativeView_Unloaded;

        if (VirtualView is MauiButton virtualButton)
        {
            virtualButton.Unloaded -= VirtualView_Unloaded;
        }

        // Deliberately no ResetState() here: the platform view is already
        // cleared, so clearing visual-state setters would throw.
        base.DisconnectHandler(platformView);
    }

    protected override WinButton CreatePlatformView()
    {
        var platformView = base.CreatePlatformView();

        platformView.PointerEntered += (_, _) =>
        {
            if (VirtualView is VisualElement element)
            {
                MauiVisualStateManager.GoToState(element, "Hover");
            }
        };

        platformView.PointerExited += (_, _) =>
        {
            if (VirtualView is VisualElement element)
            {
                MauiVisualStateManager.GoToState(element, "Normal");
            }
        };

        platformView.AddHandler(
            UIElement.PointerPressedEvent,
            new PointerEventHandler(NativeView_PointerPressed),
            handledEventsToo: true);
        platformView.AddHandler(
            UIElement.PointerReleasedEvent,
            new PointerEventHandler(NativeView_PointerReleased),
            handledEventsToo: true);

        return platformView;
    }

    private void NativeView_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (VirtualView is VisualElement element)
        {
            MauiVisualStateManager.GoToState(element, "Pressed");
        }
    }

    private void NativeView_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (VirtualView is VisualElement element)
        {
            MauiVisualStateManager.GoToState(element, "Normal");
        }
    }

    private void NativeView_PointerCanceled(object sender, PointerRoutedEventArgs e) => ResetState();

    private void NativeView_PointerCaptureLost(object sender, PointerRoutedEventArgs e) => ResetState();

    private void NativeView_Unloaded(object sender, RoutedEventArgs e) => ResetState();
}
