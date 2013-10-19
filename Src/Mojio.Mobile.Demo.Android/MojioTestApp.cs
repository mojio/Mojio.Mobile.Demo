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
using Com.TestFlightApp.Lib;
using Mojio.Client;

namespace Mojio.Mobile.Android.Test
{
    [Application]
    public class MojioTestApp : Application
	{
        public MojioTestApp(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        public override void OnCreate()
        {
            base.OnCreate();

			// Add an exception handler for all uncaught exceptions.
			AndroidEnvironment.UnhandledExceptionRaiser += MyApp_UnhandledExceptionHandler;
            
            // Initialize the TestFlight framework.
			if( Config.TestFlightApi != null )
				TestFlight.TakeOff(this, Config.TestFlightApi);

			// Setup Logger
			DependancyResolver.Set<ILogger> (new MojioLog ());

			if (Config.MojioAppId == null || Config.MojioAppKey == null)
				throw new Exception ("You must fill in the App ID and Key in Config.cs");

			// Setup Mojio Client
			DependancyResolver.Set<MojioClient> (new MojioClient (
					this, 
					new Guid (Config.MojioAppId), 
					new Guid (Config.MojioAppKey), 
					Config.MojioApiEndpoint
			));
        }

        void MyApp_UnhandledExceptionHandler(object sender, RaiseThrowableEventArgs e)
        {
            // Send the Exception to TestFlight.
			if( !String.IsNullOrEmpty(Config.TestFlightApi) )
            	TestFlight.SendCrash(e.Exception);

			// Log event
			DependancyResolver.Get<ILogger> ().Critical ("CRASH - " + e.Exception.Message);

            throw e.Exception;
        }

        protected override void Dispose(bool disposing)
        {
            // Remove the exception handler.
            AndroidEnvironment.UnhandledExceptionRaiser -= MyApp_UnhandledExceptionHandler;
            base.Dispose(disposing);
        }
    }
}