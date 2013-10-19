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
using Android.Provider;
using Java.Util;

namespace Mojio.Mobile.Android.Test
{
    [Activity(Label = "Calendar Activity")]
    public class CalendarActivity : ListActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.CalList);
             
            // List Calendars
            var calendarsUri = CalendarContract.Calendars.ContentUri;

            string[] calendarsProjection = {
               CalendarContract.Calendars.InterfaceConsts.Id,
               CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
               CalendarContract.Calendars.InterfaceConsts.AccountName
            };

            var cursor = ManagedQuery(calendarsUri, calendarsProjection, null, null, null);

            string[] sourceColumns = {CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName, 
                CalendarContract.Calendars.InterfaceConsts.AccountName};

            int[] targetResources = { Resource.Id.calDisplayName, Resource.Id.calAccountName };

            SimpleCursorAdapter adapter = new SimpleCursorAdapter(this, Resource.Layout.CalListItem,
                cursor, sourceColumns, targetResources);

            ListAdapter = adapter;
            
            ListView.ItemClick += (sender, e) =>
            {
                int i = (e as AdapterView.ItemClickEventArgs).Position;

                cursor.MoveToPosition(i);
                int calId = cursor.GetInt(cursor.GetColumnIndex(calendarsProjection[0]));

                var showEvents = new Intent(this, typeof(CalendarEventListActivity));
                showEvents.PutExtra("calId", calId);
                StartActivity(showEvents);
            };
        }
    }
}