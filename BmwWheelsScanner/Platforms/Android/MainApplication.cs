using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;


namespace BmwWheelsScanner
{
#if DEBUG                                   // connect to local service on the
    [Application(UsesCleartextTraffic = true)]  // emulator's host for debugging,
#else                                       // access via 
[Application(UsesCleartextTraffic = true)]                              
#endif

    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
