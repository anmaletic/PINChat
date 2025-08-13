using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using PINChat.UI.Core.Extensions;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.ViewManager.Interfaces;

namespace PINChat.UI.ViewModels;

public partial class ErrorDialogViewModel : ViewModelBase, IParameterized, IClosable, IUpdateable
{
    public event Action? OnClosed;
    
    [ObservableProperty]
    private SmartObservableCollection<string> _errors = [];
    
    [ObservableProperty]
    private string? _title = string.Empty;
    
    public void OnParametersSet(Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("Errors", out var errors))
        {
            Errors.AddRange((List<string>)errors);
        }

        Title = $"Exceptions dialog ( {Errors.Count} )";
    }
    
    [RelayCommand]
    private void Close()
    {
        OnClosed?.Invoke();
    }

    public void Update(IEnumerable<string> items)
    {
        Errors.Clear();
        foreach (var item in items)
        {
            Errors.Add(item);
        }
        
        Title = $"Exceptions dialog ( {Errors.Count} )";
    }
}