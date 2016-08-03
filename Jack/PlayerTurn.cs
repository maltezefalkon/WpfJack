using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public abstract class PlayerTurn
    {
        public PlayerTurn(Player actingPlayer)
        {
            ActingPlayer = actingPlayer;
        }

        public abstract IEnumerable<IAction> GetActions(Game game);

        public Player ActingPlayer
        {
            get;
            private set;
        }
    }
}
