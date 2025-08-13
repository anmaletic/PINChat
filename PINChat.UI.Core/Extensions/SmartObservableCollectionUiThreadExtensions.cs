using Avalonia.Threading;

namespace PINChat.UI.Core.Extensions;

public static class SmartCollectionUiThreadExtensions
{
    public static UiThreadSmartCollectionWrapper<T> OnUIThread<T>(
        this SmartObservableCollection<T> collection)
    {
        return new UiThreadSmartCollectionWrapper<T>(collection);
    }
}

public class UiThreadSmartCollectionWrapper<T>
{
    private readonly SmartObservableCollection<T> _collection;

    internal UiThreadSmartCollectionWrapper(SmartObservableCollection<T> collection)
    {
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    public async Task AddRangeAsync(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        await Dispatcher.UIThread.InvokeAsync(() => _collection.AddRange(items));
    }

    public async Task ClearAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() => _collection.Clear());
    }
}