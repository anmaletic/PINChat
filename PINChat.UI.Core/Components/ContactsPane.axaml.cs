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
    
    public ContactsPane()
    {
        InitializeComponent();
        
        SelectContactCommand = new RelayCommand<UserModel>(contact =>
        {
            Dispatcher.UIThread.InvokeAsync(() => SelectedContact = contact);
        });
        
        ContactsProperty.Changed.AddClassHandler<ContactsPane>((s, e) =>
        {
            s.ApplyFilter();
        });
        
        ApplyFilter();
    }
    
    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        ApplyFilter(((TextBox)sender!).Text);
    }
    
    private void ApplyFilter(string? filter = "")
    {
        FilteredContacts.Clear();

        if (Contacts.Count == 0)
        {
            return; // No contacts to filter
        }

        // If search text is empty, show all contacts
        if (string.IsNullOrWhiteSpace(filter))
        {
            foreach (var contact in Contacts)
            {
                FilteredContacts.Add(contact);
            }
        }
        else // Apply filter based on search text
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