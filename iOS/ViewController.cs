﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using UIKit;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Foundation;

namespace TestAppCenter.iOS
{
    public partial class ViewController : UIViewController
    {
        int count = 1;

        private void GenerateDummyException()
        {

            try
            {
                int divByZero = 95 / int.Parse("0");
            }
            catch (DivideByZeroException ex)
            {
                Crashes.TrackError(ex);
            }

        }

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start ();
#endif

            // Perform any additional setup after loading the view, typically from a nib.
            ClickMeButton.TouchUpInside += delegate
            {
                var title = string.Format("{0} clicks!", count++);
                ClickMeButton.SetTitle(title, UIControlState.Normal);

                Analytics.TrackEvent($"Button Clicks from iOS - with Info",
                                    new Dictionary<string, string>
                                    {
                                        { "Category", $"Testing - {count}" }
                                    });

                // Crashes.GenerateTestCrash();
                // GenerateDummyException();

            };

            ViewMeButton.TouchUpInside += delegate
            {

                PerformSegue("ViewMeSegue", ViewMeButton);


            };

            Analytics.TrackEvent("View Controller loaded - iOS");
            //UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.        
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            var viewMeViewController = segue.DestinationViewController
                                            as ViewMeViewController;
            viewMeViewController.ViewMeString = ClickMeTextField.Text;
        }

    }
}
