using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using PINChat.UI.Core.Helpers;
using PINChat.UI.Core.Messages;
using PINChat.UI.ViewManager.Interfaces;

namespace PINChat.UI.ViewModels;

public partial class LoginViewModel : LoadableViewModelBase
{
    private readonly IViewManager _viewManager;
    
    [ObservableProperty]
    private UserControl _currentContent = new();

    public LoginViewModel() : this(null!)
    {
    }
    
    public LoginViewModel(IViewManager viewManager)
    {
   
        _viewManager = viewManager;
        
        StrongReferenceMessenger.Default.Register<ChangeLoginViewMessage>(this, OnChangeViewMessageReceived);
        StrongReferenceMessenger.Default.Register<IsViewDeactivatedMessage>(this, OnDeactivatedMessageReceived);

        InitializeDefaultView();
    }
    
    private void OnDeactivatedMessageReceived(object recipient, IsViewDeactivatedMessage message)
    {
        if (message.IsDeactivated)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
        }
    }

    private void InitializeDefaultView()
    {
        ShowView(PlatformHelper.SignInView);
    }
    
    private void OnChangeViewMessageReceived(object recipient, ChangeLoginViewMessage msg)
    {
        switch (msg.View)
        {
            case "SignUpViewModel":
                ShowView(PlatformHelper.SignUpView);
                return;
            case "SignInViewModel":
                ShowView(PlatformHelper.SignInView);
                return;
        }
    }
    
    private void ShowView(string viewName)
    {
        CurrentContent = _viewManager.GetView(viewName);
    }
}