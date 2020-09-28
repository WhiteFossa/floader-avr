using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FloaderClientGUI.Views
{
    public class AboutWindow : Window
    {
        public AboutWindow()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
