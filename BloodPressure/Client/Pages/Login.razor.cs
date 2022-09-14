using Microsoft.AspNetCore.Components.Web;

namespace BloodPressure.Client.Pages;

partial class Login
{
    [Inject] private IAuthService? AuthService { get; set; }
    [Inject] private ISyncLocalStorageService? LocalStorage { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private IDialogService? DialogService { get; set; }

    [Parameter] public string ReturnUrl { get; set; }

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

    private readonly UserLogin _userLogin = new();
    private bool _showPassword = false;
    private InputType _inputType = InputType.Password;
    private string _passwordIcon = Icons.Material.Filled.Visibility;

    private readonly PatternMask _identificationCardMask = new("00.000.000-a")
    {
        Transformation = (char c) => c.ToString().ToUpperInvariant()[0]
    };

    private async Task HandleLogin()
    {
        ServiceResponse<string>? result = await AuthService.Login(_userLogin);

        if (result.Success)
        {
            LocalStorage.SetItem("authToken", result.Data);
            await AuthenticationStateProvider.GetAuthenticationStateAsync();
            NavigationManager.NavigateTo(ReturnUrl);
        }
        else
            await DialogService.ShowMessageBox("", (MarkupString) $"<div style=\"font-size: 1.1em\">{result.Message}</div>", yesText: "Ok");
    }

    private void ShowHidePassword()
    {
        if (_showPassword)
        {
            _showPassword = false;
            _inputType = InputType.Password;
            _passwordIcon = Icons.Material.Filled.Visibility;
        }
        else
        {
            _showPassword = true;
            _inputType = InputType.Text;
            _passwordIcon = Icons.Material.Filled.VisibilityOff;
        }
    }
}
