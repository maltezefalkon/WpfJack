using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public abstract class GiantWin : Win
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

        public override WinType WinType
        {
            get
            {
                return WinType.GiantHorizontal;
            }
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

        public override WinType WinType
        {
            get
            {
                return WinType.GiantVertical;
            }
        }
    }

    public class GiantWinByDiscard : GiantWin
    {
        public GiantWinByDiscard(GiantPlayer player)
            : base(player)
        {

        }

        public override WinType WinType
        {
            get
            {
                return WinType.GiantDiscard;
            }
        }

        public override string ToString()
        {
            return "Giant WINS by Discard";
        }
    }
}
