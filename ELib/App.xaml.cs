﻿using ELib.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ELib
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new TestLog())
            {
                BarBackgroundColor = Color.FromHex("#6A2C90"),
                BarTextColor = Color.White,

            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
