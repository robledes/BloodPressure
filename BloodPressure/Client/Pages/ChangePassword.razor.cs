namespace BloodPressure.Client.Pages;

partial class ChangePassword
{
    [Inject] private IAuthService? AuthService { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }

    [Parameter] public string ReturnUrl { get; set; }

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

    private readonly UserChangePassword _userChangePassword = new();

    private async void HandleChangePassword()
    {
        ServiceResponse<bool>? result = await AuthService.ChangePassword(_userChangePassword);

        Snackbar.Add(
            result.Message,
            Severity.Success,
            config => { config.Icon = Icons.Outlined.Password; }
        );

        NavigationManager.NavigateTo(ReturnUrl);
    }
}
