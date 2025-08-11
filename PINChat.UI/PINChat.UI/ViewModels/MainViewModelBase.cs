using Avalonia;

namespace PINChat.UI.ViewModels;

public partial class MainViewModelBase : ViewModelBase
{
    [ObservableProperty]
    private Thickness? _safeArea;    
}