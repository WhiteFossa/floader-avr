using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FloaderClientGUI.ViewModels;

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

            // Setting up ports list double click handler
            var portsListBox = this.Find<ListBox>("PortsListBox");
            portsListBox.DoubleTapped += OnPortDoubleClick;
        }

        /// <summary>
        /// Called when user double clicks a port
        /// </summary>
        private void OnPortDoubleClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (PortSelectionWindowViewModel)((Control)sender).DataContext;

            viewModel.OK(this);
        }
    }
}