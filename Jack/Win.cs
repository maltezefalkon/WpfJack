using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class Win
    {
        public Win(Player p)
        {
            WinningPlayer = p;
        }

        public virtual Player WinningPlayer
        {
            get;
            protected set;
        }
    }
}
