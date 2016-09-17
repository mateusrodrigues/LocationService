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
using Location.Droid.Services;
using Android.Util;
using System.Threading.Tasks;

namespace Location.Droid
{
    public class App
    {
        // Events
        public event EventHandler<ServiceConnectedEventArgs> LocationServiceConnected = delegate { };

        // Declarations
        protected readonly string logTag = "App";
        protected static LocationServiceConnection locationServiceConnection;

        // Properties
        private static App current;
        public static App Current
        {
            get { return current; }
        }

        public LocationService LocationService
        {
            get
            {
                if (locationServiceConnection.Binder == null)
                {
                    throw new Exception("Service not bound yet");
                }

                // Note that we use the ServiceConnection to get the Binder, and the Binder to get the Service here
                return locationServiceConnection.Binder.Service;
            }
        }

        #region Application context

        static App()
        {
            current = new App();
        }

        protected App()
        {
            // Create a new service connection so we can get a binder to the service
            locationServiceConnection = new LocationServiceConnection(null);

            // This event will fire when the Service connects in the OnServiceConnected call
            locationServiceConnection.ServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
            {
                Log.Debug(logTag, "Service Connected");
                // We will use this event to notify MainActivity when to start updating the UI
                this.LocationServiceConnected(this, e);
            };
        }

        public static void StartLocationService()
        {
            // Starting a service like this is blocking, so we want to do it on a background thread
            new Task(() =>
            {
                // Start our main service
                Log.Debug("App", "Calling StartService");
                Android.App.Application.Context.StartService(new Intent(Android.App.Application.Context, typeof(LocationService)));

                // Bind our service (Android goes and finds the running service by type, and puts a reference
                // on the binder to that service)
                // The intent tells the OS where to find our Service (the Context) and tye Type of Service
                // we're looking for (LocationService)
                Intent locationServiceIntent = new Intent(Android.App.Application.Context, typeof(LocationService));
                Log.Debug("App", "Calling service binding");

                // Finally, we can bing to the Service using our Intent and the ServiceConnection we
                // created in a previous step
                Android.App.Application.Context.BindService(locationServiceIntent, locationServiceConnection, Bind.AutoCreate);
            }).Start();
        }

        public static void StopLocationService()
        {
            // Check for nulls in case StartLocationService task has not yet completed
            Log.Debug("App", "Stop LocationService");

            if (locationServiceConnection != null)
            {
                // Unbind from the LocationService; otherwise, StopSelf (below) will not work:
                Log.Debug("App", "Unbinding from LocationService");
                Android.App.Application.Context.UnbindService(locationServiceConnection);
            }

            // Stop the LocationService:
            if (Current.LocationService != null)
            {
                Log.Debug("App", "Stopping the LocationService");
                Current.LocationService.StopSelf();
            }
        }

        #endregion
    }
}