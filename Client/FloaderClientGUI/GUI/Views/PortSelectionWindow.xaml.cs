using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FloaderClientGUI.Views
{
    public class PortSelectionWindow : Window
    {
        public PortSelectionWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}