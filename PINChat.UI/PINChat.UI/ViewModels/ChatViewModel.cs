using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PINChat.Core.Domain.Enums;
using PINChat.UI.Core.Helpers;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Messages;
using PINChat.UI.Core.Models;
using PINChat.UI.ViewManager.Interfaces;

namespace PINChat.UI.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    private readonly IViewManager _viewManager;
    private readonly ILoggedInUserService _loggedInUserService;
    
    private string _activeView = "";
    
    [ObservableProperty]
    private UserControl? _currentView;

    [ObservableProperty]
    private UserModel _loggedInUser;

    public ChatViewModel() : this(null!, null!)
    {
    }

    public ChatViewModel(IViewManager viewManager, ILoggedInUserService loggedInUserService)
    {
        _viewManager = viewManager;
        _loggedInUserService = loggedInUserService;

        LoggedInUser = _loggedInUserService.User!;
        
        InitializeDefaultView();
    }
    
    private void InitializeDefaultView()
    {
        ShowView(PlatformHelper.MessagingView);
    }
    
    [RelayCommand]
    private void HandleSelectedDockItem(DockButton btn)
    {
        switch (btn)
        {
            case DockButton.Chat:
                ShowView(PlatformHelper.MessagingView);
                break;
            case DockButton.Contacts:
                ShowView(PlatformHelper.ContactsView);
                break;
            case DockButton.Groups:
                break;
            case DockButton.Theme:
                ChangeTheme();
                break;
            case DockButton.Settings:
                break;
            case DockButton.Profile:
                ShowView(PlatformHelper.ProfileView);
                break;
            case DockButton.Logout:
                Logout();
                break;
            default:
                throw new NotImplementedException();
        }
    }
    
    private void ShowView(string viewName)
    {
        if (string.IsNullOrEmpty(viewName))
            throw new ArgumentException("View name cannot be null or empty.", nameof(viewName));

        if (_activeView == viewName)
        {
            return;
        }

        _activeView = viewName;

        CurrentView = _viewManager.GetView(viewName);
    }

    [RelayCommand]
    private void ChangeTheme()
    {
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = Application.Current.ActualThemeVariant == ThemeVariant.Light
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _loggedInUserService.ClearUser();

        StrongReferenceMessenger.Default.UnregisterAll(this);
        StrongReferenceMessenger.Default.Send(new ChangeViewMessage(nameof(LoginViewModel)));
    }
}