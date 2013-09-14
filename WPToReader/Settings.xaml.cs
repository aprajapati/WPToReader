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
using WPToReaderClassLib;
using Microsoft.Phone.Scheduler;

namespace WPToReader
{
    public partial class Settings : PhoneApplicationPage
    {
        PeriodicTask periodicTask;

        string periodicTaskName = "WP2ReaderAgent";
        public bool agentsAreEnabled = true;

        public Settings()
        {
            InitializeComponent();

            AppSettings settings = new AppSettings();
            UserName.Text = settings.UserName;
            Password.Password = settings.Password;
            BkGroundTaskEnable.IsChecked = settings.BgTaskEnabled;
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

        private void BkGroundTaskEnable_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckBoxEvents)
                return;
            StartWP2ReaderAgent();

        }

        bool ignoreCheckBoxEvents = false;


        public void StartWP2ReaderAgent()
        {
            // Variable for tracking enabled status of background agents for this app.
            agentsAreEnabled = true;

            // Obtain a reference to the period task, if one exists
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            periodicTask.Description = "Background Agent for WPToReader app";
            
            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(periodicTask);
                //PeriodicStackPanel.DataContext = periodicTask;
#if DEBUG_AGENT
                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
                ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                    agentsAreEnabled = false;
                    BkGroundTaskEnable.IsChecked = false;
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.

                }
                BkGroundTaskEnable.IsChecked = false;
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
                BkGroundTaskEnable.IsChecked = false;
            }

            AppSettings settings = new AppSettings();
            settings.BgTaskEnabled = agentsAreEnabled;
        }

        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }

        private void BkGroundTaskEnable_Unchecked(object sender, RoutedEventArgs e)
        {
            RemoveAgent(periodicTaskName);
        }


    }
}
