using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WPToReader.Resources;
using System.IO;
using System.Net.Http;

namespace WPToReader
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
        }


        // Appbar at the bottom of the screen
        // TODO: Add help
        private void BuildLocalizedApplicationBar()
        {

            ApplicationBar = new ApplicationBar();

            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem("Settings");
            appBarMenuItem.Click += onSettings;

            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        private void onSettings(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //clearData();
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void clearData()
        {
            Title.Text = "";
            Author.Text = "";
            Url.Text = "";
            Text.Text = "";
        }

        private void onCancle(object sender, RoutedEventArgs e)
        {
            clearData();
        }

        private async void onClick(object sender, RoutedEventArgs e)
        {
            string data = "";
            if (Url.Text != "")
                data += "url=" + Url.Text;
            else
                data += "text=" + Text.Text;

            //sendRequest(data);
            AppSettings settings = new AppSettings();
            String message = await SendToReaderAPI.sendDoc(settings.UserName, settings.Password, Url.Text, Author.Text, Title.Text, Text.Text);

            // Inform user with appropriate message
            MessageBox.Show(message, "Compose", MessageBoxButton.OK);

            if (message == "Operatoin successful!")
                clearData();
        }

    }
}