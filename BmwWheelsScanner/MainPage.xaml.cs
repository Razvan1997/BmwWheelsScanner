using BmwWheelsScanner.Models;
using Maui.DataGrid;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Graphics;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace BmwWheelsScanner
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private ObservableCollection<WheelSpecs> wheelsSpecs;
        public ObservableCollection<WheelSpecs> WheelsSpecs
        {
            get => wheelsSpecs;
            set { SetProperty(ref wheelsSpecs, value); }
        }

        private ObservableCollection<Wheel> wheels;
        public ObservableCollection<Wheel> Wheels
        {
            get => wheels;
            set { SetProperty(ref wheels, value); }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set { SetProperty(ref isBusy, value); }
        }

        private DelegateCommand<Wheel> navigateToDetailsScreen;
        public DelegateCommand<Wheel> NavigateToDetailsScreen =>
            navigateToDetailsScreen ?? (navigateToDetailsScreen = new DelegateCommand<Wheel>(async (wheel) => { await ExecuteNavigateToDetailsScreen(wheel); }));

        public MainPage()
        {
            WheelsSpecs = new ObservableCollection<WheelSpecs>();
            Wheels = new ObservableCollection<Wheel>();
            BindingContext = this;
            InitializeComponent();
            loading.IsBusy = false;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            if (CameraView.Cameras.Count() > 0)
            {
                CameraView.Camera = CameraView.Cameras.First();

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await CameraView.StopCameraAsync();
                    await CameraView.StartCameraAsync();
                });
            }

        }

        private void CameraView_CamerasLoaded(object sender, EventArgs e)
        {
            if (CameraView.Camera == null)
            {
                CameraView.Camera = CameraView.Cameras.First();

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await CameraView.StopCameraAsync();
                    await CameraView.StartCameraAsync();
                });
            }
        }   

        private async void Button_Clicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                outImage.Source = CameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG);

                loading.IsBusy = true;

                byte[] image = await ConvertImageSourceToBytesAsync(outImage.Source);

                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/bson"));
                //https://91.213.76.112:8082/predict
                Uri uri = new Uri(string.Format("http://91.213.76.112:80/WeatherForecast/predict", string.Empty));
                try
                {
                    MediaTypeFormatter bsonFormatter = new BsonMediaTypeFormatter();
                    var base64String = Convert.ToBase64String(image);

                    var request = new
                    {
                        Content = base64String
                    };

                    var jsonRequestData = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await _client.PostAsync(uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var customerJsonString = await response.Content.ReadAsStringAsync();
                        var deserialized = JsonConvert.DeserializeObject<IEnumerable<Wheel>>(custome‌​rJsonString);
                        if (deserialized.Count() > 0)
                        {
                            Wheels.Clear();
                            foreach (Wheel wheel in deserialized)
                            {
                                Wheels.Add(wheel);
                            }
                            lblNotFound.IsVisible = true;
                            lblNotFound.FontSize = 15;
                            lblNotFound.Text = $"Scan completed successfully. Total {Wheels.Count}, including possible matches.";
                        }
                        else
                        {
                            Wheels.Clear();
                            lblNotFound.IsVisible = true;
                            lblNotFound.FontSize = 20;
                            lblNotFound.Text = "Wheel not found.";
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

        public async Task<byte[]> ConvertImageSourceToBytesAsync(ImageSource imageSource)
        {
            Stream stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
            byte[] bytesAvailable = new byte[stream.Length];
            stream.Read(bytesAvailable, 0, bytesAvailable.Length);

            return bytesAvailable;
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

        public async Task ExecuteNavigateToDetailsScreen(Wheel wheel)
        {

            if (wheel != null)
            {
                var navigationParameter = new Dictionary<string, object> { { "WheelStyleName", wheel.WheelStyleName } };
                await Shell.Current.GoToAsync($"//{nameof(WheelDetailsPage)}", navigationParameter);
            }
        }
    }

}
