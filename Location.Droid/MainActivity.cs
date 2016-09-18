using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Location.Droid.Services;
using Android.Locations;

namespace Location.Droid
{
    [Activity(Label = "LocationDroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        readonly string logTag = "MainActivity";

        // Make our labels
        TextView status;
        TextView latText;
        TextView longText;
        TextView altText;
        TextView speedText;
        TextView bearText;
        TextView accText;

        // Program intent button
        Button mapsButton;

        // Location information
        double latitude, longitude;

        #region Lifecycle

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug(logTag, "OnCreate: Location app is becoming active");

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // This event fires when the ServiceConnection lets the client (our App class) know that
            // the Service is connected. We use this event to start updating the UI with location
            // updates from the Service
            App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
            {
                Log.Debug(logTag, "ServiceConnected Event raised");
                // Notifies us of location changes from the system
                App.Current.LocationService.LocationChanged += HandleLocationChanged;
                // Notifies us of user changes to the location provider (i.e. the user disabled or enabled GPS)
                App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
                App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
                // Notifies us of the changing status of a provider (i.e. GPS no longer available)
                App.Current.LocationService.StatusChanged += HandleStatusChanged;
            };

            // 'Open in Maps' button on view
            mapsButton = FindViewById<Button>(Resource.Id.btnOpenMaps);
            // Create intent when button is clicked
            mapsButton.Click += delegate
            {
                var streetViewUri = Android.Net.Uri.Parse($"google.streetview:cbll={latitude},{longitude}&cbp=1,90,,0,1.0&mz=20");
                var streetViewIntent = new Intent(Intent.ActionView, streetViewUri);
                StartActivity(streetViewIntent);
            };

            status = FindViewById<TextView>(Resource.Id.status);
            latText = FindViewById<TextView>(Resource.Id.lat);
            longText = FindViewById<TextView>(Resource.Id.longx);
            altText = FindViewById<TextView>(Resource.Id.alt);
            speedText = FindViewById<TextView>(Resource.Id.speed);
            bearText = FindViewById<TextView>(Resource.Id.bear);
            accText = FindViewById<TextView>(Resource.Id.acc);

            altText.Text = "altitude";
            speedText.Text = "speed";
            bearText.Text = "bearing";
            accText.Text = "accuracy";

            // Start the location service:
            App.StartLocationService();
        }

        protected override void OnPause()
        {
            Log.Debug(logTag, "OnPause: Location app is moving to the background");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Log.Debug(logTag, "OnResume: Location app is moving into foreground");
            base.OnResume();
        }

        protected override void OnDestroy()
        {
            Log.Debug(logTag, "OnDestroy: Location app is becoming inactive");
            base.OnDestroy();

            // Stop the location service:
            App.StopLocationService();
        }

        #endregion

        #region Android Location Service methods

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            Android.Locations.Location location = e.Location;
            Log.Debug(logTag, "Foreground updating");

            // These events are on a background thread, need to update on the UI thread
            RunOnUiThread(() =>
            {
                mapsButton.Enabled = true;

                status.Visibility = ViewStates.Gone;

                latText.Text = String.Format("Latitude: {0}", location.Latitude);
                latitude = location.Latitude;

                longText.Text = String.Format("Longitude: {0}", location.Longitude);
                longitude = location.Longitude;

                altText.Text = String.Format("Altitude: {0}", location.Altitude);
                speedText.Text = String.Format("Speed: {0}", location.Speed);
                accText.Text = String.Format("Accuracy: {0}", location.Accuracy);

                bearText.Text = String.Format("Bearing: {0}", location.Bearing);
            });
        }

        public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            Log.Debug(logTag, "Location provider disabled event raised");
        }

        public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            Log.Debug(logTag, "Location provider enabled event raised");
        }

        public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Log.Debug(logTag, "Location status changed event raised");
        }

        #endregion
    }
}

