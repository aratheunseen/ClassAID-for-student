﻿using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ClassAid.Models.Users;
using ClassAid.Views.AdminViews.Settings;
using Xamarin.Essentials;
using ClassAid.DataContex;
using ClassAid.Models;
using System.Windows.Input;

namespace ClassAid.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dashboard : ContentPage
    {
        private Shared user;
        private static string timeFormat = @"dd\:hh\:mm";
        public ICommand teamCodeCopyCommand 
        { 
            get 
            { 
                return new Command(async () =>
                {
                    await Clipboard.SetTextAsync(user.TeamCode);
                    DependencyService.Get<Toast>().Show("Team code copied.");
                });
            } 
        }
        public ICommand addScheduleCommand
        {
            get
            {
                return new Command(async () =>
                await Navigation.PushAsync(new AddSchedulePage(user)));
            }
        }
        public ICommand addEventCommand
        {
            get
            {
                return new Command(async ()=>
                await Navigation.PushAsync(new AddEventPage(user)));
            }
        }
        public ICommand fullScheduleCommand
        {
            get
            {
                return new Command(async () =>
                await Navigation.PushAsync(new ViewSchedulePage(user)));
            }
        }
        public ICommand fullEventCommand
        {
            get
            {
                return new Command(async () =>
                await Navigation.PushAsync(new ViewEventPage(user)));
            }
        }
        public static Command chatPageCommand;
        public Dashboard(Shared user)
        {
            InitializeComponent();
            this.user = user;
            InitializeData();
        }
        // TODO: Remove this section on shipment
        // START

        public Dashboard()
        {
            InitializeComponent();
            FetchData();
        }
        private async void FetchData()
        {
            try
            {
                user = await LocalStorageEngine.ReadDataAsync<Shared>
                    (FileType.Shared);
            }
            catch (Exception)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    string key = Preferences.Get("adminKey", "");
                    user = await FirebaseHandler
                    .GetUser(key, user.IsAdmin);
                }
                else
                {
                    DependencyService.Get<Toast>().Show("ERROR. Please connect to Internet to resolve the issue.");
                    Application.Current.MainPage = new StartPage();
                    return;
                }

            }
            InitializeData();
        }

        private async void InitializeData()
        {
            if (!user.IsAdmin)
            {
                addScheduleBtnImage.IsVisible = false;
                addNoticeBtnImage.IsVisible = false;
                teamCode.IsVisible = false;
                await FirebaseHandler.RealTimeConnection(
                    CollectionTables.ScheduleList,
                    user.ScheduleList);
                await FirebaseHandler.RealTimeConnection(
                    CollectionTables.StudentList,
                    user.StudentList);
                await FirebaseHandler.RealTimeConnection(
                    CollectionTables.EventList,
                    user.EventList);
                await FirebaseHandler.RealTimeConnection(
                    CollectionTables.TeacherList,
                    user.TeacherList);
            }
            ////if (user.ScheduleList == null)
            ////    user.ScheduleList =
            ////        new ObservableCollection<ScheduleModel>();

            ////if (user.EventList == null)
            ////    user.EventList =
            ////        new ObservableCollection<EventModel>();

            ////if (user.TeacherList == null)
            ////    user.TeacherList =
            ////        new ObservableCollection<Teacher>();

            ////if (user.StudentList == null)
            ////    user.StudentList =
            ////        new ObservableCollection<Student>();

            profileBtn.Command = new Command(() => 
            Navigation.PushAsync(new StudentProfile()));
            InitLabel();

            // TODO: Remove this section on shipment
            // START
            user.ScheduleList.CollectionChanged += ScheduleList_CollectionChanged;
        }

        private void ScheduleList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InitLabel();
        }
        void InitLabel()
        {
            try
            {
                teamCode.Text = user.TeamCode;
                firstScheduleCourseCode.Text = user.ScheduleList[0].CourseCode;
                firstScheduleCourseName.Text = user.ScheduleList[0].Subject;
                firstScheduleStart.Text = user.ScheduleList[0].StartTime.ToString(timeFormat);
                firstScheduleEnd.Text = user.ScheduleList[0].EndTime.ToString(timeFormat);

                secondScheduleCourseCode.Text = user.ScheduleList[1].CourseCode;
                secondScheduleCourseName.Text = user.ScheduleList[1].Subject;
                secondScheduleStart.Text = user.ScheduleList[1].StartTime.ToString(timeFormat);
                secondScheduleEnd.Text = user.ScheduleList[1].EndTime.ToString(timeFormat);
            }
            catch (Exception) { }
        }
        // END
    }
}