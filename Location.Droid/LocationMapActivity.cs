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
using Android.Gms.Maps;
using Android.Util;
using Android.Gms.Maps.Model;
using System.Threading;
using System.Threading.Tasks;
using Android.Locations;
using Location.Droid.Services;

namespace Location.Droid
{
    [Activity(Label = "Location Map")]
    public class LocationMapActivity : Activity
    {
        readonly string logTag = "LocationMapActivity";

        private GoogleMap _map;
        private MapFragment _mapFragment;

        private bool _gettingMap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Log.Debug(logTag, "Activity being created");

            // Create your application here
            SetContentView(Resource.Layout.LocationMap);

            InitMapFragment();

            App.Current.LocationService.LocationChanged += HandleLocationChanged;
        }

        protected override void OnPause()
        {
            base.OnPause();

            App.Current.LocationService.LocationChanged -= HandleLocationChanged;
        }

        private void InitMapFragment()
        {
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (_mapFragment == null)
            {
                GoogleMapOptions options = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true)
                    .InvokeMapToolbarEnabled(true);

                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(options);
                transaction.Add(Resource.Id.map, _mapFragment, "map");
                transaction.Commit();

                if (null != _map || _gettingMap) return;

                var mapReadyCallback = new OnMapReadyClass();
                mapReadyCallback.MapReady += (sender, args) =>
                {
                    _gettingMap = false;
                    _map = mapReadyCallback.Map;
                };

                _gettingMap = true;
                _mapFragment.GetMapAsync(mapReadyCallback);
            }
        }

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            Log.Debug(logTag, "Location Changed");
            Android.Locations.Location location = e.Location;

            if (_map != null)
            {
                _map.Clear();

                MarkerOptions markerOpt = new MarkerOptions();
                markerOpt.SetPosition(new LatLng(location.Latitude, location.Longitude));
                markerOpt.SetTitle("Your position");
                markerOpt.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue));
                _map.AddMarker(markerOpt);
            }
        }
    }
}