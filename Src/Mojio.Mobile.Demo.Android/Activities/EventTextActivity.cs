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
using Android.Text.Method;
using Mojio.Events;
using PushSharp.Client;
using Android.Content.PM;
using Com.TestFlightApp.Lib;

namespace Mojio.Mobile.Android.Test
{
    [Activity(Label = "EventTextActivity", LaunchMode = LaunchMode.SingleTop)]
    public class EventTextActivity : EventBaseActivity
    {
        private TextView _deviceInfo, _deviceEvents, _lastUpdate;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.EventTextLayout);
            InitiateView();

            if (!ConnectedToNetwork)
                return;

            TestFlight.PassCheckpoint("ECreating EventTextActivity");

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

        protected void InitiateView()
        {
            var clearButton = FindViewById<Button>(Resource.Id.ClearButton);
            clearButton.Click += (o, e) =>
            {
                ClearMojioEvents();
                ShowEvents();
                ShowLastUpdate(true);
            };
            Button mapView = FindViewById<Button>(Resource.Id.MapViewButton);
            mapView.Click += (o, e) =>
            {
                var map = new Intent(this, typeof(EventMapActivity));
                StartActivity(map);
            };

            _deviceInfo = FindViewById<TextView>(Resource.Id.DeviceInfo);
            _deviceEvents = FindViewById<TextView>(Resource.Id.DeviceEvents);
            _lastUpdate = FindViewById<TextView>(Resource.Id.LastUpdateLabel);
            _deviceEvents.MovementMethod = new ScrollingMovementMethod();
        }

        private void ShowEvents()
        {
            if (_deviceEvents == null)
                return;
            if (Dev != null)
            {
                string info = "All events:\n Event type   \t\t\tTime\n";
                List<Event> events = ReceivedEvents;
                if (events != null)
                    foreach (Event eve in events)
                        info += string.Format("{0, -13}\t{1}\n", eve.EventType, eve.Time);
                _deviceEvents.Text = info;
            }
            else
                _deviceEvents.SetText(Resource.String.DefaultDeviceEvents);
        }

        // Reset displayed text if no device selected
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
            _lastUpdate.Text = (cleared ? "Last Cleared:\t" : "Last Updated:\t") + DateTime.Now.ToString();
        }
    }
}