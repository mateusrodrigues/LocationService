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
        private string logTag = "Location Service";
        protected LocationManager LocMgr = Android.App.Application.Context.GetSystemService("location") as LocationManager;

        IBinder binder;
        public override IBinder OnBind(Intent intent)
        {
            binder = new LocationServiceBinder(this);
            return binder;
        }

        public void OnLocationChanged(Android.Locations.Location location)
        {
            this.LocationChanged(this, new LocationChangedEventArgs(location));
            Log.Debug(logTag, string.Format("Latitude is {0}", location.Latitude));
            Log.Debug(logTag, string.Format("Longitude is {0}", location.Longitude));
            Log.Debug(logTag, string.Format("Altitude is {0}", location.Altitude));
            Log.Debug(logTag, string.Format("Speed is {0}", location.Speed));
            Log.Debug(logTag, string.Format("Accuracy is {0}", location.Accuracy));
            Log.Debug(logTag, string.Format("Bearing is {0}", location.Bearing));
        }

        public void OnProviderDisabled(string provider)
        {
            Log.Debug(logTag, "Provider disabled");
        }

        public void OnProviderEnabled(string provider)
        {
            Log.Debug(logTag, "Provider enabled");
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Log.Debug(logTag, string.Format("Status changed to: {0}", Enum.GetName(typeof(Availability), status)));
        }

        public void StartLocationUpdates()
        {
            var locationCriteria = new Criteria();
            locationCriteria.Accuracy = Accuracy.NoRequirement;
            locationCriteria.PowerRequirement = Power.NoRequirement;
            var locationProvider = LocMgr.GetBestProvider(locationCriteria, true);
            LocMgr.RequestLocationUpdates(locationProvider, 2000, 0, this);
        }

        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };
    }
}