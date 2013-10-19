using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Java.Util;

namespace Mojio.Mobile.Android.Test
{
    [Activity(Label = "CalendarEventListActivity")]
    public class CalendarEventListActivity : ListActivity
    {
        int _calId;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.CalEventList);

            _calId = Intent.GetIntExtra("calId", -1);

            ListEvents();
        }

        void ListEvents()
        {
            var eventsUri = CalendarContract.Events.ContentUri;

            string[] eventsProjection = { 
                CalendarContract.Events.InterfaceConsts.Id,
                CalendarContract.Events.InterfaceConsts.Title,
                CalendarContract.Events.InterfaceConsts.Dtstart
             };

            var cursor = ManagedQuery(eventsUri, eventsProjection,
             String.Format("calendar_id={0}", _calId), null, "dtstart ASC");

            string[] sourceColumns = {
                CalendarContract.Events.InterfaceConsts.Title, 
                CalendarContract.Events.InterfaceConsts.Dtstart
            };

            int[] targetResources = { Resource.Id.eventTitle, Resource.Id.eventStartDate };

            var adapter = new SimpleCursorAdapter(this, Resource.Layout.CalEventListItem,
             cursor, sourceColumns, targetResources);

            adapter.ViewBinder = new ViewBinder();

            ListAdapter = adapter;

            ListView.ItemClick += (sender, e) =>
            {
                int i = (e as AdapterView.ItemClickEventArgs).Position;

                cursor.MoveToPosition(i);
                int eventId = cursor.GetInt(cursor.GetColumnIndex(eventsProjection[0]));
                var uri = ContentUris.WithAppendedId(CalendarContract.Events.ContentUri, eventId);
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            };
        }

        class ViewBinder : Java.Lang.Object, SimpleCursorAdapter.IViewBinder
        {
            public bool SetViewValue(View view, ICursor cursor, int columnIndex)
            {
                if (columnIndex == 2)
                {
                    long ms = cursor.GetLong(columnIndex);

                    DateTime date =
                        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(ms).ToLocalTime();

                    TextView textView = (TextView)view;
                    textView.Text = date.ToLongDateString();

                    return true;
                }
                return false;
            }
        }
    }
}