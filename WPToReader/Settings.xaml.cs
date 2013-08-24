using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WPToReader
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();

            AppSettings settings = new AppSettings();
            UserName.Text = settings.UserName;
            Password.Password = settings.Password;
        }

        private async void onSave(object sender, EventArgs e)
        {
            string userName = UserName.Text;
            string password = Password.Password;

            String message = await SendToReaderAPI.checkCreds(userName, password);

            // Inform user with appropriate message
            MessageBox.Show(message, "Settings", MessageBoxButton.OK);

            if (message == "Operation successful!")
            {
                AppSettings settings = new AppSettings();
                settings.UserName = userName;
                settings.Password = password;

                // Go back to main screen on success
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
        }

        private void onReset(object sender, EventArgs e)
        {
            UserName.Text = "";
            Password.Password = "";
        }
    }
}