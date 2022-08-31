namespace BloodPressure.Client.Pages;

public partial class PatientDetails
{
    [Inject] private IAuthService AuthService { get; set; }
    [Inject] private IDialogService? DialogService { get; set; }

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

    [Parameter] public int PatientId { get; set; }

    private Patient? _patient;
    private string? _photoImage;
    private bool _isDisabled = true;

    protected override async Task OnParametersSetAsync()
    {
        ServiceResponse<Patient>? patientResult = await AuthService.Get(PatientId);
        _patient = patientResult.Data;
        ServiceResponse<Photo>? photoResult = await AuthService.GetPhoto(PatientId);

        if (photoResult.Success)
            _photoImage = Convert.ToBase64String(photoResult.Data.PhotoImage);
    }

    private async Task SaveChanges()
    {
        ServiceResponse<bool>? result = await AuthService.Update(_patient);
        await DialogService.ShowMessageBox("", (MarkupString)$"<div style=\"font-size: 1.1em\">{result.Message}</div>", yesText: "Ok");
        _isDisabled = true;
    }
}
