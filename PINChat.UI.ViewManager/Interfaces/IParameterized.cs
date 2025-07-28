namespace PINChat.UI.ViewManager.Interfaces;

public interface IParameterized
{
    Task OnParametersSet(Dictionary<string, object> parameters);
}