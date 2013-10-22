Mojio.Mobile.Demo
==================

All of our apps are developed using [Xamarin](http://xamarin.com/), a cross-platform development tool
which allows mobile apps to be written in C# for Android, iOS, and Windows Phone.

Currently only an Android demo is available.  More will be coming soon.

## Android ##

The [Mojio.Mobile.Demo.Android](http://github.com/mojio/Mojio.Mobile.Demo/Src/Mojio.Mobile.Demo.Android/)
project demonstrates the use of the C# [MojioClient](http://github.com/mojio/Mojio.Client)
to communicate with our API. As well as a very simple example of registering to an event push notification.

### Android Configuration ###

To build the demo app, you will need to [get your own MOJIO app id and key](http://sandbox.developer.moj.io/account/apps).
Then you can either edit the Config.cs file directly, or create a new **Config.Local.cs** to override the default values:

    namespace Mojio.Mobile.Demo.Android
    {
	    public partial class Config
	    {
		    public Config()
		    {
		 	    MojioAppId = "12345678-1234-1234-1234-1234567890AB";
			    MojioAppKey = "12345678-1234-1234-1234-1234567890AB";
		    }
	    }
    }
