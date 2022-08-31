namespace BloodPressure.Client.Pages;

partial class SaveMeasurement
{
    [Inject] private IMeasurementService? MeasurementService { get; set; }

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

    [Parameter] public int Id { get; set; }

    private Measurement _measurement = new();
    private string _title = string.Empty;

    protected override async Task OnParametersSetAsync()
    {
        _title = "Add measurement";

        if (Id > 0)
        {
            ServiceResponse<Measurement>? response = await MeasurementService.Get(Id);

            if (response.Data != null)
            {
                _measurement = response.Data;
                _title = "Edit measurement";
            }
            else
                Id = 0;
        }
    }

    private async Task Save()
    {
        if (Id > 0)
            await MeasurementService.Update(_measurement);
        else
            await MeasurementService.Insert(_measurement);

        MudDialog.Close();
    }
}
