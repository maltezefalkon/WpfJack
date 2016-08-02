using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class PlayerTurn
    {
        public PlayerTurn(Player actingPlayer)
        {
            Actions = new List<IAction>();
        }

        public List<IAction> Actions { get; private set; }
    }
}
