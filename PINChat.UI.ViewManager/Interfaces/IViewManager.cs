namespace PINChat.UI.ViewManager.Interfaces;

public interface IViewManager
{
    UserControl GetView(string viewName, Dictionary<string, object>? parameters = null, Action? callback = null);
}