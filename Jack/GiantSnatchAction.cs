using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class GiantSnatchAction : CompoundAction
    {
        public GiantSnatchAction()
            : base(new GiantShiftAction(), new GiantShiftAction())
        {
        }
    }

    public class GiantShiftAction : CardShiftAction
    {
        public override int NumberOfCardsAffected => 1;
    }
}
