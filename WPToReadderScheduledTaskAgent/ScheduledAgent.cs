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

        TaskDataContext dataContext = new TaskDataContext(TaskDataContext.DBConnectionString);

        void RemoveSentDocs()
        {
            Table<W2RTask> table =  dataContext.GetTable<W2RTask>();

            // Define query to gather all of the to-do items.
            var toDoItemsInDB = from W2RTask task in dataContext.ToDoItems where task._isDone == true
                                select task;

            // Execute query and place results into a collection.
            ObservableCollection<W2RTask>  sentDocuments = new ObservableCollection<W2RTask>(toDoItemsInDB);
            
            foreach (W2RTask tsk in sentDocuments)
            {
                table.DeleteOnSubmit(tsk);
            }

            dataContext.SubmitChanges();
        }

        async Task<bool> SendPendingDocs()
        {
            bool bRes = true;

            Table<W2RTask> table = dataContext.GetTable<W2RTask>();

            // Define query to gather all of the to-do items.
            var toDoItemsInDB = from W2RTask task in dataContext.ToDoItems
                                where task._isDone == false
                                select task;

            // Execute query and place results into a collection.
            ObservableCollection<W2RTask> sentDocuments = new ObservableCollection<W2RTask>(toDoItemsInDB);

            foreach (W2RTask tsk in sentDocuments)
            {
                string url = tsk._uri;
                url = url.Trim(':');
                string message = await SendToReaderAPI.postMessage(url, "BGTask");

                if (message == "Operation successful!")
                    tsk._isDone = true;
                else if (message.Substring(0, "Not Found".Length) == "Not Found")
                {
                    bRes = false;
                    break;
                }

            }

            dataContext.SubmitChanges();

            return bRes;
        }

        protected override async void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background

            RemoveSentDocs();
            await SendPendingDocs();           

            // If debugging is enabled, launch the agent again in one minute.
#if DEBUG_AGENT
            string toastMessage = "WP2Reader BG Task running";

            // Launch a toast to show that the agent is running.
            // The toast will not be shown if the foreground application is running.
            ShellToast toast = new ShellToast();
            toast.Title = "Background Agent Sample";
            toast.Content = toastMessage;
            toast.Show();

  
#endif
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
            // Call NotifyComplete to let the system know the agent is done working.
            NotifyComplete();


        }
    }
}