using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using WPToReaderClassLib;
using System.Data.Linq;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace WPToReadderScheduledTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>


        protected override async void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background

            BgTaskHelper.RemoveSentDocs();
            await BgTaskHelper.SendPendingDocs();

            // If debugging is enabled, launch the agent again in one minute.
#if DEBUG_AGENT
            string toastMessage = "WP2Reader BG Task running";

            // Launch a toast to show that the agent is running.
            // The toast will not be shown if the foreground application is running.
            ShellToast toast = new ShellToast();
            toast.Title = "Background Agent Sample";
            toast.Content = toastMessage;
            toast.Show();

            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif            
            // Call NotifyComplete to let the system know the agent is done working.
            NotifyComplete();


        }
    }
}