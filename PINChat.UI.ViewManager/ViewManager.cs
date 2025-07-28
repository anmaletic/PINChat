namespace PINChat.UI.ViewManager;

public class ViewManager : IViewManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Func<IServiceProvider, Dictionary<string, object>, Action?, UserControl>> _views = new();

    public ViewManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    internal void RegisterView<TView, TViewModel>(string? viewName = null)
        where TView : UserControl, new()
        where TViewModel : notnull
    {
        var name = viewName ?? typeof(TViewModel).Name;

        _views[name] = (provider, parameters, callback) =>
        {
            var view = new TView();
            var viewModel = provider.GetRequiredService<TViewModel>();

            if (viewModel is IParameterized parameterizedVm)
            {
                parameterizedVm.OnParametersSet(parameters);
            }

            if (viewModel is IClosable closableVm && callback != null)
            {
                closableVm.OnClosed += callback;
            }

            view.DataContext = viewModel;
            return view;
        };
    }

    public UserControl GetView(string viewName, Dictionary<string, object>? parameters = null, Action? callback = null)
    {
        if (_views.TryGetValue(viewName, out var viewFactory))
        {
            return viewFactory(_serviceProvider, parameters ?? new Dictionary<string, object>(), callback);
        }

        throw new KeyNotFoundException($"View '{viewName}' not found.");
    }
}