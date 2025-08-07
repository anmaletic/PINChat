using Android.App;
using Android.Content.PM;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;

namespace PINChat.UI.Android
{
    [Activity(
        Label = "PINChat.UI.Android",
        Theme = "@style/MyTheme.NoActionBar",
        Icon = "@drawable/icon", 
        WindowSoftInputMode = SoftInput.AdjustResize,
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
    public class MainActivity : AvaloniaMainActivity<App>
    {
        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            IconProvider.Current
                .Register<FontAwesomeIconProvider>();
         
            return base.CustomizeAppBuilder(builder)
                .WithInterFont();
        }
    }
}
