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

namespace Location.Droid.Services
{
    public class LocationServiceBinder : Binder
    {
        protected LocationService service;
        public LocationService Service
        {
            get { return this.service; }
        }

        public bool IsBound { get; set; }

        public LocationServiceBinder(LocationService service)
        {
            this.service = service;
        }
    }
}