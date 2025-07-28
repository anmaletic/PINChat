namespace PINChat.UI.ViewModels;

public partial class LoadableViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;
}