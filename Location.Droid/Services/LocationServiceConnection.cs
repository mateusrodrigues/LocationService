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
using Android.Util;

namespace Location.Droid.Services
{
    public class LocationServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public event EventHandler<ServiceConnectedEventArgs> ServiceConnected = delegate { };

        protected LocationServiceBinder binder;
        public LocationServiceBinder Binder
        {
            get { return this.binder; }
            set { this.binder = value; }
        }

        public LocationServiceConnection(LocationServiceBinder binder)
        {
            if (binder != null)
            {
                this.binder = binder;
            }
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            LocationServiceBinder serviceBinder = service as LocationServiceBinder;

            if (serviceBinder != null)
            {
                this.binder = serviceBinder;
                this.binder.IsBound = true;

                // Raise the service bound event
                this.ServiceConnected(this, new ServiceConnectedEventArgs() { Binder = service });

                // Begin updating the location in the service
                serviceBinder.Service.StartLocationUpdates();
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            this.binder.IsBound = false;
        }
    }
}