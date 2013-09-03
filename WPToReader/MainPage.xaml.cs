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
using WPToReader.Resources;
using System.IO;
using System.Net.Http;
using WPToReaderClassLib;
using System.Data.Linq;
using System.Data.Common;

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

            // Start background task is needed
            
            AppSettings appsettings = new AppSettings();
            if (appsettings.BgTaskEnabled)
            {
                Settings settings = new Settings();
                settings.StartWP2ReaderAgent();
            }
        }


        // Appbar at the bottom of the screen
        // TODO: Add help
        private void BuildLocalizedApplicationBar()
        {

            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Minimized;
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

            if (message == "Operatoin successful!")
                clearData();
            else if( message.Substring(0, "Not Found".Length) == "Not Found" )
            {
                string url = message.Substring("Not Found:".Length);
                try
                {
                    W2RTask task = new W2RTask();
                    task._uri = url;


                    TaskDataContext dataContext = new TaskDataContext(TaskDataContext.DBConnectionString);
                    Table<W2RTask> table = dataContext.GetTable<W2RTask>();
                    table.InsertOnSubmit(task);

                    dataContext.SubmitChanges();
                    message = "Document will be sent when phone connects to network";
                    clearData();
                }
                catch (DbException exp)
                {
                    string strUniqueConstErr = "A duplicate value cannot be inserted into a unique index.";
                    if (strUniqueConstErr == exp.Message.Substring(0, strUniqueConstErr.Length))
                    {
                        message = "This url is already queued";
                        clearData();
                    }
                    else
                        message = exp.Message;
                }
                
               
            }

            // Inform user with appropriate message
            MessageBox.Show(message, "Compose", MessageBoxButton.OK);            
        }

        bool m_isBackgroundTaskEnabled = false;
    }
}
