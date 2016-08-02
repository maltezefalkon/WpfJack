using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class JackSneakAction : CompoundAction
    {
        public JackSneakAction()
            : base(new JackShiftAction(), new JackShiftAction(), new JackShiftAction())
        { }
    }

    public class JackShiftAction : CardShiftAction
    {
        public override int NumberOfCardsAffected => 1;
    }
}
