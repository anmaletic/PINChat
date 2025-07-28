namespace PINChat.UI.ViewManager.Interfaces;

public interface IViewManager
{
    void RegisterView<TView, TViewModel>(string? viewName = null)
        where TView : class, new()
        where TViewModel : notnull;

    TView GetView<TView>(string viewName, Dictionary<string, object>? parameters = null, Action? callback = null)
        where TView : class;

    bool IsViewRegistered(string viewName);
}