using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WPToReaderClassLib
{
    [Table]
    public class W2RTask
    {
        private string uri;
        private bool isDone;
        [Column( IsPrimaryKey = true, CanBeNull = false )]
        public string _uri
        {
            get
            {
                return uri;
            }
            set
            {
                if (value != "")
                    uri = value;
            }
        }
        [Column]
        public bool _isDone
        {
            get
            {
                return isDone;
            }
            set
            {
                isDone = value;
            }
        }
    }
}
