using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPToReaderClassLib
{
    public class TaskDataContext: DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/ToDo.sdf";

        // Pass the connection string to the base class.
        public TaskDataContext(string connectionString) : base(connectionString) { } 

        // Specify a single table for the to-do items.
        public Table<W2RTask> ToDoItems;
    }
}
