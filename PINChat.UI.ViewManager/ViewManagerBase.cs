namespace PINChat.UI.ViewManager;

public class ViewManagerBase : IViewManager
{
    private readonly Dictionary<string, Func<Dictionary<string, object>, Action?, object>> _views = new();
    private readonly IServiceProvider _serviceProvider;

    public ViewManagerBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void RegisterView<TView, TViewModel>(string? viewName = null)
        where TView : class, new()
        where TViewModel : notnull
    {
        var name = viewName ?? typeof(TViewModel).Name;
        
        _views[name] = (parameters, callback) =>
        {
            var view = new TView();
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            
            if (viewModel is IParameterized parameterizedVm)
            {
                parameterizedVm.OnParametersSet(parameters);
            }

            if (viewModel is IClosable closableVm && callback != null)
            {
                closableVm.OnClosed += callback;
            }
            
            if (view is Avalonia.Controls.Control avaloniaView)
            {
                avaloniaView.DataContext = viewModel;
            }
            
            return view;
        };
    }
    
    public TView GetView<TView>(string viewName, Dictionary<string, object>? parameters = null, Action? callback = null) 
        where TView : class
    {
        if (_views.TryGetValue(viewName, out var viewFactory))
        {
            var view = viewFactory(parameters ?? new Dictionary<string, object>(), callback);
            return (view as TView) ?? throw new InvalidCastException($"View '{viewName}' is not of type {typeof(TView)}");
        }

        throw new KeyNotFoundException($"View '{viewName}' not found.");
    }

    public bool IsViewRegistered(string viewName) => _views.ContainsKey(viewName);
}