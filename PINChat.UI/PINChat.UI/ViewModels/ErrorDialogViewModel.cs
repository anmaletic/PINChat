using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using PINChat.UI.ViewManager.Interfaces;

namespace PINChat.UI.ViewModels;

public partial class ErrorDialogViewModel : ViewModelBase, IParameterized, IClosable
{
    public event Action? OnClosed;
    
    [ObservableProperty]
    private string? _message;
    
    [ObservableProperty]
    private string? _title;
    
    public void OnParametersSet(Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("Title", out var title))
            Title = title.ToString() ?? string.Empty;
            
        if (parameters.TryGetValue("Message", out var message))
            Message = message.ToString() ?? string.Empty;
    }
    
    [RelayCommand]
    private void Close()
    {
        OnClosed?.Invoke();
    }
    
}