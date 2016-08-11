using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.GiantStrategy
{
    public class VerticalStrategy : BaseGiantStrategy
    {
        public override IEnumerable<Tuple<IAction, decimal>> GetPossibleActions(Game game)
        {
            throw new NotImplementedException();
        }

        public override decimal GetStrength(Game game)
        {
            throw new NotImplementedException();
        }
    }
}
