using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public interface IStrategy
    {
        event EventHandler<LogEventArgs> Log;
        decimal GetStrength(Game game);
        IEnumerable<Tuple<IAction, decimal>> GetPossibleActions(Game game);
    }
}
