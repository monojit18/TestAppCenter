using System;
using DBG = System.Diagnostics;
using Java.Interop;
using Android.Content;
using Android.App;
using Android.Widget;
using Android.OS;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;

namespace TestAppCenter.Droid
{
    [Activity(Label = "TestAppCenter", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {

        private void GenerateDummyException()
        {

            try
            {
                int divByZero = 100 / int.Parse("0");
            }
            catch (DivideByZeroException ex)
            {
                Crashes.TrackError(ex);
            }

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            if (AppCenter.Configured == false)
            {

                Push.PushNotificationReceived += (sender, e) =>
                {
                    var summary = $"Push notification received:" +
                                    $"\n\tNotification title: {e.Title}" +
                                    $"\n\tMessage: {e.Message}";


                    if (e.CustomData != null)
                    {
                        summary += "\n\tCustom data:\n";
                        foreach (var key in e.CustomData.Keys)
                        {
                            summary += $"\t\t{key} : {e.CustomData[key]}\n";
                            System.Diagnostics.Debug.WriteLine(summary);

                        }
                    }

                };
            }

            AppCenter.Start("81e6286f-3c43-4ca4-afaf-8015abf34563", typeof(Analytics),
                            typeof(Crashes), typeof(Push));
            Analytics.TrackEvent("Main Acitivity loaded - Android");

            // Get our button from the layout resource,
            // and attach an event to it

            var clickMeTextField = FindViewById<EditText>(Resource.Id.ClickMeTextField);
            Button clickMeButton = FindViewById<Button>(Resource.Id.ClickMeButton);
            Button viewMeButton = FindViewById<Button>(Resource.Id.ViewMeButton);


            clickMeButton.Click += delegate
            {

                clickMeTextField.Text = clickMeButton.Text;

                Analytics.TrackEvent("Button Clicked from  - Android");
                // Crashes.GenerateTestCrash();
                GenerateDummyException();

            };

            viewMeButton.Click += delegate
            {

                var viewMeIntent = new Intent(this, typeof(ViewMeActivity));
                viewMeIntent.PutExtra("ViewMeString", clickMeTextField.Text);
                StartActivity(viewMeIntent);

            };
        }
                
        [Export("viewMeBackdoor")]
        public void ViewMeBackdoor(string backdoorString)
        {

            DBG.Debug.WriteLine(backdoorString);

        }
    }
}

