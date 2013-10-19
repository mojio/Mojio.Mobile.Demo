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

using Mojio.Events;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.GoogleMaps;
using Android.Graphics.Drawables;
using Android.Content.PM;
using Com.TestFlightApp.Lib;

namespace Mojio.Mobile.Demo.Android
{
    [Activity(Label = "EventMapActivity", LaunchMode = LaunchMode.SingleTop)]
    public class EventMapActivity : EventBaseActivity
    {
        private LatLng _curLocation = new LatLng(49.283226, -123.106644);
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private TextView _deviceInfo, _lastUpdate;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.EventMapLayout);
            InitiateView();

            if (!ConnectedToNetwork)
                return;

            TestFlight.PassCheckpoint("ECreating EventMapActivity");

            ShowDevInfo();
            ShowEvents();
            ShowLastUpdate();
        }

        protected override void OnStart()
        {
            base.OnStart();
            CurContext = this;
        }

        protected override void OnResume()
        {
            base.OnResume();
            ShowEvents();
        }

        protected override void OnMojioEventReceived(Event eve)
        {
            base.OnMojioEventReceived(eve);
            ShowEvents();
            ShowLastUpdate();
        }

        private void InitiateView()
        {
            var clearButton = FindViewById<Button>(Resource.Id.ClearButton);
            clearButton.Click += (o, e) =>{ 
                ClearMojioEvents();
                ShowEvents();
                ShowLastUpdate(true);
            };

            InitMapFragment();

            _map = _mapFragment.Map;
            _deviceInfo = FindViewById<TextView>(Resource.Id.DeviceInfo);
            _lastUpdate = FindViewById<TextView>(Resource.Id.LastUpdateLabel);
        }

        private void InitMapFragment()
        {
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    //.InvokeMapType(GoogleMap.MapTypeSatellite)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.DeviceEventsMap, _mapFragment, "map");
                fragTx.Commit();
            }
        }

        private void ShowEvents()
        {
            if (_map == null)
            {
                if (_mapFragment.Map == null)
                    return;
                _map = _mapFragment.Map;
            }
            _map.Clear();

            if (Dev != null)
            {
                List<Event> events = ReceivedEvents;
                PolylineOptions pathOptions = new PolylineOptions();
                if (events != null)
                    foreach (Event eve in events)
                        if (eve is TripEvent)
                        {
                            var tripEvent = eve as TripEvent;

							// Make sure event has a location
							if (tripEvent.Location != null && !tripEvent.Location.IsEmpty ()) {
								Log.Verbose ("Adding event location to map");

								_curLocation = new LatLng (tripEvent.Location.Lat, tripEvent.Location.Lng);
								pathOptions.Add (new LatLng (tripEvent.Location.Lat, tripEvent.Location.Lng));
							}
                        }

                // add polygon line for path
                Polyline polyline = _map.AddPolyline(pathOptions);

                // add marker for current location
                MarkerOptions marker = new MarkerOptions();
                marker.SetPosition(_curLocation);
                _map.AddMarker(marker);

                // move map to current location
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(_curLocation, 18);
                _map.MoveCamera(cameraUpdate);
            }
        }

        private void ShowDevInfo()
        {
            if (_deviceInfo == null)
                return;
            if (Dev != null)
                _deviceInfo.Text = string.Format("Current Device:\t{0} ID:\t{1}\n", Dev.Name, Dev.IdToString);
            else
                _deviceInfo.SetText(Resource.String.DefaultDeviceInfo);
        }

        private void ShowLastUpdate(bool cleared = false)
        {
            _lastUpdate.Text = (cleared ? "Last Cleared:\t":"Last Updated:\t") + DateTime.Now.ToString();
        }

    }
}