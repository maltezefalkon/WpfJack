using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class JackWin : Win
    {
        public JackWin(JackPlayer jack)
            : base(jack)
        { }

        public override string ToString()
        {
            return "Jack WINS by capturing 3 treasures";
        }

        public override WinType WinType
        {
            get
            {
                return WinType.JackWin;
            }
        }
    }
}
