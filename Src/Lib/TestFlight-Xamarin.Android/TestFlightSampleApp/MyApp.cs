using System;
using Android.App;
using Android.Runtime;
using Com.TestFlightApp.Lib;

namespace TestFlightSampleApp
{
    [Application]
    public class MyApp : Application
    {
        public MyApp(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        public override void OnCreate()
        {
            base.OnCreate();
            
            // Initialize the TestFlight framework.
            TestFlight.TakeOff(this, "774ab641-4fb5-41eb-9530-8fd743f36a7b");
            
            // Add an exception handler for all uncaught exceptions.
            AndroidEnvironment.UnhandledExceptionRaiser += MyApp_UnhandledExceptionHandler;
        }

        void MyApp_UnhandledExceptionHandler(object sender, RaiseThrowableEventArgs e)
        {
            // Send the Exception to TestFlight.
            TestFlight.SendCrash(e.Exception);

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