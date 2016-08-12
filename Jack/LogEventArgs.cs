using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(string message, int level = 0)
        {
            Message = message;
            Level = level;
        }

        public string Message
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }
    }
}
