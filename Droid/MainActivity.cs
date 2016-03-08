using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Net;

namespace AwaitTest.Droid
{
	[Activity(Label = "AwaitTest", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);

			button.Click += async delegate {
				button.Enabled = false;
				button.Text = "Getting page...";
				var client = new MyClient();
				try
				{
					var response = await client.GetPage(new Uri("http://google.com"));
					if (response.uri == null || response.status == (HttpStatusCode)0)
					{
						button.Text = "Corrupted async result";
					}
					else
					{
						button.Text = "Success";
					}
				}
				catch
				{
					button.Text = "Http request failed (check connection)";
				}
				finally
				{
					button.Enabled = true;
				}
			};
		}
	}
}


