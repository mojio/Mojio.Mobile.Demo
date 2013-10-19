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
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class UserInfoActivity : BaseActivity
    {
        private TextView userInfo, deviceSelection;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.UserInfo);
            InitiateView();

            if (!ConnectedToNetwork)
                return;

            if (!Client.IsLoggedIn())
            {
                ResetDisplay();
                GotoLogin();
                return;
            }
            DisplayUserInfo();

            LoadDeviceSelection();
            DisplayDeviceSelection();
        }

        private void InitiateView()
        {
            // Set up buttons
            Button logout = FindViewById<Button>(Resource.Id.LogoutButton);
            logout.Click += new EventHandler(OnLogoutClicked);
            Button selectDevice = FindViewById<Button>(Resource.Id.SelectDeviceButton);
            selectDevice.Click += new EventHandler(OnSelectDeviceClicked);
            Button deviceInfo = FindViewById<Button>(Resource.Id.DeviceInfoButton);
            deviceInfo.Click += (o, e) => { GotoDeviceInfo(); };

            userInfo = FindViewById<TextView>(Resource.Id.UserInfo);
            deviceSelection = FindViewById<TextView>(Resource.Id.DeviceSelection);
        }

        private void AddCrashButton()
        {
            Button crashBt = new Button(this);
            crashBt.Text = "C# crash";
            crashBt.Click += (o, e) =>{throw new ApplicationException("App is throwing an error."); };
            LinearLayout ll = FindViewById<LinearLayout>(Resource.Id.UserInfoLayout);
            ll.AddView(crashBt);
        }
        
        private void OnLogoutClicked(object sender, EventArgs e)
        {
            ResetClient();
            ResetDisplay();
            GotoLogin();
        }

        private void GotoLogin()
        {
            var login = new Intent(this, typeof(LoginActivity));
            login.AddFlags(ActivityFlags.ClearTop);
            StartActivity(login);
        }

        private void GotoDeviceInfo()
        {
			// Launch event list activity
            var di = new Intent(this, typeof(EventTextActivity));
            StartActivity(di);
        }

        private void OnSelectDeviceClicked(object sender, EventArgs e)
        {
            Dialog dialog = new Dialog(this);
            dialog.SetTitle(Resource.String.DeviceDialogTitle);

            // Auto size the dialog based on it's contents
            dialog.Window.SetLayout(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            dialog.SetContentView(Resource.Layout.SelectDevice);
            dialog.Window.SetTitleColor(global::Android.Graphics.Color.LightYellow);

            var layout = dialog.FindViewById<LinearLayout>(Resource.Id.SelectDeviceLayout);
            if (layout == null || Client == null)
                return;

			// Query API for all of users mojio devices
            var res = Client.UserMojios(Client.CurrentUser.Id);
            foreach (Device moj in res.Data)
            {
                Button button = new Button(this);
                button.Text = string.Format("Name:{0} \nId:{1}", moj.Name, moj.IdToString);
                button.Click += (o, args) =>
                {
                    Dev = moj;
                    dialog.Dismiss();
                    SaveDeviceSelection();
                    DisplayDeviceSelection();
                };
                layout.AddView(button);
            }
            dialog.Show();
        }

        // Display user info 
        // Reset displayed text if no user logged in
        private void DisplayUserInfo()
        {
            if (userInfo == null)
                return;

            var user = Client.CurrentUser;
            if (user != null)
            {
                string info = string.Format("Hello, {0}.\nUsername: {1}\nEmail:    {2}\n",
                    user.FirstName, user.UserName, user.Email);
                var address = Client.GetShipping();
                if( address != null )
                    info += string.Format("Shipping address:\n{0}, {1} \n{2}, {3} \n{4}\n{5}",
                        address.Address1, address.Address2, address.City, address.State, address.Zip, address.Country);
                userInfo.Text = info;
            }
            else
                userInfo.SetText(Resource.String.DefaultUserInfo);
        }

        // Display current selected device
        // Reset displayed text if no device selected
        private void DisplayDeviceSelection()
        {
            if (deviceSelection == null)
                return;
            if (Dev != null)
                deviceSelection.Text = string.Format("Selected device:\n Name:\t{0}\nID:\t{1}\n",
                    Dev.Name, Dev.IdToString);
            else
                deviceSelection.SetText(Resource.String.DefaultDeviceSelection);
        }

        /// <summary>
        /// Remove userinfo from previous logged in user
        /// </summary>
        private void ResetDisplay()
        {
            var logout = FindViewById<Button>(Resource.Id.LogoutButton);
            if (logout != null)
                logout.SetText(Resource.String.LoginButtonText);

            LinearLayout ll = FindViewById<LinearLayout>(Resource.Id.UserInfoLayout);
            if (ll != null)
            {
                ll.RemoveAllViews();
                ll.AddView(userInfo);
                ll.AddView(logout);
            }
            DisplayUserInfo();
        }

        /// <summary>
        /// Reset User of client and selection of device
        /// </summary>
        private void ResetClient()
        {
            Client.ClearUser();
            Dev = null;
            SaveDeviceSelection();
        }
    }
}