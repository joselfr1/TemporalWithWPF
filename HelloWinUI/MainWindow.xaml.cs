using HelloWinUI.ViewModels;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace HelloWinUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class MainWindow : Window
    {

        public MainWindow(SayHelloViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }


        private async void RunWorkflow_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SayHelloViewModel viewModel)
            {
                await viewModel.SendAsync();
            }
        }

    }
}