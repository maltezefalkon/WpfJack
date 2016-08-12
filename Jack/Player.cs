using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public abstract class Player
    {
        public abstract PlayerTurn GetNextTurn(Game game);

        public abstract IEnumerable<IStrategy> GetStrategies(Game game);
    }

}
