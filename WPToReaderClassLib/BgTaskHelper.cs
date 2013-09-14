using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static async Task<bool> SendPendingDocs()
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

                ShellToast toast = new ShellToast();
                toast.Title = "WPToReader";
                toast.Content = message;
                toast.Show();

            }

            dataContext.SubmitChanges();

            return bRes;
        }

    }
}
