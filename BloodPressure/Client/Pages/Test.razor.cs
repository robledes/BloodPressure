namespace BloodPressure.Client.Pages;

public partial class Test
{
    [Inject] public IDialogService DialogService { get; set; }

    private async Task Busy()
    {
        DialogParameters? message = new() { ["Message"] = "Había una vez un barquito chiquitito que no podía navegar. Pasaron un dos tres cuatro cinco seis semanas y aquel barquito navegó. Y si ésta historia parece corta volveremos a empezar..." };
        DialogOptions? options = new() { CloseButton = false, NoHeader = true, CloseOnEscapeKey = false, DisableBackdropClick = true };
        DialogReference busyMessage = (DialogReference)DialogService.Show<BusyMessage>(string.Empty, message, options);
        await Task.Delay(10000);
        busyMessage.Close();
    }
}
