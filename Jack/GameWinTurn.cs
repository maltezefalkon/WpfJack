using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class GameWinTurn : PlayerTurn
    {
        public GameWinTurn(Player actingPlayer)
            : base(actingPlayer)
        { }

        public override IEnumerable<IAction> GetActions(Game game)
        {
            yield break;
        }
    }
}
