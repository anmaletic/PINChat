using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using PINChat.UI.Core.Helpers;
using PINChat.UI.Core.Messages;
using PINChat.UI.ViewManager.Interfaces;

namespace PINChat.UI.ViewModels;

public partial class MainViewModel : MainViewModelBase
{

    private readonly IViewManager _viewManager;

    [ObservableProperty]
    private UserControl? _currentView;

    public MainViewModel() : this(null!)
    {
    }
    
    public MainViewModel(IViewManager viewManager)
    {
        _viewManager = viewManager;
        
        StrongReferenceMessenger.Default.Register<ChangeViewMessage>(this, OnChangeViewMessageReceived);

        InitializeDefaultView();
    }
    
    private void InitializeDefaultView()
    {
        ShowView(nameof(LoginViewModel));
    }
    
    private void OnChangeViewMessageReceived(object recipient, ChangeViewMessage msg)
    {
        ShowView(msg.View);
    }
    
    private void ShowView(string viewName)
    {
        CurrentView = viewName switch
        {
            "ChatViewModel" => _viewManager.GetView(PlatformHelper.ChatView),
            "LoginViewModel" => _viewManager.GetView(viewName),
            _ => CurrentView
        };
    }
}