namespace PINChat.UI.ViewManager;

public class ViewManagerBuilder
{
    private readonly ViewManager _viewManager;

    internal ViewManagerBuilder(ViewManager viewManager)
    {
        _viewManager = viewManager;
    }

    public ViewManagerBuilder RegisterView<TView, TViewModel>(string? viewName = null)
        where TView : UserControl, new()
        where TViewModel : notnull
    {
        _viewManager.RegisterView<TView, TViewModel>(viewName);
        return this;
    }
}