using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Security.Claims;

namespace BloodPressure.Client.Pages;

public partial class Profile
{
    [Inject] private IAuthService AuthService { get; set; }
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] private ISyncLocalStorageService? LocalStorage { get; set; }
    [Inject] private IDialogService? DialogService { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    [Parameter] public string ReturnUrl { get; set; }

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

    private Patient? _patient = null;
    private Photo? _photo = null;
    private string? _photoImage = null;
    private byte[]? _streamPhoto = null;
    private string? _patientId;

    private readonly PatternMask _identificationCardMask = new("00.000.000-a")
    {
        Transformation = (char c) => c.ToString().ToUpperInvariant()[0]
    };


    protected override async Task OnInitializedAsync()
    {
        AuthenticationState? authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _patientId = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        ServiceResponse<Patient>? patientResult = await AuthService.Get();
        _patient = patientResult.Data;
        ServiceResponse<Photo>? photoResult = await AuthService.GetPhoto();

        if (photoResult.Success)
            _photoImage = Convert.ToBase64String(photoResult.Data.PhotoImage);
    }

    private async Task SaveChanges()
    {
        if (_photoImage != null)
        {
            if (_streamPhoto != null)
            {
                DialogParameters? message = new() { ["Message"] = "Resizing photo, please wait..." };
                DialogOptions? options = new() { CloseButton = false, NoHeader = true, CloseOnEscapeKey = false, DisableBackdropClick = true };
                DialogReference busyMessage = (DialogReference)DialogService.Show<BusyMessage>(string.Empty, message, options);
                await Task.Delay(500);

                using Image? image = Image.Load(_streamPhoto);
                image.Mutate(c => c.Resize(0, 130));
                MemoryStream outStream = new();
                await image.SaveAsJpegAsync(outStream);

                _photo = new()
                {
                    Id = 0,
                    PhotoImage = outStream.ToArray(),
                    PatientId = int.Parse(_patientId)
                };

                ServiceResponse<Photo> photoExists = await AuthService.GetPhoto();

                if (photoExists.Success)
                    await AuthService.UpdatePhoto(_photo);
                else
                    await AuthService.InsertPhoto(_photo);

                busyMessage.Close();
            }
        }
        else
            await AuthService.DeletePhoto();


        ServiceResponse<bool>? result = await AuthService.Update(_patient);
        await DialogService.ShowMessageBox("", (MarkupString)$"<div style=\"font-size: 1.1em\">{result.Message}</div>", yesText: "Ok");
    }

    private async Task Delete()
    {
        if ((bool)await DialogService.ShowMessageBox("",
            (MarkupString)$"<div style=\"font-size: 1.1em; text-align: center\">Patient <b>#{_patientId}</b> will be deleted.<br /> Are you sure?</div>",
            noText: "Cancel", yesText: "Yes"))
        {
            ServiceResponse<bool> result = await AuthService.Delete();
            LocalStorage.RemoveItem("authToken");
            await AuthenticationStateProvider.GetAuthenticationStateAsync();
            NavigationManager.NavigateTo(ReturnUrl);

            Snackbar.Add(
                result.Message,
                Severity.Success,
                config => { config.Icon = Icons.Outlined.DeleteForever; }
            );
        }
    }

    private async Task SelectPhoto(InputFileChangeEventArgs e)
    {
        _streamPhoto = new byte[e.File.Size];
        await e.File.OpenReadStream(e.File.Size).ReadAsync(_streamPhoto);
        _photoImage = Convert.ToBase64String(_streamPhoto);
    }
}
