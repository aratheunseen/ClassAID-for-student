﻿using ClassAid.DataContex;
using System;
using ClassAid.Models.Users;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Diagnostics;
using ClassAid.Models;

namespace ClassAid.Views.AdminViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminLoginPage : ContentPage
    {
        public static Command TapCommand;
        public static bool isCliclable = false;
        public AdminLoginPage()
        {
            InitializeComponent();
            //privacyURI.Command = new Command(async () =>
            //await Launcher.OpenAsync(new Uri("https://mahmudx.com")));
            TapCommand = new Command(() => BtnAdd_Clicked());
        }
        private async void BtnAdd_Clicked()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                Admin admin = new Admin(userName.Text + "admin", userPass.Text)
                {
                    IsAdmin = true
                };
                activityIndicator.IsRunning = true;
                var tempAdmin =
                    await FirebaseHandler.GetAdminAsync(admin.Key);
                if (tempAdmin == null)
                {
                    activityIndicator.IsRunning = false;
                    await Navigation.PushAsync(
                        new AdditionalDetails(admin));
                    FirebaseHandler.InsertAdmin(admin);
                }
                else
                {
                    Preferences.Set(PrefKeys.IsLoggedIn, true);
                    Preferences.Set(PrefKeys.AdminKey, tempAdmin.Key);
                    Preferences.Set(PrefKeys.IsAdmin, true);
                    Preferences.Set(PrefKeys.Key, tempAdmin.Key);
                    activityIndicator.IsRunning = false;
                    Application.Current.MainPage =
                        new NavigationPage(new Dashboard(tempAdmin));
                    LocalDbContex.CreateTables();
                    LocalDbContex.SaveUser(tempAdmin);
                    LocalDbContex.SaveBatchDetails(tempAdmin.BatchDetails);
                    foreach (var item in tempAdmin.EventList)
                        LocalDbContex.SaveEvents(item);
                    foreach (var item in tempAdmin.TeacherList)
                        LocalDbContex.SaveTeacher(item);
                    foreach (var item in tempAdmin.ScheduleList)
                        LocalDbContex.SaveSchedules(item);
                }
            }
            else
            {
                DependencyService.Get<Toast>().Show("No INTERNET connection.");
            }
        }
        private void Form_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(userName.Text) ||
                string.IsNullOrWhiteSpace(userPass.Text)||
                userName.Text.Length < 6 ||
                userPass.Text.Length < 6 )

            {
                signInBtn.Command = null;
            }
            else
                signInBtn.Command = TapCommand;
        }
    }
}