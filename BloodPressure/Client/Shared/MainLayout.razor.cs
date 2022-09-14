namespace BloodPressure.Client.Shared;

public partial class MainLayout
{
    [Inject] private ISyncLocalStorageService? LocalStorage { get; set; }
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    [Inject] private IDialogService? DialogService { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    private readonly MudTheme _theme = new()
    {
        Typography = new Typography()
        {
            Default = new Default()
            {
                FontFamily = new[] { "Baloo 2", "Calibri" },
            }
        }
    };

    private bool _isDarkMode;
    private bool _drawerOpen = true;
    private string _iconMode = string.Empty;
    private string _lightDarkText = string.Empty;

    private readonly DialogOptions _dialogOptions = new()
    {
        CloseOnEscapeKey = true,
        DisableBackdropClick = true,
        CloseButton = false,
        NoHeader = true
    };

    protected override void OnInitialized()
    {
        _isDarkMode = LocalStorage.GetItem<bool>("isDarkMode");
        _lightDarkText = _isDarkMode ? "Light mode" : "Dark mode";
        SetIconMode();
    }

    private void DrawerToggle() => _drawerOpen = !_drawerOpen;

    private void DarkToggle()
    {
        _isDarkMode = !_isDarkMode;
        _lightDarkText = _isDarkMode ? "Light mode" : "Dark mode";
        SetIconMode();
        LocalStorage.SetItem("isDarkMode", _isDarkMode);
    }

    private void SetIconMode() => _iconMode = _isDarkMode ? Icons.Filled.LightMode : Icons.Outlined.DarkMode;

    private async Task Logout()
    {
        LocalStorage.RemoveItem("authToken");
        await AuthenticationStateProvider.GetAuthenticationStateAsync();
        NavigationManager.NavigateTo("");
        
        Snackbar.Add(
            "You have successfully logged out.",
            Severity.Success,
            config => { config.Icon = Icons.Outlined.Logout; }
        );
    }

    private void Login() => DialogService.Show<Login>(string.Empty, ReturnUrl(), _dialogOptions);

    private void ChangePassword() => DialogService.Show<ChangePassword>(string.Empty, ReturnUrl(), _dialogOptions);

    private void Profile() => DialogService.Show<Profile>(string.Empty, ReturnUrl(), _dialogOptions);

    private void Register() => DialogService.Show<Register>(string.Empty, ReturnUrl(), _dialogOptions);

    private DialogParameters? ReturnUrl()
        => new() { ["ReturnUrl"] = NavigationManager.ToBaseRelativePath(NavigationManager.Uri) };
}