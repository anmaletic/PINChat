using CommunityToolkit.Mvvm.Messaging;
using PINChat.UI.Core.Messages;

namespace PINChat.UI.ViewModels;

public partial class LoadableViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    protected LoadableViewModelBase()
    {
        StrongReferenceMessenger.Default.Register<IsViewDeactivatedMessage>(this, OnDeactivatedMessageReceived);
    }
    
    private void OnDeactivatedMessageReceived(object recipient, IsViewDeactivatedMessage message)
    {
        if (message.IsDeactivated)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}