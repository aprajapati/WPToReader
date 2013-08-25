/*
 * Copyright 2013-present InnocentDevil
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */
 
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
