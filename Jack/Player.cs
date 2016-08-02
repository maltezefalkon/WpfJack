using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public abstract class Player
    {
        public List<IAction> AvailableActions
        {
            get;
            protected set;
        }

        public abstract PlayerTurn GetNextTurn(Game game);
    }

}
