using System.Security.Claims;

namespace BloodPressure.Client.Pages;

public partial class Measurements
{
    [Inject] private IMeasurementService MeasurementService { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] private ISyncLocalStorageService LocalStorage { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    private IEnumerable<Measurement>? _measurements = Enumerable.Empty<Measurement>();
    private ServiceResponse<IEnumerable<Measurement>> _measurementsResult;
    private bool _dataUpdated;

    private List<ChartSeries> _chartSeries;
    private readonly string[] _chartSeriesName = new string[3];
    private string[] _xAxisLabels;

    private readonly DialogOptions _dialogOptions = new()
    {
        CloseOnEscapeKey = true,
        DisableBackdropClick = true,
        CloseButton = false,
        NoHeader = true
    };

    protected override async Task OnInitializedAsync()
    {
        if (!await TokenExpired())
            await UpdateList();
    }

    private async Task Save(int id)
    {
        DialogParameters? parameter = new() { ["Id"] = id };
        DialogResult result = await DialogService.Show<SaveMeasurement>(string.Empty, parameter, _dialogOptions).Result;

        if (!result.Cancelled)
            await UpdateList();
    }

    private async Task Delete(int id, string date, string time)
    {
        if ((bool)await DialogService.ShowMessageBox("",
            (MarkupString)$"<div style=\"font-size: 1.1em; text-align: center\">Measurement on<br />{date}  {time}<br />will be deleted.<br /><br />Are you sure?</div>",
            noText: "Cancel", yesText: "Yes"))
        {
            await MeasurementService.Delete(id);
            await UpdateList();
        }
    }

    private async Task UpdateList()
    {
        _dataUpdated = false;
        _measurements = Enumerable.Empty<Measurement>();
        _measurementsResult = await MeasurementService.GetAll();

        if (_measurementsResult.Success)
        {
            _measurements = _measurementsResult.Data;
            FillChartSeries();
        }

        _dataUpdated = true;
        StateHasChanged();
    }

    private void FillChartSeries()
    {
        int size = _measurements.Count();

        double[] chartSeriesSystolic = new double[size];
        double[] chartSeriesDiastolic = new double[size];
        double[] chartSeriesPulse = new double[size];
        _xAxisLabels = new string[size];
        int i = 0;

        foreach (Measurement measurement in _measurements)
        {
            chartSeriesSystolic[i] = (double)measurement.Systolic;
            chartSeriesDiastolic[i] = (double)measurement.Diastolic;
            chartSeriesPulse[i] = (double)measurement.Pulse;
            _xAxisLabels[i] = measurement.DayMonthOnly;
            i++;
        }

        _chartSeriesName[0] = "SYSTOLIC : " +
                              chartSeriesSystolic.Min().ToString() + "-" +
                              chartSeriesSystolic.Max().ToString() + " (" +
                              ((int)chartSeriesSystolic.Average()).ToString() + ")";

        _chartSeriesName[1] = "DIASTOLIC : " +
                              chartSeriesDiastolic.Min().ToString() + "-" +
                              chartSeriesDiastolic.Max().ToString() + " (" +
                              ((int)chartSeriesDiastolic.Average()).ToString() + ")";

        _chartSeriesName[2] = "PULSE : " +
                              chartSeriesPulse.Min().ToString() + "-" +
                              chartSeriesPulse.Max().ToString() + " (" +
                              ((int)chartSeriesPulse.Average()).ToString() + ")";

        _chartSeries = new()
        {
            new ChartSeries() { Name = _chartSeriesName[0], Data = chartSeriesSystolic },
            new ChartSeries() { Name = _chartSeriesName[1], Data = chartSeriesDiastolic },
            new ChartSeries() { Name = _chartSeriesName[2], Data = chartSeriesPulse }
        };
    }

    private async Task<bool> TokenExpired()
    {
        AuthenticationState? authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        IEnumerable<Claim> _claims = authState.User.Claims;

        DateTime _tokenExpiryDate = DateTime.Parse("1970-01-01").AddSeconds(int.Parse
            (_claims.FirstOrDefault(c => c.Type == "exp").Value)).ToLocalTime();

        if (DateTime.Now > _tokenExpiryDate)
        {
            Snackbar.Add(
                Helpers.MsgFormat("Authentication is expired.", "Please login again."),
                Severity.Warning,
                config => { config.Icon = Icons.Outlined.Login; }
            );

            LocalStorage.RemoveItem("authToken");
            await AuthenticationStateProvider.GetAuthenticationStateAsync();
            NavigationManager.NavigateTo("");
            return true;
        }

        return false;
    }
}

