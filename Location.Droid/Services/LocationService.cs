using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using Android.Util;

namespace Location.Droid.Services
{
    [Service]
    public class LocationService : Service, ILocationListener
    {
        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };
        public event EventHandler<ProviderDisabledEventArgs> ProviderDisabled = delegate { };
        public event EventHandler<ProviderEnabledEventArgs> ProviderEnabled = delegate { };
        public event EventHandler<StatusChangedEventArgs> StatusChanged = delegate { };

        public LocationService()
        {

        }

        protected LocationManager LocMgr = Android.App.Application.Context.GetSystemService("location") as LocationManager;

        readonly string logTag = "LocationService";
        IBinder binder;

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(logTag, "OnCreate called in the LocationService");
        }

        // This gets called when StartService is called in our App class
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        // This gets called once, the first time any client binds to the service
        // and returns an instance of the LocationServiceBinder. All future clients will
        // reuse the same instance of the binder
        public override IBinder OnBind(Intent intent)
        {
            binder = new LocationServiceBinder(this);
            return binder;
        }

        // Handle location updates from the location manager
        public void StartLocationUpdates()
        {
            // We can set a different location criteria based on requirements for our app
            // For example, we might want to preserve power, of the extreme accuracy
            var locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.NoRequirement;
            locationCriteria.PowerRequirement = Power.NoRequirement;

            // Get provider: GPS, Network, etc.
            var locationProvider = LocMgr.GetBestProvider(locationCriteria, true);
            Log.Debug(logTag, string.Format("You are about to get location updates via {0}", locationProvider));

            // Get an initial fix on location
            LocMgr.RequestLocationUpdates(locationProvider, 2000, 0, this);

            Log.Debug(logTag, "Now sending location updates");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug(logTag, "Service has been terminated");

            // Stop getting updates from the location manager
            LocMgr.RemoveUpdates(this);
        }

        #region ILocationListener implementation

        // ILocationListener is a way for the service to subscribe for updates
        // from the System location service

        public void OnLocationChanged(Android.Locations.Location location)
        {
            this.LocationChanged(this, new LocationChangedEventArgs(location));

            // This should be updating every time we request new location updates
            // both when the app is in the background, and in the foreground
            Log.Debug(logTag, string.Format("Latitude is {0}", location.Latitude));
            Log.Debug(logTag, string.Format("Longitude is {0}", location.Longitude));
            Log.Debug(logTag, string.Format("Altitude is {0}", location.Altitude));
            Log.Debug(logTag, string.Format("Speed is {0}", location.Speed));
            Log.Debug(logTag, string.Format("Accuracy is {0}", location.Accuracy));
            Log.Debug(logTag, string.Format("Bearing is {0}", location.Bearing));
        }

        public void OnProviderDisabled(string provider)
        {
            this.ProviderDisabled(this, new ProviderDisabledEventArgs(provider));
        }

        public void OnProviderEnabled(string provider)
        {
            this.ProviderEnabled(this, new ProviderEnabledEventArgs(provider));
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            this.StatusChanged(this, new StatusChangedEventArgs(provider, status, extras));
        }

        #endregion
    }
}