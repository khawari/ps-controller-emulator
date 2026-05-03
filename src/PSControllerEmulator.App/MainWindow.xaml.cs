using PSControllerEmulator.Core.Controllers;
using PSControllerEmulator.Devices;
using System.Windows;

namespace PSControllerEmulator.App;

public partial class MainWindow : Window
{
    private readonly IControllerDiscoveryService _discoveryService;

    public MainWindow()
    {
        InitializeComponent();
        _discoveryService = DeviceServiceFactory.CreateForApp();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await RefreshControllersAsync();
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await RefreshControllersAsync();
    }

    private async Task RefreshControllersAsync()
    {
        RefreshButton.IsEnabled = false;
        StatusText.Text = "Scanning USB controllers...";

        try
        {
            var controllers = await _discoveryService.DiscoverAsync();
            var summaries = controllers.Select(ControllerSummaryViewModel.From).ToList();

            ControllerList.ItemsSource = summaries;
            EmptyState.Visibility = summaries.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ModeBadge.Text = summaries.Any(controller => controller.IsMock) ? "Mock fallback" : "USB hardware";
            StatusText.Text = summaries.Count == 1
                ? "1 controller available"
                : $"{summaries.Count} controllers available";
        }
        catch (Exception ex)
        {
            ControllerList.ItemsSource = Array.Empty<ControllerSummaryViewModel>();
            EmptyState.Visibility = Visibility.Visible;
            ModeBadge.Text = "Scan error";
            StatusText.Text = ex.Message;
        }
        finally
        {
            RefreshButton.IsEnabled = true;
        }
    }

    private sealed record ControllerSummaryViewModel(
        string DisplayName,
        string Connection,
        string Battery,
        string InputState,
        string Features,
        bool IsMock)
    {
        public static ControllerSummaryViewModel From(ConnectedController controller)
        {
            return new ControllerSummaryViewModel(
                controller.DisplayName,
                controller.ConnectionType.ToDisplayText(),
                controller.Battery.ToDisplayText(),
                controller.InputState.ToDisplayText(),
                controller.Capabilities.ToDisplayText(),
                controller.IsMock);
        }
    }
}
