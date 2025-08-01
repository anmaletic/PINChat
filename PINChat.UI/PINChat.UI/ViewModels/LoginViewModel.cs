using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation;
using PINChat.Api.Sdk;
using PINChat.UI.Core.Components;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Messages;
using PINChat.UI.Core.Models;
using PINChat.UI.ViewManager.Interfaces;

namespace PINChat.UI.ViewModels;

public partial class LoginViewModel : LoadableViewModelBase
{
    private readonly IViewManager _viewManager;
    
    [ObservableProperty]
    private UserControl _currentContent = new();

    public LoginViewModel() : this(null!)
    {
        // Default constructor for design-time data or testing purposes
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
        ShowView(nameof(SignInViewModel));
    }
    
    private void OnChangeViewMessageReceived(object recipient, ChangeLoginViewMessage msg)
    {
        ShowView(msg.View);
    }
    
    private void ShowView(string viewName)
    {
        CurrentContent = _viewManager.GetView(viewName);
    }
}