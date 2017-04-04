using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Media.Capture;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Phone.UI.Input;
using Windows.System.Display;
using Windows.Devices.Geolocation;
using System.Threading;
using Windows.Media;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SelfieLocation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        // Captures the value entered by user.
        private uint _desireAccuracyInMetersValue = 0;
        private CancellationTokenSource _cts = null;

        public BlankPage1()
        {
            this.InitializeComponent();
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            double latitude = 53.2740967;
            double longitude = -9.0495774;
            try
            {
                // Request permission to access location
                var accessStatus = await Geolocator.RequestAccessAsync();
                Geoposition pos = null;
                
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:

                        // Get cancellation token
                        _cts = new CancellationTokenSource();
                        CancellationToken token = _cts.Token;

                        //_rootPage.NotifyUser("Waiting for update...", NotifyType.StatusMessage);

                        // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                        Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = _desireAccuracyInMetersValue };

                        // Carry out the operation
                        pos = await geolocator.GetGeopositionAsync().AsTask(token);
                        latitude = pos.Coordinate.Latitude;
                        longitude = pos.Coordinate.Longitude;




                        // UpdateLocationData(pos);
                        //   _rootPage.NotifyUser("Location updated.", NotifyType.StatusMessage);
                        break;

                    case GeolocationAccessStatus.Denied:

                        break;

                    case GeolocationAccessStatus.Unspecified:

                        break;


                }
            }
            catch (TaskCanceledException)
            {
                // _rootPage.NotifyUser("Canceled.", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                // _rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
            finally
            {
                _cts = null;
            }

            HyperLink1.Content = String.Format( "Lat= {0:N3},Long= {1:N3}" ,latitude,longitude);

            // Specify a known location.
            Windows.Devices.Geolocation.BasicGeoposition cityPosition = new Windows.Devices.Geolocation.BasicGeoposition() { Latitude = latitude, Longitude = longitude };
            Windows.Devices.Geolocation.Geopoint cityCenter = new Windows.Devices.Geolocation.Geopoint(cityPosition);

            // Set the map location.
            MapControl1.Center = cityCenter;
            MapControl1.ZoomLevel = 12;
            MapControl1.LandmarksVisible = true;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Location location = new Location(MapControl1.Center.Position.Latitude, MapControl1.Center.Position.Longitude);
            this.Frame.Navigate(typeof(MainPage),location);
        }

        private void MapControl1_CenterChanged(MapControl sender, object args)
        {
            HyperLink1.Content = String.Format("Lat= {0:N3},Long= {1:N3}", sender.Center.Position.Latitude, sender.Center.Position.Longitude);
            

        }
    }
}
