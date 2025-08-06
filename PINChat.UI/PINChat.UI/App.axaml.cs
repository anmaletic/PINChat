using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using PINChat.Api.Sdk;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Services;

namespace PINChat.UI
{
    public partial class App : Application
    {
        public App()
        {
            ConfigureServices();
        }
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            this.AttachDeveloperTools();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                
                desktop.MainWindow = new MainWindow
                {
                    DataContext = Ioc.Default.GetService<MainViewModel>()
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = Ioc.Default.GetService<MainViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
        
        private void ConfigureServices()
        {
            Ioc.Default.ConfigureServices(new ServiceCollection()
                // .AddSingleton<GlobalErrorHandler>()
                .AddSingleton<MainWindow>()
                .AddSingleton<DialogService>()
                .AddSingleton<Func<TopLevel?>>(x=> () =>
                {
                    return ApplicationLifetime switch
                    {
                        IClassicDesktopStyleApplicationLifetime topWindow => TopLevel.GetTopLevel(topWindow.MainWindow),
                        ISingleViewApplicationLifetime singleViewPlatform => TopLevel.GetTopLevel(singleViewPlatform.MainView),
                        _ => null
                    };
                })
                .AddViewModels()
                .AddViewManager()
                .AddApiSdk()
                .AddSingleton<IChatService, ChatService>()
                .AddSingleton<IMinioFrontendService, MinioFrontendService>()
                .AddSingleton<ILoggedInUserService, LoggedInUserService>()
                .BuildServiceProvider());
        }
    }
}