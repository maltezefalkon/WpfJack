using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class GiantWin : Win
    {
        public GiantWin(GiantPlayer player)
            : base(player)
        {

        }
    }

    public class GiantHorizontalWin : GiantWin
    {
        public GiantHorizontalWin(GiantPlayer player)
            : base(player)
        {

        }

        public override string ToString()
        {
            return "Giant WINS Horizontally";
        }
    }

    public class GiantVerticalWin : GiantWin
    {
        public GiantVerticalWin(GiantPlayer player)
            : base(player)
        {

        }

        public override string ToString()
        {
            return "Giant WINS Vertically";
        }
    }

    public class GiantWinByDiscard : GiantWin
    {
        public GiantWinByDiscard(GiantPlayer player)
            : base(player)
        {

        }

        public override string ToString()
        {
            return "Giant WINS by Discard";
        }
    }
}
