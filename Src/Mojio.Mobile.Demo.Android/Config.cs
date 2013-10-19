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
using Mojio.Client;

namespace Mojio.Mobile.Android.Test
{
	public static class Config
	{
		/** 
		 * To run the app, you MUST supply a valid mojio app ID and secret key.
		 * 
		 * Get your mojio App ID and SecretKey from http://sandbox.developer.moj.io/account
		 */
		public static string MojioAppId = null;
		public static string MojioAppKey = null;

		// The API endpoint to use.  Default is Sandbox.  This MUST match where you got your app id/key.
		public static string MojioApiEndpoint = MojioClient.Sandbox;

		// This is used to setup test flight if you wish to use it for deployment.  (OPTIONAL)
		public static string TestFlightApi = null;
	}
}

