using System.Security.Claims;

namespace BloodPressure.Client.Pages;

partial class Admin
{
    [Inject] private IAuthService AuthService { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    private ClaimsPrincipal _currentUser;
    private IEnumerable<PatientsList>? _patientsList = new List<PatientsList>();
    private bool _dataLoaded;
    private string _searchString;

    private readonly DialogOptions _dialogOptions = new()
    {
        CloseOnEscapeKey = true,
        DisableBackdropClick = true,
        CloseButton = false,
        NoHeader = true
    };

    private Func<PatientsList, bool> QuickFilter => field =>
    {
        return string.IsNullOrWhiteSpace(_searchString) ||
               field.FullName.Contains(_searchString, StringComparison.OrdinalIgnoreCase);
    };

    protected override async Task OnParametersSetAsync()
        => _currentUser = (await AuthenticationStateTask).User;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_currentUser.IsInRole("A") && !_dataLoaded)
        {
            ServiceResponse<IEnumerable<PatientsList>> patientsListResult = await AuthService.GetAll();

            if (patientsListResult.Success)
                _patientsList = patientsListResult.Data;

            _dataLoaded = true;
            StateHasChanged();
        }
    }

    private void HandleLogin()
    {
        DialogParameters? returnUrl = new() { ["ReturnUrl"] = NavigationManager.ToBaseRelativePath(NavigationManager.Uri) };
        DialogService.Show<Login>(string.Empty, returnUrl, _dialogOptions);
    }

    private void ViewPatientDetails(int id)
    {
        DialogParameters? patientId = new() { ["PatientId"] = id };
        DialogService.Show<PatientDetails>(string.Empty, patientId, _dialogOptions);
    }

}
