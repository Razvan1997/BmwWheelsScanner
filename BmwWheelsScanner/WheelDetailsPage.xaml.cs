
using BmwWheelsScanner.Models;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace BmwWheelsScanner;

[QueryProperty(nameof(WheelStyleName), "WheelStyleName")]
public partial class WheelDetailsPage : ContentPage, IQueryAttributable
{
    private ObservableCollection<WheelSpecs> wheelSpecification;
    public ObservableCollection<WheelSpecs> WheelSpecification
    {
        get => wheelSpecification;
        set { SetProperty(ref wheelSpecification, value); }
    }

    private ObservableCollection<ThumbNailModel> images;
    public ObservableCollection<ThumbNailModel> Images
    {
        get => images;
        set { SetProperty(ref images, value); }
    }
    public string WheelStyleName { get; set; }
    private bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        set { SetProperty(ref isBusy, value); }
    }

    private DelegateCommand backCommand;
    public DelegateCommand BackCommand =>
        backCommand ?? (backCommand =
                    new DelegateCommand(ExecuteBackCommand));

    public WheelDetailsPage()
    {
        Images = new ObservableCollection<ThumbNailModel>();
        WheelSpecification = new ObservableCollection<WheelSpecs>();
        BindingContext = this;
        InitializeComponent();
    }

    private async void LoadImages()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            loading.IsBusy = true;
            HttpClient _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            Uri uri = new Uri(string.Format("http://91.213.76.112:80/WeatherForecast/Images", string.Empty));

            try
            {
                //var style = new WheelStyleName()
                //{
                //    style = WheelStyleName,
                //};

                // Converteste obiectul în format JSON
                var jsonRequestData = Newtonsoft.Json.JsonConvert.SerializeObject(WheelStyleName);
                var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;

                response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var customerJsonString = await response.Content.ReadAsStringAsync();
                    var deserialized = JsonConvert.DeserializeObject<IEnumerable<ThumbNailModel>>(custome‌​rJsonString);
                    if (deserialized.Count() > 0)
                    {
                        Images.Clear();
                        foreach (ThumbNailModel wheel in deserialized)
                        {
                            Images.Add(wheel);
                        }
                    }
                    else
                    {
                        Images.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            finally
            {
                loading.IsBusy = false;
            }
        });
    }

    private async void LoadSpecs()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            loading.IsBusy = true;
            HttpClient _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            Uri uri = new Uri(string.Format("http://91.213.76.112:80/WeatherForecast/Specs", string.Empty));

            try
            {
                //var style = new WheelStyleName()
                //{
                //    style = WheelStyleName,
                //};

                // Converteste obiectul în format JSON
                var jsonRequestData = Newtonsoft.Json.JsonConvert.SerializeObject(WheelStyleName);
                var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;

                response = await _client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var customerJsonString = await response.Content.ReadAsStringAsync();
                    var deserialized = JsonConvert.DeserializeObject<IEnumerable<WheelSpecs>>(custome‌​rJsonString);
                    if (deserialized.Count() > 0)
                    {
                        WheelSpecification.Clear();
                        foreach (WheelSpecs wheel in deserialized)
                        {
                            WheelSpecification.Add(wheel);
                        }
                    }
                    else
                    {
                        WheelSpecification.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            finally
            {
                loading.IsBusy = false;
                lblSpecs.IsVisible = true;
                btnViewInfo.IsVisible = true;
            }
        });
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

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        WheelStyleName = query["WheelStyleName"] as string;
        LoadImages();
        LoadSpecs();
    }

    public async void ExecuteBackCommand()
    {
        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object> { { "WheelStyleName", WheelStyleName } };

        Shell.Current.GoToAsync($"//{nameof(GoogleSearchWheel)}", navigationParameter);
    }
}