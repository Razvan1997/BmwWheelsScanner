using BmwWheelsScanner.Models;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;

namespace BmwWheelsScanner;

public partial class GoogleSearchWheel : ContentPage, IQueryAttributable
{
    private DelegateCommand backCommand;
    public DelegateCommand BackCommand =>
        backCommand ?? (backCommand =
                    new DelegateCommand(ExecuteBackCommand));

    private const string cx = "839ad390234944ff3";
    private const string apiKey = "AIzaSyB4-7FGF34rVgIreEhWMY1r1yN1T0fmeWs";

    private ObservableCollection<GoogleResult> wheelSearches;
    public ObservableCollection<GoogleResult> WheelSearches
    {
        get => wheelSearches;
        set { SetProperty(ref wheelSearches, value); }
    }

    private string searchTitleFor;
    public string SearchTitleFor
    {
        get => searchTitleFor;
        set { SetProperty(ref searchTitleFor, value); }
    }

    private bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        set { SetProperty(ref isBusy, value); }
    }

    public string WheelStyleName { get; set; }

    public GoogleSearchWheel()
    {
        WheelSearches = new ObservableCollection<GoogleResult>();
        BindingContext = this;
        InitializeComponent();
        loading.IsBusy = false;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            loading.IsBusy = true;
            LoadResult(WheelStyleName);
            loading.IsBusy = false;
        });
    }

    public async void ExecuteBackCommand()
    {
        var navigationParameter = new Dictionary<string, object> { { "WheelStyleName", WheelStyleName } };
        await Shell.Current.GoToAsync($"//{nameof(WheelDetailsPage)}", navigationParameter);
    }

    private async Task<WebResponse> MakeRequest(string searchItem)
    {
        try
        {
            var location = await Geolocation.GetLocationAsync();

            if (location != null)
            {
                double latitude = location.Latitude;
                double longitude = location.Longitude;

                // Obțineți numele țării din locația actuală
                var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
                var countryName = placemarks?.FirstOrDefault()?.CountryName;
                string countryFilter = $"site:{countryName.ToLower()}";
                //" after:2023-01-01 " + " cr:countryRO"
                var request = WebRequest.Create("https://www.googleapis.com/customsearch/v1?key=" + apiKey + "&cx=" + cx + "&q=" + searchItem + " BMW" + "&cr=" + countryFilter);
                var response = await request.GetResponseAsync();

                return response;
            }
        }
        catch (Exception ex)
        {

        }
        return null;
    }

    private async void LoadResult(string searchItem)
    {
        HttpWebResponse response = (HttpWebResponse)await MakeRequest(searchItem);

        Stream data = response.GetResponseStream();
        StreamReader reader = new StreamReader(data);
        string responseStream = reader.ReadToEnd();

        dynamic jsonData = JsonConvert.DeserializeObject(responseStream);


        foreach (var item in jsonData.items)
        {
            WheelSearches.Add(new GoogleResult
            {
                Title = item.title,
                Link = item.link,
                Snippet = item.snippet,
            });
        }
    }

    protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

        storage = value;
        RaisePropertyChanged(propertyName);

        return true;
    }

    protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        PropertyChanged?.Invoke(this, args);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        WheelSearches.Clear();

        WheelStyleName = query["WheelStyleName"] as string;

        SearchTitleFor = $"Searching details on internet for {WheelStyleName}";
        lblSearch.Text = SearchTitleFor;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var tappedLabel = (Label)sender;
        var tappedItem = (GoogleResult)tappedLabel.BindingContext;

        if (tappedItem != null)
        {
            await Launcher.OpenAsync(new Uri(tappedItem.Link));
        }

    }
}