using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPToReaderClassLib
{
    public class BgTaskHelper
    {
        static TaskDataContext dataContext = new TaskDataContext(TaskDataContext.DBConnectionString);

        public static void RemoveSentDocs()
        {
            try
            {
                Table<W2RTask> table = dataContext.GetTable<W2RTask>();

                // Define query to gather all of the to-do items.
                var toDoItemsInDB = from W2RTask task in dataContext.ToDoItems
                                    where task._isDone == true
                                    select task;

                // Execute query and place results into a collection.
                ObservableCollection<W2RTask> sentDocuments = new ObservableCollection<W2RTask>(toDoItemsInDB);

                foreach (W2RTask tsk in sentDocuments)
                {
                    table.DeleteOnSubmit(tsk);
                }

                dataContext.SubmitChanges();
            }
            catch (DbException e)
            {
                ShellToast toast = new ShellToast();
                toast.Title = "WPToReader";
                toast.Content = "Exception occured in background task processing(RemoveDocs). Please report to developer.Error Msg = " + e.Message;
                toast.Show();
            }
        }

        public static async Task<bool> SendPendingDocs()
        {
            bool bRes = true;
            bool bSent = false;
            try
            {

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
                    {
                        bSent = true;
                        tsk._isDone = true;
                    }
                    else if (message.Substring(0, "Not Found".Length) == "Not Found")
                    {
                        bRes = false;
                        break;
                    }
                    else if (message != "Exceeded the rate limit. Please wait 10 seconds and try again.")
                    {
                        //Something is wrong with this request. Get it out of the database.
                        tsk._isDone = true;
                        bRes = false;
                    }
                }

                dataContext.SubmitChanges();
                
                if (bSent && bRes)
                {
                    ShellToast toast = new ShellToast();
                    toast.Title = "WPToReader";
                    toast.Content = "All linkes has been sent to kindle!!!";
                    toast.Show();                
                }                               
            }
            catch (Exception e)
            {
                ShellToast toast = new ShellToast();
                toast.Title = "WPToReader";
                toast.Content = "Exception occured in background task processing(SendDocs). Please report to developer.Error Msg = " + e.Message;
                toast.Show();
            }

            return bRes;
        }

    }
}
