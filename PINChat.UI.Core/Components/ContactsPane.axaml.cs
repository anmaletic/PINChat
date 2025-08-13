using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Components;

public partial class ContactsPane : UserControl
{
    private string _currentFilter = string.Empty;
    
    public static readonly StyledProperty<ObservableCollection<UserModel>> ContactsProperty =
        AvaloniaProperty.Register<ContactsPane, ObservableCollection<UserModel>>(nameof(Contacts), []);

    public ObservableCollection<UserModel> Contacts
    {
        get => GetValue(ContactsProperty);
        set => SetValue(ContactsProperty, value);
    }

    public static readonly StyledProperty<UserModel?> SelectedContactProperty =
        AvaloniaProperty.Register<ContactsPane, UserModel?>(nameof(SelectedContact));

    public UserModel? SelectedContact
    {
        get => GetValue(SelectedContactProperty);
        set => SetValue(SelectedContactProperty, value);
    }
    public ObservableCollection<UserModel> FilteredContacts { get; } = [];
    public ICommand SelectContactCommand { get; }
    
    static ContactsPane()
    {
        ContactsProperty.Changed.AddClassHandler<ContactsPane>((control, e) =>
        {
            control.OnContactsChanged(e);
        });
    }
    
    public ContactsPane()
    {
        InitializeComponent();
        
        SelectContactCommand = new RelayCommand<UserModel>(contact =>
        {
            Dispatcher.UIThread.InvokeAsync(() => SelectedContact = contact);
        });
        
        ApplyFilter();
    }
    
    private void OnContactsChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.OldValue is ObservableCollection<UserModel> oldCollection)
        {
            oldCollection.CollectionChanged -= OnContactsCollectionChanged;
        }
        
        if (e.NewValue is ObservableCollection<UserModel> newCollection)
        {
            newCollection.CollectionChanged += OnContactsCollectionChanged;
        }
        
        ApplyFilter(_currentFilter);
    }
    
    private void OnContactsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() => ApplyFilter(_currentFilter));
    }
    
    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var filterText = ((TextBox)sender!).Text ?? string.Empty;
        ApplyFilter(filterText);
    }
    
    private void ApplyFilter(string? filter = "")
    {
        _currentFilter = filter ?? string.Empty;
        
        FilteredContacts.Clear();

        if (Contacts.Count == 0)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(filter))
        {
            foreach (var contact in Contacts)
            {
                FilteredContacts.Add(contact);
            }
        }
        else
        {
            var lowerCaseSearchText = filter.ToLowerInvariant();
            foreach (var contact in Contacts)
            {
                if (contact.UserName!.ToLowerInvariant().Contains(lowerCaseSearchText))
                {
                    FilteredContacts.Add(contact);
                }
            }
        }
    }
}