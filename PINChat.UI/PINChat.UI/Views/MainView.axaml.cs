using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Interactivity;

namespace PINChat.UI.Views
{
    public partial class MainView : UserControl
    {
        public IInputPane? InputPane { get; private set; }
        public IInsetsManager? InsetsManager { get; private set; }
        
        public MainView()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            
            InputPane = TopLevel.GetTopLevel(this)!.InputPane;
            if (InputPane != null)
            {
                InputPane.StateChanged += InputPane_StateChanged;
                InsetsManager = TopLevel.GetTopLevel(this)!.InsetsManager;
            }
        }
        
        private void InputPane_StateChanged(object? sender, InputPaneStateEventArgs e)
        {
            if (DataContext is MainViewModel model && InputPane is not null && InsetsManager is not null)
            {
                var safeArea = InsetsManager.SafeAreaPadding;
                var occludedArea = InputPane.OccludedRect;
 
                model.SafeArea = new Thickness(safeArea.Left, safeArea.Top, safeArea.Right, occludedArea.Height);
            }
        }
    }
}