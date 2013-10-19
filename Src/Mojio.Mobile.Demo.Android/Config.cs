/**
 * This is a sample config file.  To run this project you MUST input your own AppID and Key
 * 
 * If you wish to create an unversioned config file that will be persistent through updates
 * use can use a partial class   Config.Local.cs:
 * 
 * 
 * 
 *       namespace Mojio.Mobile.Demo.Android
 *       {
 *           public partial class Config
 *           {
 *               public Config()
 *               {
 *                   MojioAppId = "YOUR-APP-ID";
 *                   MojioAppKey = "YOUR-APP-KEY";
 *                   ...etc
 *                }
 *            }
 *        }
 */
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

namespace Mojio.Mobile.Demo.Android
{
	public partial class Config
	{
		/** 
		 * To run the app, you MUST supply a valid mojio app ID and secret key.
		 * 
		 * Get your mojio App ID and SecretKey from http://sandbox.developer.moj.io/account
		 */
		public string MojioAppId = null;
		public string MojioAppKey = null;

		// The API endpoint to use.  Default is Sandbox.  This MUST match where you got your app id/key.
		public string MojioApiEndpoint = MojioClient.Sandbox;

		// This is used to setup test flight if you wish to use it for deployment.  (OPTIONAL)
		public string TestFlightApi = null;
	}
}

