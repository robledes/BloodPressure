using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BloodPressure.Client.Pages;

public partial class Register
{
    [Inject] private IAuthService? AuthService { get; set; }
    [Inject] private IDialogService? DialogService { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }

    [Parameter] public string ReturnUrl { get; set; }

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

    private readonly UserRegister _userRegister = new();
    private string? _photoImage = null;
    private byte[]? _streamPhoto;

    private readonly PatternMask _identificationCardMask = new("00.000.000-a")
    {
        Transformation = (char c) => c.ToString().ToUpperInvariant()[0]
    };


    private async Task HandleRegistration()
    {
        if (_photoImage != null)
        {
            DialogParameters? message = new() { ["Message"] = "Resizing photo, please wait..." };
            DialogOptions? options = new() { CloseButton = false, NoHeader = true, CloseOnEscapeKey = false, DisableBackdropClick = true };
            DialogReference busyMessage = (DialogReference)DialogService.Show<BusyMessage>(string.Empty, message, options);
            await Task.Delay(500);

            using Image? image = Image.Load(_streamPhoto);
            image.Mutate(c => c.Resize(0, 130));
            MemoryStream outStream = new();
            await image.SaveAsJpegAsync(outStream);
            _userRegister.Photo = outStream.ToArray();

            busyMessage.Close();
        }
        
        ServiceResponse<int>? result = await AuthService.Register(_userRegister);
        await DialogService.ShowMessageBox("", (MarkupString)$"<div style=\"font-size: 1.1em\">{result.Message}</div>", yesText: "Ok");

        if (result.Success)
            NavigationManager.NavigateTo(ReturnUrl);
    }

    private async Task SelectPhoto(InputFileChangeEventArgs e)
    {
        _streamPhoto = new byte[e.File.Size];
        await e.File.OpenReadStream(e.File.Size).ReadAsync(_streamPhoto);
        _photoImage = Convert.ToBase64String(_streamPhoto);
    }
}
